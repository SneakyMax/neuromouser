using System;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    public class PatrolPoint : InGameObject
    {
        public override int Layer { get { return 4; } }

        public PatrolPoint NextPoint { get; set; }

        public PatrolPoint PreviousPoint { get; set; }

        private int? nextPointId;

        public override void Deserialize(string serialized)
        {
            nextPointId = serialized.Length == 0 ? (int?)null : Convert.ToInt32(serialized);
        }

        public override void PostAllDeserialized()
        {
            if (nextPointId == null)
                return;

            var nextPoint = LevelLoader.GetObject(nextPointId.Value) as PatrolPoint;
            if (nextPoint == null)
                throw new InvalidOperationException("Invalid level loaded. Msising object with id " + nextPointId);

            NextPoint = nextPoint;
            NextPoint.PreviousPoint = this;
        }

        public GridPosition Position
        {
            get
            {
                Debug.Assert(StartGridPosition != null, "StartGridPosition != null");
                // ReSharper disable once PossibleInvalidOperationException
                return StartGridPosition.Value;
            }
        }
    }
}