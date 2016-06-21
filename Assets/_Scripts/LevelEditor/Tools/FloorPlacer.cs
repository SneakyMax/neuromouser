using System;
using Assets._Scripts.LevelEditor.Objects;

namespace Assets._Scripts.LevelEditor.Tools
{
    [UnityComponent]
    public class FloorPlacer : SimplePlacerTool
    {
        [AssignedInUnity]
        public FloorType FloorType;

        protected override void PostActivateTool(IPlacedObject placedObject)
        {
            var floor = placedObject as Floor;
            if (floor == null)
                throw new InvalidOperationException("Needed floor but found " + placedObject.GetType().Name);

            floor.FloorType = FloorType;
        }
    }
}