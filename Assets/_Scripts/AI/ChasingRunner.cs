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
        private GridPosition lastRunnerGridPosition;

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
                lastRunnerGridPosition = PlacementGrid.Instance.GetClosestGridPosition(lastRunnerPosition);

                var directionToPlayer = Cat.transform.position.UnitVectorTo(currentPlayer.transform.position);
                DesiredVelocity = directionToPlayer * Cat.ChaseSpeed;

                pathToLastRunnerPosition = null;
            }
            else
            {
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
            pathToLastRunnerPosition = GetPathTo(lastRunnerGridPosition);
            if (pathToLastRunnerPosition == null || pathToLastRunnerPosition.Count == 0)
            {
                ReturnToDefaultState();
            }
        }

        private void GoToLastRunnerPosition()
        {
            MoveAlongPath(pathToLastRunnerPosition, Cat.ChaseSpeed);

            var worldGridPosition = PlacementGrid.Instance.GetWorldPosition(lastRunnerGridPosition);

            if (Cat.transform.position.DistanceTo(worldGridPosition) < CatAI.ReachedPositionThreshold)
                ReturnToDefaultState();
        }

        public override void Exit()
        {
            player = null;
            pathToLastRunnerPosition = null;
            lastRunnerPosition = Vector3.zero;
            lastRunnerGridPosition = new GridPosition();
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