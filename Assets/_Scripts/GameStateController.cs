using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Scripts
{
    [UnityComponent]
    public class GameStateController : MonoBehaviour
    {
        public static GameStateController Instance { get; private set; }

        private bool levelLoadRequested;

        public string LoadedLevelName { get; private set; }

        /// <summary>When a game/match has been started or restarted (e.g. after dying).</summary>
        public event Action GameStarted;

        public event Action OnPlayerDied;

        [AssignedInUnity]
        public Image InterfaceFadeInCover;

        [AssignedInUnity]
        public Image ScreenFadeInCover;

		[AssignedInUnity]
		public float SecondsBeforeLevelEnd = 300f;

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
            CoverScreen();
            levelLoadRequested = true;
            LoadedLevelName = levelName;
            LevelLoader.Instance.LoadLevel(levelName);
        }

        public void PlayerDied()
        {
            if (OnPlayerDied != null)
                OnPlayerDied();

            RestartLevel();
        }

        public void RestartLevel()
        {
            CoverScreen();
            LevelLoader.Instance.LoadLevel(LoadedLevelName);
        }

        private void LevelLoaded()
        {
            foreach (var obj in LevelLoader.Instance.AllInGameObjects)
            {
                obj.GameStart();
            }

            if (GameStarted != null)
                GameStarted();

			HackerInterface.Instance.LevelTimer.SecondsLeft = SecondsBeforeLevelEnd;
			HackerInterface.Instance.LevelTimer.TimerRunning = true;

            StartCoroutine(FadeSequence());
        }

        public void CoverScreen()
        {
            ScreenFadeInCover.color = ScreenFadeInCover.color.WithAlpha(1);
            InterfaceFadeInCover.color = InterfaceFadeInCover.color.WithAlpha(1);
        }

        private IEnumerator FadeSequence()
        {
            yield return null;

            InterfaceFadeInCover.DOFade(0, 1.0f);

            yield return new WaitForSeconds(1.0f);

            ScreenFadeInCover.DOFade(0, 0.5f);            
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