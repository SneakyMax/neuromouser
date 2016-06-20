using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [UnityComponent]
    public class Floor : InGameObject
    {
        public override int Layer { get { return 0; } }
    }
}