using System;
using System.Collections.Generic;
using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts.AI
{
    public class ChasingRunner : CatAIState
    {
        private Queue<GridPosition> pathToLastRunnerPosition;

        private Vector3 lastRunnerPosition;

		private RunnerPlayer player;

        public override void Enter()
        {
            FMODUnity.RuntimeManager.PlayOneShot(UnityEngine.Random.value > 0.5f ? Cat.SoundGrowlEventName : Cat.SoundHissEventName, Cat.transform.position);
        }

        public override void Update()
        {
            if (player == null)
                throw new InvalidOperationException("Call SetRunner before transitioning to ChasingRunner.");

            var currentPlayer = AI.CheckFieldOfViewForMouse();

            if (currentPlayer != null)
            {
                lastRunnerPosition = currentPlayer.transform.position;

                var directionToPlayer = Cat.transform.position.UnitVectorTo(currentPlayer.transform.position);
                DesiredVelocity = directionToPlayer * Cat.ChaseSpeed;

                pathToLastRunnerPosition = null;
            }
            else
            {
                if (pathToLastRunnerPosition == null)
                {
                    Debug.Log("Lost Runner");
                }
                LostPlayer();
            }
        }

        private void LostPlayer()
        {
            if (pathToLastRunnerPosition == null || pathToLastRunnerPosition.Count == 0)
                GetPathToLastRunnerPosition();

            if (pathToLastRunnerPosition == null || pathToLastRunnerPosition.Count == 0)
            {
                ReturnToDefaultState();
                return;
            }
            
            GoToLastRunnerPosition();
        }

        private void GetPathToLastRunnerPosition()
        {
            pathToLastRunnerPosition = GetPathTo(PlacementGrid.Instance.GetClosestGridPosition(lastRunnerPosition));
            if (pathToLastRunnerPosition == null || pathToLastRunnerPosition.Count == 0)
            {
                ReturnToDefaultState();
            }
        }

        private void GoToLastRunnerPosition()
        {
            MoveAlongPath(pathToLastRunnerPosition, Cat.ChaseSpeed);

            if (Cat.transform.position.DistanceTo(lastRunnerPosition) < CatAI.ReachedPositionThreshold)
                ReturnToDefaultState();
        }

        public override void Exit()
        {
            player = null;
            pathToLastRunnerPosition = null;
            lastRunnerPosition = Vector3.zero;
        }

        private void ReturnToDefaultState()
        {
            if (AI.IsPatroller)
                AI.SetState<Patrolling>();
            else
                AI.SetState<Idle>();
        }

        public void SetRunner(RunnerPlayer player)
        {
            this.player = player;
        }
    }
}