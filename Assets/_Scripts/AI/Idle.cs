using System;
using System.Collections.Generic;
using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts.AI
{
    public class Idle : CatAIState
    {
        private GridPosition startPosition;

        private Quaternion startRotation;

        private bool isReturningToIdlePosition;

        private Queue<GridPosition> pathToIdle; 

        public override void Init()
        {
            if (Cat.StartGridPosition == null)
                throw new InvalidOperationException("Cat didn't start on a grid position.");

            startPosition = Cat.StartGridPosition.Value;
            startRotation = Quaternion.AngleAxis(Cat.StartRotation, Vector3.forward);
        }

        public override void Enter()
        {
            if (AI.PreviousState == null)
            {
                isReturningToIdlePosition = false;
                return;
            }

            ReturnToIdlePosition();
        }

        private void ReturnToIdlePosition()
        {
            isReturningToIdlePosition = true;

            pathToIdle = GetPathTo(startPosition);

            if (pathToIdle == null)
            {
                //help can't get back to idle position!
                isReturningToIdlePosition = false;
                pathToIdle = null;
            }
        }
        
        public override void Update()
        {
            var possibleMouse = AI.CheckFieldOfViewForMouse();

            if (possibleMouse != null)
            {
                AI.GetState<ChasingRunner>().SetRunner(possibleMouse);
                AI.SetState<ChasingRunner>();
                return;
            }

            if (isReturningToIdlePosition)
            {
                UpdateReturnToIdle();
            }
            else
            {
                Cat.Turn(startRotation);
            }
        }

        private void UpdateReturnToIdle()
        {
            var worldStartPosition = PlacementGrid.Instance.GetWorldPosition(startPosition);
            var currentPosition = Cat.transform.position;

            if (currentPosition.DistanceTo(worldStartPosition) < CatAI.ReachedPositionThreshold)
            {
                StopMoving();
                isReturningToIdlePosition = false;
                Cat.Turn(startRotation);
                return;
            }

            MoveAlongPath(pathToIdle, Cat.PatrolSpeed);
        }
    }
}