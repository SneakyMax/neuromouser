using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    public class PlacedWall : IPlacedObject
    {
        public string Type { get { return "Wall"; } }

        public PlacedWall(GameObject levelEditorObject)
        {
            
        }

        public string Serialize()
        {
            throw new System.NotImplementedException();
        }
    }
}