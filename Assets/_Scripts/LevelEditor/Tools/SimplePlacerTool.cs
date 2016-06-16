using Assets._Scripts.LevelEditor.Objects;
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

        public override void ActivateTool(Vector2 position)
        {
            if (SnapToGrid && WorkingLevel.Instance.IsGridObjectAt(position))
                return;

            var instance = (GameObject)Instantiate(ThingToPlacePrefab, position, Quaternion.identity);
            var placed = new PlacedWall(instance);

            if (SnapToGrid)
            {
                WorkingLevel.Instance.PlaceGridObject(placed, position);
            }
            else
            {
                WorkingLevel.Instance.Place(placed, position.x, position.y);
            }
        }
    }
}