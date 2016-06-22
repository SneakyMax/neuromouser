using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets._Scripts.GameObjects;
using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts.AI
{
    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    public class Patrolling : CatAIState
    {
        public const float ReachedPatrolPointThreshold = 0.2f;

        private PatrolPoint startPatrolPoint;

        private PatrolPoint previousPatrolPoint;

        private PatrolPoint nextPatrolPoint;
        private Vector2 nextPatrolPointPosition;

        private Queue<GridPosition> path;

        private bool isPatrollingForward;

        private Vector2 desiredVelocity;

        public Patrolling()
        {
            isPatrollingForward = true;
        }

        public void SetStartPatrolNode(PatrolPoint startPatrolPoint)
        {
            this.startPatrolPoint = startPatrolPoint;
            SetNextPatrolPoint(startPatrolPoint);
        }

        public override void Enter()
        {
            if (startPatrolPoint == null)
                throw new InvalidOperationException("Missing a start patrol point.");

            GetNewPath();
        }

        public override void Update()
        {
            var currentPosition = Cat.transform.position;

            // If reached the patrol point, set the next patrol point (then wait a frame).
            if (currentPosition.DistanceTo(nextPatrolPointPosition) < ReachedPatrolPointThreshold)
            {
                SetToStop();
                ReachedPatrolPoint();
                return;
            }

            if (path == null)
                return; // A bug, see errors

            // If reached a path waypoint, set the next path waypoint (then wait a frame).
            var nextPathPosition = PlacementGrid.Instance.GetWorldPosition(path.Peek());

            if (currentPosition.DistanceTo(nextPathPosition) < ReachedPatrolPointThreshold)
            {
                SetToStop();
                path.Dequeue();
                return; //todo?
            }

            // Set the desired velocity towards the next path waypoint.
            var direction = currentPosition.UnitVectorTo(nextPathPosition);

            desiredVelocity = direction * Cat.PatrolSpeed;

            var seePlayer = CatAI.CheckFieldOfViewForMouse();
            if (seePlayer != null)
            {
                CatAI.GetState<ChasingRunner>().SetRunner(seePlayer);
                CatAI.SetState<ChasingRunner>();
            }
        }

        public override void FixedUpdate()
        {
            Cat.Move(desiredVelocity);
        }

        private void SetToStop()
        {
            desiredVelocity = new Vector2();
        }

        private void ReachedPatrolPoint()
        {
            var nextNextPatrolPoint = isPatrollingForward ? nextPatrolPoint.NextPoint : nextPatrolPoint.PreviousPoint;

            if (nextNextPatrolPoint == null) // Turn around
            {
                isPatrollingForward = !isPatrollingForward;
                nextNextPatrolPoint = isPatrollingForward ? nextPatrolPoint.NextPoint : nextPatrolPoint.PreviousPoint;
            }

            SetNextPatrolPoint(nextNextPatrolPoint);

            GetNewPath();
        }

        private void GetNewPath()
        {
            if (previousPatrolPoint == null)
                ReachedPatrolPoint(); // Hack?

            path = new Queue<GridPosition>(Pathfinding.Instance.GetPath(previousPatrolPoint.Position, nextPatrolPoint.Position));
        }

        private void SetNextPatrolPoint(PatrolPoint point)
        {
            previousPatrolPoint = nextPatrolPoint;
            nextPatrolPoint = point;

            nextPatrolPointPosition = PlacementGrid.Instance.GetWorldPosition(point.Position);
        }
    }
}