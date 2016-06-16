using System;

namespace Assets._Scripts.LevelEditor.Objects
{
    public class PlacedFloor : IPlacedObject
    {
        public string Type { get { return "Floor"; } }

        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}