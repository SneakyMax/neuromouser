using System.Security.AccessControl;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class PlacementGrid : MonoBehaviour
    {
        public static PlacementGrid Instance { get; private set; }

        public Transform TopLeft;

        /// <summary>The number of grid units across, in <see cref="GridSize"/> increments.</summary>
        public int Width;
        
        public int Height;
        
        public float GridSize;
        
        public Vector2 StartPosition
        {
            get { return TopLeft == null ? new Vector3() : TopLeft.transform.position; }
        }
        
        public Vector2 EndPosition
        {
            get { return StartPosition + new Vector2(Width * GridSize, -(Height * GridSize)); }
        }

        public PlacementGrid()
        {
            Width = 1;
            Height = 1;
            GridSize = 1;
        }

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        [UnityMessage]
        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            var z = 0;
            var start = new Vector3(StartPosition.x, StartPosition.y, z) - new Vector3(GridSize / 2, -GridSize / 2);

            var width = Width * GridSize;
            var height = Height * GridSize;

            for (var x = 0f; Mathf.Abs(x - (Width + 1) * GridSize) > 0.001f; x += GridSize)
            {
                var startPos = start + new Vector3(x, 0);
                Gizmos.DrawLine(startPos, startPos + new Vector3(0, -height));
            }

            for (var y = 0f; Mathf.Abs(y - (Height + 1) * GridSize) > 0.001f; y += GridSize)
            {
                var startPos = start + new Vector3(0, -y);
                Gizmos.DrawLine(startPos, startPos + new Vector3(width, 0));
            }
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
