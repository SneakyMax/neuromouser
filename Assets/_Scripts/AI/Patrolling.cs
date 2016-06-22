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
        private PatrolPoint startPatrolPoint;

        private PatrolPoint previousPatrolPoint;

        private PatrolPoint nextPatrolPoint;
        private Vector2 nextPatrolPointPosition;

        private Queue<GridPosition> path;

        private bool isPatrollingForward;

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

            path = GetPathTo(nextPatrolPoint.Position);
        }

        public override void Update()
        {
            var currentPosition = Cat.transform.position;

            // If reached the patrol point, set the next patrol point (then wait a frame).
            if (currentPosition.DistanceTo(nextPatrolPointPosition) < CatAI.ReachedPositionThreshold)
            {
                StopMoving();
                ReachedPatrolPoint();
                return;
            }

            if (path == null)
                return; // A bug, see errors
            
            DesiredVelocity = MoveAlongPath(path, Cat.PatrolSpeed);

            var seePlayer = CatAI.CheckFieldOfViewForMouse();
            if (seePlayer != null)
            {
                CatAI.GetState<ChasingRunner>().SetRunner(seePlayer);
                CatAI.SetState<ChasingRunner>();
            }
        }

        private void ReachedPatrolPoint()
        {
            var nextNextPatrolPoint = isPatrollingForward ? nextPatrolPoint.NextPoint : nextPatrolPoint.PreviousPoint;

            if (nextNextPatrolPoint == null) // Turn around
            {
                isPatrollingForward = !isPatrollingForward;
                nextNextPatrolPoint = isPatrollingForward ? nextPatrolPoint.NextPoint : nextPatrolPoint.PreviousPoint;
            }

            if (nextNextPatrolPoint == null)
            {
                // No patrol path! Bail out
                CatAI.SetState<Idle>();
                return;
            }

            SetNextPatrolPoint(nextNextPatrolPoint);

            GetPathFromPreviousPatrolPointToNewPatrolPoint();
        }

        private void GetPathFromPreviousPatrolPointToNewPatrolPoint()
        {
            if (previousPatrolPoint == null)
            {
                ReachedPatrolPoint(); // Hack?

                if (nextPatrolPoint == null)
                {
                    // No patrol path! Bail out
                    return;
                }
            }

            if (previousPatrolPoint == null)
                throw new InvalidOperationException("Previous patrol point wasn't loaded??");

            path = GetPath(previousPatrolPoint.Position, nextPatrolPoint.Position);

            if (path.Count == 0)
            {
                // No path!
                Debug.LogWarningFormat("Couldn't find path from {0} to {1}", previousPatrolPoint.Position, nextPatrolPoint.Position);
                CatAI.SetState<Idle>();
            }
        }

        private void SetNextPatrolPoint(PatrolPoint point)
        {
            if (point == null)
                throw new ArgumentNullException("point");

            previousPatrolPoint = nextPatrolPoint;
            nextPatrolPoint = point;

            nextPatrolPointPosition = PlacementGrid.Instance.GetWorldPosition(point.Position);
        }
    }
}