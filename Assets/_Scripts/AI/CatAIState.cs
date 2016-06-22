﻿using System.Collections.Generic;
using Assets._Scripts.GameObjects;
using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts.AI
{
    public abstract class CatAIState
    {
        public Cat Cat {get { return CatAI.Cat; } }

        public CatAI CatAI { get; set; }

        public bool IsActive { get; set; }

        protected Vector2 DesiredVelocity { get; set; }

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
            var currentPosition = Cat.transform.position;
            var closestGridPosition = PlacementGrid.Instance.GetGridPosition(PlacementGrid.Instance.GetClosestSnappedPosition(currentPosition));

            return new Queue<GridPosition>(Pathfinding.Instance.GetPath(closestGridPosition, position));
        } 

        protected Vector3 MoveAlongPath(Queue<GridPosition> path, float speed)
        {
            var currentPosition = Cat.transform.position;

            var nextPathPosition = PlacementGrid.Instance.GetWorldPosition(path.Peek());

            if (currentPosition.DistanceTo(nextPathPosition) < CatAI.ReachedPositionThreshold)
            {
                path.Dequeue();
                return new Vector3();
            }

            var direction = currentPosition.UnitVectorTo(nextPathPosition);

            return direction * speed;
        }
    }
}