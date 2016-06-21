using System;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    public class PatrolPoint : PlacedObject
    {
        private static readonly int[] layers = { 4 };
        public override int[] Layers { get { return layers; } }

        public PatrolPoint NextPoint { get; set; }

        public PatrolPoint PreviousPoint { get; set; }

        private int? nextPointId;

        public override string Serialize()
        {
            return NextPoint != null ? NextPoint.Id.ToString() : "";
        }

        public override void Deserialize(string serialized)
        {
            nextPointId = serialized.Length == 0 ? (int?)null : Convert.ToInt32(serialized);
        }

        public override void PostAllDeserialized()
        {
            SetUpLineRenderer();
            
            if (nextPointId == null)
                return;

            var nextPoint = WorkingLevel.Instance.Get(nextPointId.Value) as PatrolPoint;
                
            if (nextPoint == null)
                throw new InvalidOperationException("Couldn't find next patrol point??");

            NextPoint = nextPoint;
            nextPoint.PreviousPoint = this;
        }

        public override void AfterPlace()
        {
            SetUpLineRenderer();
        }

        private void SetUpLineRenderer()
        {
            var lineRenderer = GetComponent<LineRenderer>();

            if (lineRenderer == null)
                throw new InvalidOperationException("Missing line renderer in patrol point.");

            if (PreviousPoint == null)
                return;

            lineRenderer.SetWidth(0.1f, 0.1f);
            lineRenderer.SetPositions(new[]
            {
                transform.position,
                PreviousPoint.transform.position
            });
        }
    }
}