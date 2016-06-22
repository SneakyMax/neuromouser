using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class SaveButton : MonoBehaviour
    {
        [AssignedInUnity]
        public InputField Input;

        [CalledFromUnity]
        public void Save()
        {
            if (String.IsNullOrEmpty(Input.text))
                return;

            EnsureSaveDirectoryExists();

            var path = GetLevelPath(Input.text);
            var contents = WorkingLevel.Instance.SerializeLevel();

            File.WriteAllText(path, contents);
        }

        public static void EnsureSaveDirectoryExists()
        {
            Directory.CreateDirectory(GetGameSaveDirectory());
        }

        public static string GetGameSaveDirectory()
        {
            var myGames = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games");
            return Path.Combine(myGames, "Neuromouser");
        }

        public static string GetLevelPath(string levelName)
        {
            return Path.Combine(GetGameSaveDirectory(), GetLevelFileName(levelName));
        }

        private static string GetLevelFileName(string levelName)
        {
            return "level_" + levelName + ".txt";
        }
    }
}