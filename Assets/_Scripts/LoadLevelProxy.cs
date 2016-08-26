using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Scripts
{
    [UnityComponent]
    public class LoadLevelProxy : MonoBehaviour
    {
        public string LevelName;

        [UnityMessage]
        public void Start()
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex != 1 || String.IsNullOrEmpty(LevelName))
                return;

            StartCoroutine(WaitToLoadLevel());
        }

        private IEnumerator WaitToLoadLevel()
        {
            GameStateController.Instance.CoverScreen(); // This is the earliest I can all this here.
            
            yield return null;

            GameStateController.Instance.LoadLevel(LevelName);

            Destroy(gameObject);
        }
    }
}