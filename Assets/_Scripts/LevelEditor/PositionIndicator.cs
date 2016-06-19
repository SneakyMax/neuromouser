using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class PositionIndicator : MonoBehaviour
    {
        private Vector2 lastPosition;

        private Text text;

        [UnityMessage]
        public void Start()
        {
            text = GetComponent<Text>();
        }

        [UnityMessage]
        public void Update()
        {
            var currentPosition = EditorCursor.Instance.HoldingTool == null ? new Vector2() : EditorCursor.Instance.HoldingTool.CurrentPosition;

            if (currentPosition != lastPosition)
            {
                var gridPosition = PlacementGrid.Instance.GetGridPosition(currentPosition);
                text.text = String.Format("({0:0.0}, {1:0.0}) world ({2}, {3}) grid", currentPosition.x, currentPosition.y, gridPosition.X, gridPosition.Y);
            }

            lastPosition = currentPosition;
        } 
    }
}