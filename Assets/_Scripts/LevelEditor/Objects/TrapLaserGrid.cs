using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class TrapLaserGrid : PlacedObject
    {
        private static readonly int[] layers = { 1, 2 }; //maybe?
        public override int[] Layers { get { return layers; } }
    }
}