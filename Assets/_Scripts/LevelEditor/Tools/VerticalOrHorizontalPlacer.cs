using UnityEngine;

namespace Assets._Scripts.LevelEditor.Tools
{
    [UnityComponent]
    public class VerticalOrHorizontalPlacer : SimplePlacerTool
    {
        private bool isHorizontal;

        protected override void PrePlace(IPlacedObject instantiated)
        {
            var rotatable = instantiated as IHasVerticalOrHorizontalOrientation;
            if (rotatable != null)
            {
                rotatable.IsHorizontal = isHorizontal;
            }
        }

        public override void KeyPressed(KeyCode key)
        {
            if (key != KeyCode.R)
                return;

            isHorizontal = !isHorizontal;

            transform.rotation = Quaternion.AngleAxis(isHorizontal ? 90 : 0, Vector3.forward);
        }
    }
}