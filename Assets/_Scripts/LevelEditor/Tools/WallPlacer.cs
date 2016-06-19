using System;
using Assets._Scripts.LevelEditor.Objects;

namespace Assets._Scripts.LevelEditor.Tools
{
    [UnityComponent]
    public class WallPlacer : SimplePlacerTool
    {
        [AssignedInUnity]
        public WallType WallType;

        protected override void PostActivateTool(IPlacedObject placedObject)
        {
            var wall = placedObject as Wall;
            if (wall == null)
                throw new InvalidOperationException("Needed door but found " + placedObject.GetType().Name);

            wall.WallType = WallType;
        }
    }
}