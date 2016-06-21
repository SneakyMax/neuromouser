using System;
using Assets._Scripts.LevelEditor.Objects;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Tools
{
    [UnityComponent]
    public class PatrolPointPlacer : SimplePlacerTool
    {
        private PatrolPoint PreviousPoint;

        protected override void PostActivateTool(IPlacedObject placedObject)
        {
            var patrolPoint = placedObject as PatrolPoint;
            if (patrolPoint == null)
                throw new InvalidOperationException("Not a patrol point.");

            if (PreviousPoint != null)
            {
                PreviousPoint.NextPoint = patrolPoint;
                patrolPoint.PreviousPoint = PreviousPoint;
            }

            PreviousPoint = patrolPoint;
        }
    }
}