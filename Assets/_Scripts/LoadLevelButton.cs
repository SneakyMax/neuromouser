using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts
{
    [UnityComponent]
    public class LoadLevelButton : MonoBehaviour
    {
        [AssignedInUnity]
        public InputField LevelNameInput;

        [AssignedInUnity]
        public LevelLoader LevelLoader;

        public void Load()
        {
            var levelName = LevelNameInput.text;

            if (String.IsNullOrEmpty(levelName))
                return;

            LevelLoader.LoadLevel(levelName);
        }
    }
}