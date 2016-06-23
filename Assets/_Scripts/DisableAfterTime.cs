using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class DisableAfterTime : MonoBehaviour
    {
        [AssignedInUnity]
        public float Time;

        [AssignedInUnity]
        public bool StartWhenEnabled;

        private Coroutine disableCoroutine;

        [UnityMessage]
        public void OnEnable()
        {
            if (StartWhenEnabled)
                DisableAfterDelay();
        }

        public void CancelDisable()
        {
            StopCoroutine(disableCoroutine);
        }

        public void DisableAfterDelay()
        {
            disableCoroutine = StartCoroutine(DisableAfterTimeCoroutine());
        }

        public void DoDisable()
        {
            if (disableCoroutine != null)
                StopCoroutine(disableCoroutine);

            gameObject.SetActive(false);
        }

        private IEnumerator DisableAfterTimeCoroutine()
        {
            yield return new WaitForSeconds(Time);
            DoDisable();
        }
    }
}