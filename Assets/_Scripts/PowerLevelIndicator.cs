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
            if (indicatedPower + 1 == ICEHandler.Level1PowerMin || indicatedPower + 1 == ICEHandler.Level2PowerMin || indicatedPower + 1 == ICEHandler.Level3PowerMin)
            {
                var dummy = new GameObject();
                dummy.name = "Gap";
                dummy.transform.SetParent(StartPosition, false);
                dummy.transform.localPosition = new Vector3(0, Separation * indicatorLights.Count, 0);
                indicatorLights.Push(dummy);
            }

            var newLight = Instantiate(IndicatorPrefab);
            newLight.transform.SetParent(StartPosition, false);
            newLight.transform.localPosition = new Vector3(0, Separation * indicatorLights.Count, 0);

            indicatorLights.Push(newLight);
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