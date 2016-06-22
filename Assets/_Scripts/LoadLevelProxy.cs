using System;
using System.Collections;
using UnityEngine;

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
        }

        [UnityMessage]
        public void OnLevelWasLoaded(int levelId)
        {
            if (levelId != 1 || String.IsNullOrEmpty(LevelName))
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