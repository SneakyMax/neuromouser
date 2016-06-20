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
    }
}