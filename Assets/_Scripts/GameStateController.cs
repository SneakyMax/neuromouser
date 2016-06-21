using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class GameStateController : MonoBehaviour
    {
        [UnityMessage]
        public void Start()
        {
            LevelLoader.Instance.LevelLoaded += LevelLoaded;
        }

        private void LevelLoaded()
        {
            foreach (var obj in LevelLoader.Instance.AllInGameObjects)
            {
                obj.GameStart();
            }
        }
    }
}