using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class TrapAlarm : PlacedObject
    {
        private static readonly int[] layers = { 2 };
        public override int[] Layers { get { return layers; } }
    }
}