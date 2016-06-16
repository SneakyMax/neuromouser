using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    public class PlacedWall : IPlacedObject
    {
        public GameObject UnityObject { get; private set; }

        public string Type { get { return "Wall"; } }

        public PlacedWall(GameObject levelEditorObject)
        {
            UnityObject = levelEditorObject;
        }

        public string Serialize()
        {
            throw new System.NotImplementedException();
        }
    }
}