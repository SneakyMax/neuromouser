using System;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Tools
{
    [UnityComponent]
    public class SimplePlacerTool : Tool
    {
        [AssignedInUnity]
        public GameObject ThingToPlacePrefab;

        [AssignedInUnity]
        public bool SnapToGrid;

        public override bool ShouldSnapToGrid { get { return SnapToGrid; } }

        [UnityMessage]
        public void Start()
        {
            if(ThingToPlacePrefab.GetInterfaceComponent<IPlacedObject>() == null)
                throw new InvalidOperationException("Missing IPlacedObject component on " + ThingToPlacePrefab.name);

            ToolStart();
        }

        protected virtual void ToolStart()
        {
            
        }

        public override void ActivateTool(Vector2 position)
        {
            var layersObjectWillOccupy = ThingToPlacePrefab.GetInterfaceComponent<IPlacedObject>().Layers;

            if (SnapToGrid && WorkingLevel.Instance.IsAnyGridObjectAt(position, layersObjectWillOccupy))
                return;

            var instance = (GameObject)Instantiate(ThingToPlacePrefab, position, Quaternion.identity);
            var placed = instance.GetInterfaceComponent<IPlacedObject>();

            if (SnapToGrid)
            {
                WorkingLevel.Instance.PlaceGridObject(placed, position);
            }
            else
            {
                WorkingLevel.Instance.Place(placed, position.x, position.y);
            }

            PostActivateTool(placed);

            placed.AfterPlace();
        }

        public override void SecondaryActivateTool(Vector2 position)
        {
            if (SnapToGrid)
            {
                WorkingLevel.Instance.RemoveTopmostGridObjectAt(position);
            }
        }
    }
}