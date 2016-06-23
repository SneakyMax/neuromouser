using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.AI;
using Assets._Scripts.LevelEditor;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    public class TrapAlarm : InGameObject
    {
        [AssignedInUnity]
        public float AlarmRange;

        public override int Layer { get { return 2; } }

        public override bool IsDynamic { get { return true; } }

        public override bool IsTraversableAt(GridPosition position)
        {
            return true;
        }

		[FMODUnity.EventRef]
		public string alarmSound = "event:/alarm";	

        [UnityMessage]
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") == false)
                return;

            SoundAlarm();
        }

        private void SoundAlarm()
        {
			FMODUnity.RuntimeManager.PlayOneShot (alarmSound, transform.position);

			var cats = GetAllCatsInRange();

            foreach (var cat in cats)
            {
                cat.AI.GetState<InvestigatingAlarm>().TryInvestigateAlarm(this);
            }
        }

        private IList<Cat> GetAllCatsInRange()
        {
            var allCats = LevelLoader.AllInGameObjects.OfType<Cat>().ToList();

            return allCats.Where(IsInRange).ToList();
        }

        private bool IsInRange(Cat cat)
        {
            var catPosition = cat.transform.position;

            if (StartGridPosition == null)
                throw new InvalidOperationException("Alarm not aligned to grid.");

            var alarmPosition = PlacementGrid.Instance.GetWorldPosition(StartGridPosition.Value);

            return catPosition.DistanceTo(alarmPosition) <= AlarmRange;
        }
    }
}