using UnityEngine;

namespace Assets._Scripts
{
    /// <summary>Hacker shot. Just holds the power value of a shot.</summary>
    [UnityComponent]
    public class HackerShot : MonoBehaviour
    {
        /// <summary>The power value to add to an ICE when hit.</summary>
        [AssignedInUnity]
        public float PowerValue = 1f;

        /// <summary>How fast the shot moves.</summary>
        [AssignedInUnity]
        public float Speed = 0.5f;

        [AssignedInUnity]
        public HackerTerminal ParentTerminal;

        [AssignedInUnity]
        public Transform BelowShot;

        private float currentPowerLevel;
        private float separation;
        private Vector3 startPosition;

        private SpriteRenderer topLedSprite;
        private SpriteRenderer bottomLedSprite;

        [UnityMessage]
        public void Start()
        {
            currentPowerLevel = 0;

            separation = ParentTerminal.PowerReader.GetComponent<PowerLevelIndicator>().Separation;
            startPosition = ParentTerminal.PowerReader.GetComponent<PowerLevelIndicator>().StartPosition.position;

            BelowShot.localPosition = new Vector3(0, -separation, 0);

            topLedSprite = GetComponent<SpriteRenderer>();
            bottomLedSprite = BelowShot.GetComponent<SpriteRenderer>();
        }

        [UnityMessage]
        public void Update()
        {
            currentPowerLevel += Speed * Time.deltaTime;

            var indicatorAmount = GetActualPowerIndicatorAmount(currentPowerLevel);

            transform.position = startPosition + new Vector3(0, separation * Mathf.CeilToInt(indicatorAmount));

            var fraction = currentPowerLevel - Mathf.Floor(currentPowerLevel);

            topLedSprite.color = new Color(topLedSprite.color.r, topLedSprite.color.g, topLedSprite.color.b, fraction);
            bottomLedSprite.color = new Color(bottomLedSprite.color.r, bottomLedSprite.color.g, bottomLedSprite.color.b, 1.0f - fraction);

            if (currentPowerLevel > ParentTerminal.PowerReader.CurrentPower)
            {
                ParentTerminal.PowerReader.AbsorbPower(PowerValue);
                Destroy(gameObject);
            }
        }

        private float GetActualPowerIndicatorAmount(float realPowerLevel)
        {
            if (realPowerLevel >= ParentTerminal.PowerReader.Level3PowerMin)
                return realPowerLevel + 3;

            if (realPowerLevel >= ParentTerminal.PowerReader.Level2PowerMin)
                return realPowerLevel + 2;

            if (realPowerLevel >= ParentTerminal.PowerReader.Level1PowerMin)
                return realPowerLevel + 1;

            return realPowerLevel;
        }
    }
}
