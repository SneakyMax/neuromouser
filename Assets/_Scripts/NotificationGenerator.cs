using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts
{
    [UnityComponent]
    public class NotificationGenerator : MonoBehaviour
    {
        [AssignedInUnity]
        public GameObject TextPrefab;

        [AssignedInUnity]
        public string[] CameraMessages;

        [AssignedInUnity]
        public string[] DoorMessages;

        [AssignedInUnity]
        public string[] TrapMessages;

        [AssignedInUnity]
        public string[] CatMessages;

        [UnityMessage]
        public void Start()
        {
            if (CameraMessages.Length < 4 || DoorMessages.Length < 4 || TrapMessages.Length < 4 || CatMessages.Length < 4)
                throw new InvalidOperationException("Missing messages.");

            GameStateController.Instance.GameStarted += InstanceOnGameStarted;
        }

        private void InstanceOnGameStarted()
        {
            HackerInterface.Instance.OnCameraPowerChanged += CameraPowerChanged;
            HackerInterface.Instance.OnDoorPowerChanged += DoorPowerChanged;
            HackerInterface.Instance.OnTrapPowerChanged += TrapPowerChanged;
            HackerInterface.Instance.OnCatPowerChanged += CatPowerChanged;
        }

        private void CreateText(string text)
        {
            var instance = Instantiate(TextPrefab);
            instance.transform.SetParent(transform, false);

            instance.GetComponent<Text>().text = text;
        }

        private void CameraPowerChanged(int terminalPower)
        {
            CreateText(CameraMessages[terminalPower]);
        }

        private void DoorPowerChanged(int terminalPower)
        {
            CreateText(DoorMessages[terminalPower]);
        }

        private void TrapPowerChanged(int terminalPower)
        {
            CreateText(TrapMessages[terminalPower]);
        }

        private void CatPowerChanged(int terminalPower)
        {
            CreateText(CatMessages[terminalPower]);
        }
    }
}