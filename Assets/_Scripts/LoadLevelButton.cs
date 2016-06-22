using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Scripts
{
    [UnityComponent]
    public class LoadLevelButton : MonoBehaviour
    {
        [AssignedInUnity]
        public InputField LevelNameInput;

        [AssignedInUnity]
        public LoadLevelProxy LoadLevelProxyPrefab;

        [CalledFromUnity]
        public void Load()
        {
            var levelName = LevelNameInput.text;

            if (String.IsNullOrEmpty(levelName))
                return;

            if (LevelLoader.CheckLevelExists(levelName) == false)
            {
                Debug.Log("No level named " + levelName);
                return;
            }

            var instance = Instantiate(LoadLevelProxyPrefab.gameObject);

            instance.GetComponent<LoadLevelProxy>().LevelName = levelName;

            SceneManager.LoadScene(1); 
        }
    }
}