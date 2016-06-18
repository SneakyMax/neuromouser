using System;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Tools
{
    [UnityComponent]
    public class SimpleRotationPlacer : Tool
    {
        [AssignedInUnity]
        public GameObject ThingToPlacePrefab;

        [AssignedInUnity]
        public bool SnapToGrid;

        public override bool ShouldSnapToGrid { get { return SnapToGrid; } }

        private bool isHorizontal;

        [UnityMessage]
        public void Start()
        {
            if (ThingToPlacePrefab.GetInterfaceComponent<IPlacedObject>() == null)
                throw new InvalidOperationException("Missing IPlacedObject component on " + ThingToPlacePrefab.name);
        }

        public override void ActivateTool(Vector2 position)
        {
            if (SnapToGrid && WorkingLevel.Instance.IsGridObjectAt(position))
                return;

            var instance = (GameObject)Instantiate(ThingToPlacePrefab, position, Quaternion.identity);
            var placed = instance.GetInterfaceComponent<IPlacedObject>();

            var rotatable = placed as IHasRotation;
            if (rotatable != null)
            {
                rotatable.IsHorizontal = isHorizontal;
            }

            if (SnapToGrid)
            {
                WorkingLevel.Instance.PlaceGridObject(placed, position);
            }
            else
            {
                WorkingLevel.Instance.Place(placed, position.x, position.y);
            }

            PostActivateTool(placed);
        }

        public override void SecondaryActivateTool(Vector2 position)
        {
            if (SnapToGrid)
            {
                var existingObject = WorkingLevel.Instance.GetGridObjectAt(position);
                if (existingObject != null)
                {
                    WorkingLevel.Instance.RemoveGridObjectAt(position);
                    Destroy(existingObject.UnityObject);
                }
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