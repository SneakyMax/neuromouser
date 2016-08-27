using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts
{
    [UnityComponent]
    public class FlashUI : MonoBehaviour
    {
        [AssignedInUnity]
        public float OnTime = 1f;

        [AssignedInUnity]
        public float OffTime = 1f;

        [AssignedInUnity]
        public bool UseOnEnabled;

        private Graphic graphic;

        [UnityMessage]
        public void Awake()
        {
            graphic = GetComponent<Graphic>();
        }

        [UnityMessage]
        public void Start()
        {
            if (UseOnEnabled)
                return;

            StartFlashing();
        }

        [UnityMessage]
        public void OnEnable()
        {
            if (UseOnEnabled == false)
                return;

            StartFlashing();
        }

        private void StartFlashing()
        {
            StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {
            while (true)
            {
                graphic.enabled = true;
                yield return new WaitForSeconds(OnTime);
                graphic.enabled = false;
                yield return new WaitForSeconds(OffTime);
            }
        }
    }
}