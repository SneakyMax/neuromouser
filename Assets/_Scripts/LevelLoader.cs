using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets._Scripts.GameObjects;
using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class LevelLoader : MonoBehaviour
    {
        public const int RunnerLayer = 8;

        [AssignedInUnity]
        public GameObject RunnerArea;

        private IList<GameObject> allLevelObjects;

        [UnityMessage]
        public void Start()
        {
            allLevelObjects = new List<GameObject>();
        }

        public void LoadLevel(string levelName)
        {
            string contents = LoadFileContents(levelName);

            if (contents == null)
                return;

            DeserializeLevel(contents);
        }

        private static string LoadFileContents(string levelName)
        {
            SaveButton.EnsureSaveDirectoryExists();

            var saveDirectory = SaveButton.GetGameSaveDirectory();
            var fileName = "level_" + levelName + ".txt";
            var fullPath = Path.Combine(saveDirectory, fileName);

            string contents;
            try
            {
                contents = File.ReadAllText(fullPath);
            }
            catch (IOException)
            {
                Debug.LogWarning("Could not find level " + fileName);
                return null;
            }
            return contents;
        }

        private void DeserializeLevel(string level)
        {
            using (var reader = new StringReader(level))
            {
                string currentMode = null;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    switch (line)
                    {
                        case "grid:":
                            currentMode = "grid";
                            break;
                        case "nonGrid:":
                            currentMode = "nonGrid";
                            break;
                        default:
                            HandleLine(line, currentMode);
                            break;
                    }
                }
            }
        }

        private void HandleLine(string line, string currentMode)
        {
            if (String.IsNullOrEmpty(currentMode))
                return;

            switch (currentMode)
            {
                case "grid":
                    DeserializeGridItem(line);
                    break;
                case "nonGrid":
                    DeserializeNonGridItem(line);
                    break;
            }
        }

        private void Reset()
        {
            foreach (var levelObject in allLevelObjects)
            {
                Destroy(levelObject);
            }

            allLevelObjects.Clear();
        }

        private void DeserializeGridItem(string line)
        {
            var parts = line.Split(',');
            var x = Convert.ToInt32(parts[0]);
            var y = Convert.ToInt32(parts[1]);
            var id = Convert.ToInt32(parts[2]);
            var serializedObject = String.Join(",", parts.Skip(3).ToArray());

            var info = ObjectRegistration.Instance.GetInfo(id);

            if (info.ObjLevelPrefab == null || info.ObjEditorPrefab == null)
                return;

            var worldPosition = PlacementGrid.Instance.GetWorldPosition(x, y);

            CreateInstance(info, worldPosition, serializedObject);
        }

        private void DeserializeNonGridItem(string line)
        {
            var parts = line.Split(',');
            var x = Convert.ToSingle(parts[0]);
            var y = Convert.ToSingle(parts[1]);
            var id = Convert.ToInt32(parts[2]);
            var serializedObject = String.Join(",", parts.Skip(3).ToArray());

            var info = ObjectRegistration.Instance.GetInfo(id);

            CreateInstance(info, new Vector2(x, y), serializedObject);
        }

        private void CreateInstance(ObjectRegistrationInfo info, Vector2 worldPosition, string serializedObject)
        {
            var instance = (GameObject)Instantiate(info.ObjLevelPrefab, worldPosition, Quaternion.identity);

            instance.transform.SetParent(RunnerArea.transform);
            instance.layer = RunnerLayer;

            var placedObject = instance.GetInterfaceComponent<IInGameObject>();
            placedObject.LevelLoader = this;
            placedObject.Deserialize(serializedObject);
            placedObject.Initialize();

            allLevelObjects.Add(instance);
        }
    }
}