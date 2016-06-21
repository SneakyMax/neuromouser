using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class PowerChargeIndicator : MonoBehaviour
    {
        [AssignedInUnity]
        public HackerPlayer HackerPlayer;

        private float startWidth;

        [UnityMessage]
        public void Start()
        {
            startWidth = ((RectTransform)transform).sizeDelta.x;
        }

        [UnityMessage]
        public void Update()
        {
            var rect = (RectTransform)transform;

            var chargePercent = HackerPlayer.PowerCharge / HackerPlayer.MaxPowerCharge;
            var width = chargePercent * startWidth;

            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        }
    }
}