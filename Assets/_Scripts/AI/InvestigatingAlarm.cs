using System.Collections.Generic;
using Assets._Scripts.GameObjects;

namespace Assets._Scripts.AI
{
    public class InvestigatingAlarm : CatAIState
    {
        public const int ReachedAlarmThreshold = 1;

        private TrapAlarm currentAlarm;
        private Queue<GridPosition> currentPath;
        
        public void TryInvestigateAlarm(TrapAlarm alarm)
        {
            if (AI.CurrentState is ChasingRunner)
                return; // Already chasing, don't investigate alarm

            currentAlarm = alarm;

            AI.SetState<InvestigatingAlarm>();
        }

        public override void Enter()
        {
            currentPath = GetPathTo(currentAlarm.GridPosition);

            if (currentPath.Count == 0)
            {
                ReturnToPreviousState();
            }
        }

        public override void Exit()
        {
            currentPath = null;
            currentAlarm = null;
        }

        public override void Update()
        {
            // TODO collisions with other cats
            if (currentPath == null || currentPath.Count == 0)
            {
                ReturnToPreviousState();
                return;
            }

            var possibleMouse = AI.CheckFieldOfViewForMouse();

            if (possibleMouse != null)
            {
                AI.GetState<ChasingRunner>().SetRunner(possibleMouse);
                AI.SetState<ChasingRunner>();
                return;
            }

            if (Cat.transform.position.DistanceTo(currentAlarm.GridPosition.ToWorldPosition()) < ReachedAlarmThreshold)
            {
                ReturnToPreviousState();
                return;
            }

            MoveAlongPath(currentPath, Cat.ChaseSpeed);
        }

        private void ReturnToPreviousState()
        {
            AI.ReturnToDefaultState();
        }
    }
}