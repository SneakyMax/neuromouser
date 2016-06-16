using System;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    public class PlacedFloor : IPlacedObject
    {
        public GameObject UnityObject { get; private set; }

        public string Type { get { return "Floor"; } }

        public PlacedFloor(GameObject levelEditorObject)
        {
            UnityObject = levelEditorObject;
        }

        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}