using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class PlacedWall : MonoBehaviour, IPlacedObject
    {
        public GameObject UnityObject { get { return gameObject; } }

        public string Type { get { return "Wall"; } }
        
        public string Serialize()
        {
            return "";
        }

        public void Deserialize(string serialized)
        {
            
        }

        public void Destroy()
        {
            Destroy(UnityObject);
        }
    }
}