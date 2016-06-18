using System.IO;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class EditorCamera : MonoBehaviour
    {
        [AssignedInUnity]
        public Material LineMaterial;

        [UnityMessage]
        public void OnPostRender()
        {
            var halfSize = new Vector2(
                Camera.main.orthographicSize * Camera.main.aspect,
                Camera.main.orthographicSize);

            var leftBounds = (Vector2)Camera.main.transform.position - halfSize;
            var rightBounds = (Vector2)Camera.main.transform.position + halfSize;

            var gridSize = PlacementGrid.Instance.GridSize;
            var offset = gridSize / 2f;

            var start = new Vector2(Mathf.Floor(leftBounds.x / gridSize) * gridSize - offset, Mathf.Floor(leftBounds.y / gridSize) * gridSize - offset);
            var end = new Vector2(Mathf.Ceil(rightBounds.x / gridSize) * gridSize + offset, Mathf.Ceil(rightBounds.y / gridSize + offset) * gridSize + offset);

            for (var x = start.x; x < end.x; x += gridSize)
            {
                GL.Begin(GL.LINES);
                LineMaterial.SetPass(0);
                GL.Color(LineMaterial.color);

                GL.Vertex3(x, start.y, 0);
                GL.Vertex3(x, end.y, 0);
                GL.End();
            }

            for (var y = start.y; y < end.y; y += gridSize)
            {
                GL.Begin(GL.LINES);
                LineMaterial.SetPass(0);
                GL.Color(LineMaterial.color);

                GL.Vertex3(start.x, y, 0);
                GL.Vertex3(end.x, y, 0);
                GL.End();
            }
        } 
    }
}