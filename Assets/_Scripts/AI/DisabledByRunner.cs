using System.Collections;
using UnityEngine;

namespace Assets._Scripts.AI
{
    public class DisabledByRunner : CatAIState
    {
        private float disableTime;

        private bool allowStateChange;

        private Coroutine enableAfterTime;

        public override bool AllowStateChangeFrom()
        {
            return allowStateChange;
        }

        public override void Enter()
        {
            allowStateChange = false;

            Cat.HideFieldOfViewMesh();

            enableAfterTime = Cat.StartCoroutine(EnableCatAfterTime());
        }

        public override void Exit()
        {
            allowStateChange = true;

            Cat.ShowFieldOfViewMesh();

            if (enableAfterTime != null)
                Cat.StopCoroutine(enableAfterTime);
        }

        public void EnableCat()
        {
            allowStateChange = true;
            AI.ReturnToDefaultState();
        }

        public IEnumerator EnableCatAfterTime()
        {
            yield return new WaitForSeconds(disableTime);
            EnableCat();
        }

        public void SetTime(float disableTime)
        {
            this.disableTime = disableTime;
        }
    }
}