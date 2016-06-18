using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class LoadButton : MonoBehaviour
    {
        [AssignedInUnity]
        public InputField Input;

        [CalledFromUnity]
        public void Load()
        {
            if (String.IsNullOrEmpty(Input.text))
                return;

            var levelData = LoadLevelFile();

            if (levelData == null)
                return;

            WorkingLevel.Instance.Reset();
            WorkingLevel.Instance.DeserializeLevel(levelData);
        }

        private string LoadLevelFile()
        {
            SaveButton.EnsureSaveDirectoryExists();

            var saveFolder = SaveButton.GetGameSaveDirectory();
            var fileName = "level_" + Input.text + ".txt";

            var fullPath = Path.Combine(saveFolder, fileName);

            string contents;
            try
            {
                contents = File.ReadAllText(fullPath);
            }
            catch (IOException ex)
            {
                Debug.LogError(ex);
                return null;
            }

            return contents;
        }
    }
}