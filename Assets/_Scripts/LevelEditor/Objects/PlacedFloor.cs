using System;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class PlacedFloor : MonoBehaviour, IPlacedObject
    {
        public GameObject UnityObject { get { return gameObject; } }

        public string Type { get { return "Floor"; } }

        public string Serialize()
        {
            return "";
        }

        public void Deserialize(string serialized)
        {
            
        }
    }
}