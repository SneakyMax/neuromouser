using System;
using UnityEngine;

namespace Assets._Scripts.AI
{
    public class ChasingRunner : CatAIState
    {
		private RunnerPlayer player;
        private Vector3 desiredVelocity;

        public override void Enter()
        {
            FMODUnity.RuntimeManager.PlayOneShot(UnityEngine.Random.value > 0.5f ? Cat.SoundGrowlEventName : Cat.SoundHissEventName, Cat.transform.position);
        }

        public override void Update()
        {
            if (player == null)
                throw new InvalidOperationException("Call SetRunner before transitioning to ChasingRunner.");

            var currentPlayer = AI.CheckFieldOfViewForMouse();

            if (currentPlayer == null)
            {
                ReturnToDefaultState();
                return;
            }
            
            var directionToPlayer = Cat.transform.position.UnitVectorTo(currentPlayer.transform.position);
            desiredVelocity = directionToPlayer * Cat.ChaseSpeed;
        }

        public override void Exit()
        {
            player = null;
        }

        private void ReturnToDefaultState()
        {
            if (AI.IsPatroller)
                AI.SetState<Patrolling>();
            else
                AI.SetState<Idle>();
        }

        public override void FixedUpdate()
        {
            if (desiredVelocity.IsZero())
                return;

            Cat.Move(desiredVelocity);
        }

        public void SetRunner(RunnerPlayer player)
        {
            this.player = player;
        }
    }
}