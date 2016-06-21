using System;
using System.Collections;
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
        public const int CurrentVersion = 2;

        public static LevelLoader Instance { get; private set; }

        public event Action LevelLoaded;

        public const int RunnerLayer = 8;

        [AssignedInUnity]
        public GameObject RunnerArea;

        public IList<GameObject> AllLevelObjects { get; private set; }

        public IList<IInGameObject> AllInGameObjects { get; private set; }

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        [UnityMessage]
        public void Start()
        {
            AllLevelObjects = new List<GameObject>();
            AllInGameObjects = new List<IInGameObject>();
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
                int version = 1;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("version:"))
                    {
                        version = Convert.ToInt32(line.Substring("version:".Length));
                    }

                    if (version != CurrentVersion)
                    {
                        throw new InvalidOperationException(String.Format("Level was saved in version {0} format. Current version is {1}. Please open the level in the level editor and resave.", version, CurrentVersion));
                    }

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

            foreach (var obj in AllLevelObjects)
            {
                obj.GetInterfaceComponent<IInGameObject>().PostAllDeserialized();
            }

            StartCoroutine(LevelLoadedAfterDelay());
        }

        private IEnumerator LevelLoadedAfterDelay()
        {
            yield return null;

            if (LevelLoaded != null)
                LevelLoaded();
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
            foreach (var levelObject in AllLevelObjects)
            {
                Destroy(levelObject);
            }

            AllInGameObjects.Clear();
            AllLevelObjects.Clear();
        }

        private void DeserializeGridItem(string line)
        {
            var parts = line.Split(',');

            var objectId = Convert.ToInt32(parts[0]);
            var x = Convert.ToInt32(parts[1]);
            var y = Convert.ToInt32(parts[2]);
            var typeId = Convert.ToInt32(parts[3]);
            var serializedObject = String.Join(",", parts.Skip(4).ToArray());

            var info = ObjectRegistration.Instance.GetInfo(typeId);

            if (info.ObjLevelPrefab == null || info.ObjEditorPrefab == null)
                return;

            var worldPosition = PlacementGrid.Instance.GetWorldPosition(x, y);

            var instance = CreateInstance(info, worldPosition, serializedObject, objectId);
            instance.StartGridPosition = new GridPosition(x, y);
        }

        private void DeserializeNonGridItem(string line)
        {
            var parts = line.Split(',');
            var objectId = Convert.ToInt32(parts[0]);
            var x = Convert.ToSingle(parts[1]);
            var y = Convert.ToSingle(parts[2]);
            var typeId = Convert.ToInt32(parts[3]);
            var serializedObject = String.Join(",", parts.Skip(4).ToArray());

            var info = ObjectRegistration.Instance.GetInfo(typeId);

            CreateInstance(info, new Vector2(x, y), serializedObject, objectId);
        }

        private IInGameObject CreateInstance(ObjectRegistrationInfo info, Vector2 worldPosition, string serializedObject, int objectId)
        {
            var instance = (GameObject)Instantiate(info.ObjLevelPrefab, worldPosition, Quaternion.identity);

            instance.transform.SetParent(RunnerArea.transform);
            instance.layer = RunnerLayer;

            var placedObject = instance.GetInterfaceComponent<IInGameObject>();
            placedObject.LevelLoader = this;
            placedObject.Id = objectId;
            placedObject.Deserialize(serializedObject);
            placedObject.Initialize();

            RegisterObject(instance);
            
            return placedObject;
        }

        public IInGameObject GetObject(int objectId)
        {
            return AllInGameObjects.FirstOrDefault(x => x.Id == objectId);
        }

        /// <summary>A registed object will be cleaned up when loading a new level</summary>
        public void RegisterObject(GameObject obj)
        {
            AllLevelObjects.Add(obj);

            var placedObject = obj.GetInterfaceComponent<IInGameObject>();

            if(placedObject != null)
                AllInGameObjects.Add(placedObject);
        }

        public IList<IInGameObject> GetGridObjectsThatStartedAtPosition(GridPosition position)
        {
            return AllInGameObjects.Where(x => x.StartGridPosition == position).ToList();
        }
    }
}