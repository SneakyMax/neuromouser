using System.Security.AccessControl;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class PlacementGrid : MonoBehaviour
    {
        public static PlacementGrid Instance { get; private set; }
        
        [AssignedInUnity]
        public float GridSize;

        [AssignedInUnity]
        public bool ShowGrid;

        public PlacementGrid()
        {
            GridSize = 1;
        }

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        /// <summary>Gets the world coordinates that are the closest snap to the grid for the world position input.</summary>
        public Vector2 GetClosestSnappedPosition(Vector2 position)
        {
            var x = position.x;
            var y = position.y;

            x /= GridSize;
            y /= GridSize;

            return new Vector2(Mathf.Round(x) * GridSize, Mathf.Round(y) * GridSize);
        }

        /// <summary>Gets grid coordinates for a world space position. Use snapped coordinates, see <see cref="GetClosestSnappedPosition"/>.</summary>
        public GridPosition GetGridPosition(Vector2 alignedWorldPosition)
        {
            return new GridPosition(Mathf.RoundToInt(alignedWorldPosition.x / GridSize), Mathf.RoundToInt(alignedWorldPosition.y / GridSize));
        }

        public Vector2 GetWorldPosition(int gridX, int gridY)
        {
            return new Vector2(gridX * GridSize, gridY * GridSize);
        }

        public Vector2 GetWorldPosition(GridPosition position)
        {
            return new Vector2(position.X * GridSize, position.Y * GridSize);
        }
    }
}
