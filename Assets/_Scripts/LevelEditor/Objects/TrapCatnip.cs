using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class TrapCatnip : PlacedObject
    {
        private static readonly int[] layers = { 2 };
        public override int[] Layers { get { return layers; } }
    }
}