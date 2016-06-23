using System;
using UnityEngine;



namespace Assets._Scripts.AI
{
	


    public class ChasingRunner : CatAIState
    {
		[FMODUnity.EventRef]
		public string startGrowl = "event:/Cat_Growl";
		public string startHiss = "event:/Cat_Hiss";

		private RunnerPlayer player;
        private Vector3 desiredVelocity;

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

			if (UnityEngine.Random.value > 0.5f) 
			{
				FMODUnity.RuntimeManager.PlayOneShot (startGrowl, Cat.transform.position);
			} else 
			{
				FMODUnity.RuntimeManager.PlayOneShot (startHiss, Cat.transform.position);
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