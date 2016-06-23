using System.Collections.Generic;
using Assets._Scripts.GameObjects;
using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts.AI
{
    public abstract class CatAIState
    {
        public Cat Cat {get { return AI.Cat; } }

        public CatAI AI { get; set; }

        public bool IsActive { get; set; }

        protected Vector2 DesiredVelocity { get; set; }

        public GridPosition ClosestGridPosition { get { return PlacementGrid.Instance.GetGridPosition(PlacementGrid.Instance.GetClosestSnappedPosition(Cat.transform.position)); } }

        public virtual void Init()
        {
            
        }

        public virtual void Enter()
        {
            
        }

        public virtual void Exit()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual bool AllowStateChangeFrom()
        {
            return true;
        }

        public virtual void FixedUpdate()
        {
            Cat.Move(DesiredVelocity);
        }

        protected void StopMoving()
        {
            DesiredVelocity = new Vector2();
        }

        protected Queue<GridPosition> GetPathTo(GridPosition position)
        {
            return GetPath(ClosestGridPosition, position);
        }

        protected static Queue<GridPosition> GetPath(GridPosition from, GridPosition to)
        {
            var path = Pathfinding.Instance.GetPath(from, to);

            if (path == null)
                return new Queue<GridPosition>();

            return new Queue<GridPosition>(path);
        } 

        protected void MoveAlongPath(Queue<GridPosition> path, float speed)
        {
            if (path.Count == 0)
            {
                Debug.LogWarning("Path provided to MoveAlongPath is empty.");
                DesiredVelocity = Vector2.zero;
                return;
            }

            var currentPosition = Cat.transform.position;

            var nextPathPosition = PlacementGrid.Instance.GetWorldPosition(path.Peek());

            var distanceToNextPathNode = currentPosition.DistanceTo(nextPathPosition);

            if (distanceToNextPathNode < CatAI.ReachedPositionThreshold)
            {
                path.Dequeue();
                DesiredVelocity = Vector2.zero;
            }

            var direction = currentPosition.UnitVectorTo(nextPathPosition);

            DesiredVelocity = direction * Mathf.Min(speed, distanceToNextPathNode / Time.deltaTime);
        }
    }
}