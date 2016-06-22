﻿using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [UnityComponent]
    public abstract class InGameObject : MonoBehaviour, IInGameObject
    {
        public LevelLoader LevelLoader { get; set; }

        /// <summary>Drawing layer 0 = floor, 1 = on floor, 2 = mid-level, 3 = ceiling</summary>
        public abstract int Layer { get; }

        public GridPosition? StartGridPosition { get; set; }

        public virtual bool IsDynamic { get { return false; } }

        public int Id { get; set; }

        protected SpriteRenderer SpriteRenderer { get; set; }

        public virtual void Deserialize(string serialized)
        {

        }

        public void Initialize()
        {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (SpriteRenderer != null)
            {
                SpriteRenderer.sortingOrder = GetSortPosition(transform.position, Layer);
            }
        }

        /// <summary>Call this in Update if an object moves.</summary>
        protected void SortObjectThatMoves()
        {
            if (SpriteRenderer != null)
            {
                var bottomOfSpritePosition = SpriteRenderer.bounds.min;
                SpriteRenderer.sortingOrder = GetSortPosition(bottomOfSpritePosition, Layer);
            }
        }

        public static int GetSortPosition(Vector3 position, int layer)
        {
            var closestGridPosition = PlacementGrid.Instance.GetClosestSnappedPosition(position);
            var sortPosition = closestGridPosition.y * 100 - layer;
            return -Mathf.RoundToInt(sortPosition);
        }

        public virtual void PostAllDeserialized()
        {
            
        }

        public virtual void GameStart()
        {
            
        }

        public virtual bool IsTraversableAt(GridPosition position)
        {
            return false;
        }

        [UnityMessage]
        public void OnDestroy()
        {
            Cleanup();
        }

        /// <summary>Clean up an object before it gets destroyed.</summary>
        protected virtual void Cleanup()
        {
            
        }
    }
}