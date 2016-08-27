using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class DeactivateAfterTime : MonoBehaviour
    {
        [AssignedInUnity]
        public float Time;
        
        [UnityMessage]
        public void OnEnable()
        {
            StartCoroutine(DeactivateAfterTimeRoutine());
        }

        private IEnumerator DeactivateAfterTimeRoutine()
        {
            yield return new WaitForSeconds(Time);
            gameObject.SetActive(false);
        }
    }
}