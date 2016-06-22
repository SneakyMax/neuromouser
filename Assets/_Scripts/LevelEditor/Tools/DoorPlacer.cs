using System;
using Assets._Scripts.LevelEditor.Objects;

namespace Assets._Scripts.LevelEditor.Tools
{
    [UnityComponent]
    public class DoorPlacer : VerticalOrHorizontalPlacer
    {
        [AssignedInUnity]
        public int Level;

        protected override void PostActivateTool(IPlacedObject placedObject)
        {
            var door = placedObject as Door;
            if (door == null)
                throw new InvalidOperationException("Needed door but found " + placedObject.GetType().Name);

            door.Level = Level;
        }
    }
}