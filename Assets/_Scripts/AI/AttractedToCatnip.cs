using System;
using System.Collections.Generic;
using Assets._Scripts.LevelEditor;

namespace Assets._Scripts.AI
{
    public class AttractedToCatnip : CatAIState
    {
        private CatnipDebuff currentDebuff;
        private Queue<GridPosition> currentPath; 

        public void TryAttractToDebuff(CatnipDebuff catnipDebuff)
        {
            if (AI.CurrentState == this)
            {
                RecalculatePath();
            }

            if (AI.CurrentState is ChasingRunner)
                return;

            currentDebuff = catnipDebuff;

            var pathToDebuff = GetPathToDebuff();
            if (pathToDebuff == null)
                return; // No path

            AI.SetState<AttractedToCatnip>();
        }

        public void CancelAttraction()
        {
            if(AI.CurrentState == this)
                AI.ReturnToDefaultState();
        }

        private void RecalculatePath()
        {
            currentPath = GetPathToDebuff();
        }

        private Queue<GridPosition> GetPathToDebuff()
        {
            var targetPosition = PlacementGrid.Instance.GetClosestGridPosition(currentDebuff.transform.position);

            return GetPathTo(targetPosition);
        } 

        public override void Enter()
        {
            if (currentDebuff == null)
                throw new InvalidOperationException("Call from TryAttractToDebuff");

            RecalculatePath();
        }

        public override void Exit()
        {
            currentDebuff = null;
            currentPath = null;
        }

        public override void Update()
        {
            if (currentDebuff == null || currentPath == null || currentPath.Count == 0)
            {
                AI.ReturnToDefaultState();
                return;
            }

            DesiredVelocity = MoveAlongPath(currentPath, Cat.PatrolSpeed);

            var possibleMouse = AI.CheckFieldOfViewForMouse();

            if (possibleMouse != null)
            {
                AI.GetState<ChasingRunner>().SetRunner(possibleMouse);
                AI.SetState<ChasingRunner>();
            }
        }
    }
}