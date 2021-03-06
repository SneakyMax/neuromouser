﻿using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    public abstract class Tool : MonoBehaviour
    {
        public Vector2 PositionOffset;

        public EditorCursor Cursor { get; set; }

        public abstract bool ShouldSnapToGrid { get; }

        public Vector2 CurrentPosition { get; private set; }

        [UnityMessage]
        public void Start()
        {
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.sortingOrder = 9999;
            }

            ToolStart();
        }

        protected virtual void ToolStart()
        {
            
        }

        [UnityMessage]
        public void Update()
        {
            var cursorPosition = EditorCursor.GetWorldPosition();
            var desiredPosition = cursorPosition + PositionOffset;

            var snappedPosition = PlacementGrid.Instance.GetClosestSnappedPosition(desiredPosition);

            var finalPosition = ShouldSnapToGrid ? snappedPosition : desiredPosition;
            
            transform.position = finalPosition;
            CurrentPosition = finalPosition;
        }

        public abstract void ActivateTool(Vector2 position);

        protected virtual void PostActivateTool(IPlacedObject placedObject)
        {
            
        }

        public abstract void SecondaryActivateTool(Vector2 position);

        public virtual void KeyPressed(KeyCode key)
        {
            
        }
    }
}