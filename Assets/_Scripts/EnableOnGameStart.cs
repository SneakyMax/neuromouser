using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class EnableOnGameStart : MonoBehaviour
    {
        [AssignedInUnity]
        public GameObject Target;

        [UnityMessage]
        public void Awake()
        {
            Target.SetActive(false);
        }

        [UnityMessage]
        public void Start()
        {
            GameStateController.Instance.GameStarted += GameStarted;
        }

        private void GameStarted()
        {
            Target.SetActive(true);
        }
    }
}