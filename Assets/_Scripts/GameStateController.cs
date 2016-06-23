using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets._Scripts.LevelEditor;

namespace Assets._Scripts
{
    [UnityComponent]
    public class GameStateController : MonoBehaviour
    {
        /// <summary>When a game/match has been started or restarted (e.g. after dying).</summary>
        public event Action GameStarted;

        public event Action OnPlayerDied;

		public static GameStateController Instance { get; private set; }

		public string LoadedLevelName { get; private set; }

        [AssignedInUnity]
        public Image InterfaceFadeInCover;

        [AssignedInUnity]
        public Image ScreenFadeInCover;

		[AssignedInUnity]
		public float SecondsBeforeLevelEnd = 300f;

		public String[] LevelList = null;

		public int [] LevelTimeList = null;

		public Sprite[] PreTransitionList = null;

		public TransitionScreen TransScreen = null;

		private bool showingEnterStory = false;

		private bool showingExitStory = false;

		private int currentLevelIndex = 0;

		private bool levelLoadRequested;

		private bool transitionShownThisLevel = false;

		/*[NonSerialized]
		public Sprite EnterSprite = null;

		[NonSerialized]
		public Sprite ExitSprite = null;*/

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

			if ( TransScreen != null )
			{
				TransScreen.TransitionReturn += OnTransitionReturn;
			}
        }

        private IEnumerator KickBackToTitleScreenIfNoLevelLoaded()
        {
            yield return new WaitForSeconds(0.5f);
            if (levelLoadRequested == false)
            {
                GoToTitleScreen();
            }
        }

		private IEnumerator LoadLevelDelay()
		{
			if ((TransScreen != null) && (PreTransitionList != null) &&
				(currentLevelIndex < PreTransitionList.Length))
			{
				TransScreen.ShowTransition(PreTransitionList[currentLevelIndex]);
			}

			LevelLoader.Instance.Reset();
			while (!transitionShownThisLevel)
			{
				yield return null;
			}

			levelLoadRequested = true;

			LevelLoader.Instance.LoadLevel(Application.dataPath + "/levels/" + LevelList[currentLevelIndex],
				false);

			if ((LevelTimeList != null) && (currentLevelIndex < LevelTimeList.Length))
				SecondsBeforeLevelEnd = LevelTimeList[currentLevelIndex];
		}

		public void LoadLevel(string levelName)
        {
			CoverScreen();
			LoadedLevelName = levelName;
			levelLoadRequested = true;

			if (levelName == "**new game**" )
			{
				if ( ( currentLevelIndex > -1 ) && (LevelList != null) && (currentLevelIndex < LevelList.Length))
				{
					StartCoroutine(LoadLevelDelay());
					return;
				}
				else
				{
					GoToTitleScreen();
					return;
				}
			}
			transitionShownThisLevel = true;
			LevelLoader.Instance.LoadLevel( levelName );
        }

		public void PlayerGotToExit()
		{
			if ((LoadedLevelName == "**new game**") &&  (LevelList != null ))
			{
				if ( ++currentLevelIndex < LevelList.Length )
				{
					transitionShownThisLevel = false;
					LoadLevel(LoadedLevelName);
				}
				else
				{
					GoToTitleScreen();
				}
			}
			else
			{
				GoToTitleScreen();
			}
		}

        public void PlayerDied()
        {
            if (OnPlayerDied != null)
                OnPlayerDied();

            RestartLevel();
        }

        public void RestartLevel()
        {
			LoadLevel(LoadedLevelName);
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
			Instance.currentLevelIndex = 0;
			Instance.LevelList = null;
            SceneManager.LoadScene(0);
        }

		public void OnTransitionReturn()
		{
			transitionShownThisLevel = true;
		}

		/*private Sprite LoadSpriteFromPath(string pathname, float width, float height)
		{
			try
			{
				byte[] rawEnterLevelImage = File.ReadAllBytes(pathname);
				Texture2D enterLevelImage = new Texture2D(Mathf.FloorToInt(width), Mathf.FloorToInt(height),
					TextureFormat.ARGB32, false);
				enterLevelImage.LoadImage(rawEnterLevelImage);
				enterLevelImage.name = pathname;

				return Sprite.Create(enterLevelImage, new Rect(0f,0f,width,height), new Vector2(0.5f, 0.5f));
			}
			catch (IOException)
			{
				// Couldn't load sprite
				return null;
			}
		}*/
    }
}