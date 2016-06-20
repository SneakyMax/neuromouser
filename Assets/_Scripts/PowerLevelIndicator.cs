using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class PowerLevelIndicator : MonoBehaviour
    {
        [AssignedInUnity]
        public ICEHandler ICEHandler;

        [AssignedInUnity]
        public Transform StartPosition;
        
        public Vector3 CounterStartPosition { get; private set; }

        [AssignedInUnity]
        public float Separation;

        [AssignedInUnity]
        public GameObject IndicatorPrefab;

        private Stack<GameObject> indicatorLights;

        private int indicatedPower;

        [UnityMessage]
        public void Start()
        {
            indicatorLights = new Stack<GameObject>();

            CounterStartPosition = StartPosition.position + new Vector3(0, Separation * ICEHandler.MaxPower, 0);
        }

        [UnityMessage]
        public void Update()
        {
            var currentPower = Mathf.RoundToInt(ICEHandler.CurrentPower);
            
            while (indicatedPower < currentPower)
            {
                AddPowerLight();
                indicatedPower++;
            }
            while (indicatedPower > currentPower)
            {
                RemovePowerLight();
                indicatedPower--;
            }
        }

        private void AddPowerLight()
        {
            var newLight = Instantiate(IndicatorPrefab);
            newLight.transform.position = GetLightPosition(indicatedPower + 1);

            indicatorLights.Push(newLight);
        }

        public Vector3 GetLightPosition(int power)
        {
            power = Mathf.Clamp(power, 1, ICEHandler.MaxPower);
  
            var indicatorCount = 0;

            if (power >= ICEHandler.Level3PowerMin)
                indicatorCount = power + 3;

            else if (power >= ICEHandler.Level2PowerMin)
                indicatorCount = power + 2;

            else if (power >= ICEHandler.Level1PowerMin)
                indicatorCount = power + 1;

            else indicatorCount = power;

            return StartPosition.position + new Vector3(0, Separation * (indicatorCount - 1));
        }

        public Vector3 GetClosestLightPosition(float power)
        {
            return GetLightPosition(Mathf.RoundToInt(power));
        }

        private void RemovePowerLight()
        {
            var topLight = indicatorLights.Pop();

            if (topLight.GetComponent<SpriteRenderer>() == null)
            {
                topLight = indicatorLights.Pop(); // Dummy one, pop another
            }

            Destroy(topLight);
        }
    }
}