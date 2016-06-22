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

		[NonSerialized]
		public Sprite EnterSprite = null;

		[NonSerialized]
		public Sprite ExitSprite = null;

		public String[] LevelList = null;

		private int currentLevelIndex = 0;

		private bool showingEnterStory = false;

		private bool showingExitStory = false;

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
			EnterSprite = LoadSpriteFromPath(Path.Combine(SaveButton.GetGameSaveDirectory(),
				"entryimage_" + levelName + ".png"), 1920f, 1080f);
			ExitSprite = LoadSpriteFromPath(Path.Combine(SaveButton.GetGameSaveDirectory(),
				"exitimage_" + levelName + ".png"), 1920f, 1080f);

			// TODO Remove this...for debugging only
			if ( EnterSprite != null )
			{
				print( "Entersprite found." );
			}
			if ( ExitSprite != null )
			{
				print( "Exitsprite found." );
			}
        }

		public void PlayerGotToExit()
		{
			if ( LevelList != null )
			{
				if ( ++currentLevelIndex < LevelList.Length )
				{
					LoadedLevelName = LevelList[currentLevelIndex];
					RestartLevel();
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
			Instance.currentLevelIndex = 0;
			Instance.LevelList = null;
            SceneManager.LoadScene(0);
        }

		private Sprite LoadSpriteFromPath(string pathname, float width, float height)
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
		}
    }
}