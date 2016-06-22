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

        private SpriteRenderer topLedSprite;
        private SpriteRenderer bottomLedSprite;
        private PowerLevelIndicator powerLevelIndicator;

        [UnityMessage]
        public void Start()
        {
            currentPowerLevel = 0;

            powerLevelIndicator = ParentTerminal.PowerReader.GetComponent<PowerLevelIndicator>();

            BelowShot.localPosition = powerLevelIndicator.GetLightPosition(1);

            topLedSprite = GetComponent<SpriteRenderer>();
            bottomLedSprite = BelowShot.GetComponent<SpriteRenderer>();

            bottomLedSprite.color = bottomLedSprite.color.WithAlpha(0);

            GameStateController.Instance.OnPlayerDied += OnPlayerDied;
        }

        private void OnPlayerDied()
        {
            GameStateController.Instance.OnPlayerDied -= OnPlayerDied;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (this != null)
                Destroy(gameObject);
        }

        [UnityMessage]
        public void Update()
        {
            currentPowerLevel += Speed * Time.deltaTime;

            var topPowerLevel = Mathf.CeilToInt(currentPowerLevel);
            var indicatorPosition = powerLevelIndicator.GetLightPosition(topPowerLevel);

            transform.position = indicatorPosition;
            BelowShot.position = powerLevelIndicator.GetLightPosition(topPowerLevel - 1);

            var fraction = currentPowerLevel - Mathf.Floor(currentPowerLevel);

            topLedSprite.color = topLedSprite.color.WithAlpha(fraction);
            bottomLedSprite.color = bottomLedSprite.color.WithAlpha(1.0f - fraction);

            if (currentPowerLevel > ParentTerminal.PowerReader.CurrentPower)
            {
                ParentTerminal.PowerReader.AbsorbPower(PowerValue);
                Destroy(gameObject);
            }
        }
    }
}
