using UnityEngine;

namespace Assets._Scripts.LevelEditor.Tools
{
    [UnityComponent]
    public class SimpleRotationPlacer : SimplePlacerTool
    {
        private int rotation;

        protected override void PrePlace(IPlacedObject instantiated)
        {
            var rotatable = instantiated as IHasRotation;
            if (rotatable != null)
            {
                rotatable.RotationDegrees = rotation;
            }
        }

        public override void KeyPressed(KeyCode key)
        {
            if (key == KeyCode.R)
            {
                rotation -= 90;
                if (rotation < 0)
                    rotation -= 360;
                else if (rotation > 360)
                    rotation -= 360;

                transform.rotation = Quaternion.AngleAxis(rotation, Vector3.forward);
            }
        }
    }
}