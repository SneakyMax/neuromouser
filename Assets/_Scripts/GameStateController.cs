﻿using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets._Scripts
{
    [UnityComponent]
    public class GameStateController : MonoBehaviour
    {
        /// <summary>When a game/match has been started or restarted (e.g. after dying).</summary>
        public event Action GameStarted;

        public event Action OnPlayerDied;

        [EventRef]
        public string MusicTrackOne = "event:/Track_1";

        [EventRef]
        public string MusicTrackThree = "event:/Track_3";

        public EventInstance MusicTrackOneInstance;

        public EventInstance MusicTrackThreeInstance;

		public static GameStateController Instance { get; private set; }

		public string LoadedLevelName { get; private set; }

        [AssignedInUnity]
        public Image InterfaceFadeInCover;

        [AssignedInUnity]
        public Image ScreenFadeInCover;

		[AssignedInUnity]
		public float SecondsBeforeLevelEnd = 300f;

        [AssignedInUnity]
        public string[] LevelList;

        [AssignedInUnity]
        public int [] LevelTimeList;

        [AssignedInUnity]
        public Sprite[] PreTransitionList;

        [AssignedInUnity]
		public TransitionScreen TransitionScreen;

        private int currentLevelIndex;

		private bool levelLoadRequested;

		private bool transitionShownThisLevel;

        private int? currentPlayingMusic;

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        [UnityMessage]
        public void Start()
        {
            MusicTrackOneInstance = RuntimeManager.CreateInstance(MusicTrackOne);
            MusicTrackThreeInstance = RuntimeManager.CreateInstance(MusicTrackThree);
            
            LevelLoader.Instance.LevelLoaded += LevelLoaded;

            StartCoroutine(KickBackToTitleScreenIfNoLevelLoaded());

			if (TransitionScreen != null)
			{
				TransitionScreen.TransitionReturn += OnTransitionReturn;
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
			if (TransitionScreen != null && PreTransitionList != null && currentLevelIndex < PreTransitionList.Length)
			{
				TransitionScreen.ShowTransition(PreTransitionList[currentLevelIndex]);
			}

			LevelLoader.Instance.Reset();
			while (!transitionShownThisLevel)
			{
				yield return null;
			}

			levelLoadRequested = true;

            StartMusicIfNotPlaying();

		    var fullFilePath = Path.Combine(Application.streamingAssetsPath, "Levels/" + LevelList[currentLevelIndex]);

            LevelLoader.Instance.LoadLevel(fullFilePath, false);

			if (LevelTimeList != null && currentLevelIndex < LevelTimeList.Length)
				SecondsBeforeLevelEnd = LevelTimeList[currentLevelIndex];
		}

		public void LoadLevel(string levelName)
        {
			CoverScreen();
			LoadedLevelName = levelName;
			levelLoadRequested = true;

			if (levelName == "**new game**" )
			{
			    if (currentLevelIndex < 0 || LevelList == null || currentLevelIndex >= LevelList.Length)
			    {
			        GoToTitleScreen();
			        return;
			    }

			    StartCoroutine(LoadLevelDelay());
			    return;
			}

            StartMusicIfNotPlaying();

            transitionShownThisLevel = true;
			LevelLoader.Instance.LoadLevel(levelName);
        }

        public void StartMusicIfNotPlaying()
        {
            if (currentPlayingMusic != null)
                return;

            if (Random.value > 0.5f)
            {
                MusicTrackOneInstance.start();
                currentPlayingMusic = 1;
            }
            else
            {
                MusicTrackThreeInstance.start();
                currentPlayingMusic = 3;
            }
        }

        public void StopMusic()
        {
            if (currentPlayingMusic == 1)
            {
                MusicTrackOneInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }
            else if (currentPlayingMusic == 3)
            {
                MusicTrackThreeInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }

            currentPlayingMusic = null;
        }

		public void PlayerGotToExit()
		{
		    StopMusic();

			if (LoadedLevelName == "**new game**" && LevelList != null)
			{
			    currentLevelIndex++;
				if (currentLevelIndex < LevelList.Length)
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

        private void GoToTitleScreen()
        {
            StopMusic();
			Instance.currentLevelIndex = 0;
			Instance.LevelList = null;
            SceneManager.LoadScene(0);
        }

		public void OnTransitionReturn()
		{
			transitionShownThisLevel = true;
		}
    }
}