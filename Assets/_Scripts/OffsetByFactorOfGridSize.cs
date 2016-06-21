using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class OffsetByFactorOfGridSize : MonoBehaviour
    {
        [AssignedInUnity]
        public Vector2 Factor = new Vector2(0, 0.5f);

        [UnityMessage]
        public void Start()
        {
            transform.position = transform.position + new Vector3(PlacementGrid.Instance.GridSize * Factor.x, -PlacementGrid.Instance.GridSize * Factor.y);
        }
    }
}