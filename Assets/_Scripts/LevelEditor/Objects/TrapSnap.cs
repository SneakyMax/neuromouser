using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class TrapSnap : PlacedObject
    {
        private static readonly int[] layers = { 1 };
        public override int[] Layers { get { return layers; } }
    }
}