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

            var fileName = "level_" + Input.text + ".txt";
            var fullPath = Path.Combine(GetGameSaveDirectory(), fileName);
            var contents = WorkingLevel.Instance.SerializeLevel();

            File.WriteAllText(fullPath, contents);
        }

        private static void EnsureSaveDirectoryExists()
        {
            Directory.CreateDirectory(GetGameSaveDirectory());
        }

        private static string GetGameSaveDirectory()
        {
            var myGames = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games");
            return Path.Combine(myGames, "Neuromouser");
        } 
    }
}