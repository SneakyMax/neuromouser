using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class RunnerCamera : MonoBehaviour
    {
        public static RunnerCamera Instance { get; private set; }

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        [UnityMessage]
        public void Start()
        {
            LevelLoader.Instance.LevelUnloading += DetachCamera;
        }

        private void DetachCamera()
        {
            transform.SetParent(null, false);
        }
    }
}