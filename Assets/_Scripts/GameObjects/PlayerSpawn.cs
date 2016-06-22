using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    public class PlayerSpawn : InGameObject
    {
        public override int Layer { get { return 4; } }

        [AssignedInUnity]
        public GameObject PlayerPrefab;

        [UnityMessage]
        public void Start()
        {
            var playerInstance = (GameObject)Instantiate(PlayerPrefab, transform.position, Quaternion.identity);
            playerInstance.transform.SetParent(LevelLoader.RunnerArea.transform);
            playerInstance.layer = LevelLoader.RunnerLayer;

            RunnerCamera.Instance.transform.SetParent(playerInstance.transform, false);
            
            playerInstance.GetComponentInChildren<Renderer>().sortingLayerName = "RunnerMain";

            LevelLoader.RegisterObject(playerInstance);
        }
    }
}