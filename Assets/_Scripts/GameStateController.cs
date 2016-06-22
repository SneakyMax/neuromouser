using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Scripts
{
    [UnityComponent]
    public class GameStateController : MonoBehaviour
    {
        public static GameStateController Instance { get; private set; }

        private bool levelLoadRequested;

        public string LoadedLevelName { get; private set; }

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        [UnityMessage]
        public void Start()
        {
            LevelLoader.Instance.LevelLoaded += LevelLoaded;

            StartCoroutine(KickBackToTitleScreenIfNoLevelLoaded());
        }

        private IEnumerator KickBackToTitleScreenIfNoLevelLoaded()
        {
            yield return new WaitForSeconds(0.5f);
            if (levelLoadRequested == false)
            {
                GoToTitleScreen();
            }
        }

        public void LoadLevel(string levelName)
        {
            levelLoadRequested = true;
            LoadedLevelName = levelName;
            LevelLoader.Instance.LoadLevel(levelName);
        }

        public void PlayerDied()
        {
            RestartLevel();
        }

        public void RestartLevel()
        {
            LevelLoader.Instance.LoadLevel(LoadedLevelName);
        }

        private void LevelLoaded()
        {
            foreach (var obj in LevelLoader.Instance.AllInGameObjects)
            {
                obj.GameStart();
            }
        }

        [UnityMessage]
        public void Update()
        {
            if (Input.GetButtonDown("Escape"))
            {
                GoToTitleScreen();
            }
        }

        private static void GoToTitleScreen()
        {
            SceneManager.LoadScene(0);
        }
    }
}