using System;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    public class Cat : PlacedObject, IHasRotation
    {
        private static readonly int[] layers = { 1, 2 }; 
        public override int[] Layers { get { return layers; } }

        public int RotationDegrees { get; set; }

        public override string Serialize()
        {
            return RotationDegrees.ToString();
        }

        public override void Deserialize(string serialized)
        {
            int rotation;
            try
            {
                rotation = Convert.ToInt32(serialized);
            }
            catch (FormatException)
            {
                rotation = 0;
            }

            RotationDegrees = rotation;

            Refresh();
        }

        public override void AfterPlace()
        {
            Refresh();
        }

        private void Refresh()
        {
            transform.rotation = Quaternion.AngleAxis(RotationDegrees, Vector3.forward);
        }
    }
}