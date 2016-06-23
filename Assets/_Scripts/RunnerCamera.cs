using System;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class RunnerCamera : MonoBehaviour
    {
        public static RunnerCamera Instance { get; private set; }

        [AssignedInUnity]
        public float[] ZoomLevelFactors;

        [AssignedInUnity, Range(0, 10)]
        public float ZoomChangeRate;

        private float desiredOrthographicSize;

        private new Camera camera;
        private float startOrthographicSize;

        [UnityMessage]
        public void Awake()
        {
            Instance = this;

            if (ZoomLevelFactors.Length != 4)
                throw new InvalidOperationException("Need 4 zoom levels.");

            camera = GetComponent<Camera>();
            startOrthographicSize = camera.orthographicSize;
        }

        [UnityMessage]
        public void Start()
        {
            GameStateController.Instance.GameStarted += OnGameStarted;
            LevelLoader.Instance.LevelUnloading += DetachCamera;
        }

        private void OnGameStarted()
        {
            HackerInterface.Instance.OnCameraPowerChanged += CameraPowerLevelChanged;
            CameraPowerLevelChanged(0);
        }

        private void CameraPowerLevelChanged(int terminalPower)
        {
            desiredOrthographicSize = ZoomLevelFactors[terminalPower] * startOrthographicSize;
        }

        [UnityMessage]
        public void Update()
        {
            if (Math.Abs(desiredOrthographicSize) < 0.001f)
                return;

            var currentSize = camera.orthographicSize;

            if (Math.Abs(currentSize - desiredOrthographicSize) > 0.001f)
            {
                if (desiredOrthographicSize < currentSize)
                {
                    camera.orthographicSize = currentSize - Mathf.Min(ZoomChangeRate * Time.deltaTime, Mathf.Abs(currentSize - desiredOrthographicSize));
                }
                else
                {
                    camera.orthographicSize = currentSize + Mathf.Min(ZoomChangeRate * Time.deltaTime, Mathf.Abs(currentSize - desiredOrthographicSize));
                }
            }
        }

        private void DetachCamera()
        {
            transform.SetParent(null, false);
        }

        public void Attach(GameObject playerInstance)
        {
            transform.SetParent(playerInstance.transform, false);

            playerInstance.GetComponentInChildren<Canvas>().worldCamera = GetComponent<Camera>();
        }
    }
}