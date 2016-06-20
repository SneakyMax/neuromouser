using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class CounterPulse : MonoBehaviour
    {
        public ICEHandler ParentHandler { get; set; }

        [AssignedInUnity]
        public float Speed;

        [AssignedInUnity]
        public float PowerDecayAmount = 1;

        public float PowerPosition { get; private set; }

        private PowerLevelIndicator powerLevelIndicator;

        [UnityMessage]
        public void Start()
        {
            PowerPosition = ParentHandler.MaxPower;
            powerLevelIndicator = ParentHandler.GetComponent<PowerLevelIndicator>();
        }

        [UnityMessage]
        public void Update()
        {
            PowerPosition -= Speed * Time.deltaTime;

            transform.position = powerLevelIndicator.GetClosestLightPosition(PowerPosition);

            if (PowerPosition <= ParentHandler.CurrentPower)
            {
                ParentHandler.AbsorbPower(-PowerDecayAmount);
                Destroy(gameObject);
            }
        }
    }
}