using System;
using System.Collections;
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

        public override int Layer { get { return 1; } }

        public override bool IsDynamic { get { return true; } }

        public override bool IsTraversableAt(GridPosition position)
        {
            return true;
        }

        [FMODUnity.EventRef]
        public string alarmSound = "event:/alarm";

        [AssignedInUnity]
        public Sprite OnSprite;

        [AssignedInUnity]
        public Sprite OffSprite;

        [AssignedInUnity]
        public Sprite AlertedSprite;

        public bool IsArmed { get; private set; }

        private bool isAlerted;

        [UnityMessage]
        public void Start()
        {
            SpriteRenderer.sprite = OffSprite;
        }

        public override void GameStart()
        {
            Arm();
            HackerInterface.Instance.OnTrapPowerChanged += TrapPowerChanged;
        }

        private void TrapPowerChanged(int terminalPower)
        {
            if (terminalPower >= 1)
            {
                Disarm();
            }
            else
            {
                Arm();
            }
        }

        private void Arm()
        {
            IsArmed = true;
            SpriteRenderer.sprite = OnSprite;
        }

        private void Disarm()
        {
            IsArmed = false;
            SpriteRenderer.sprite = OffSprite;
        }

        [UnityMessage]
        public void OnTriggerEnter2D(Collider2D other)
        {
            CheckCollision(other);
        }

        [UnityMessage]
        public void OnTriggerStay2D(Collider2D other)
        {
            CheckCollision(other);
        }

        [UnityMessage]
        public void OnTriggerExit2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
                isAlerted = false;
        }

        private void CheckCollision(Collider2D other)
        {
            if (other.CompareTag("Player") == false || IsArmed == false || isAlerted)
                return;

            SoundAlarm();
        }

        private void SoundAlarm()
        {
            isAlerted = true;

			FMODUnity.RuntimeManager.PlayOneShot (alarmSound, transform.position);

            StartCoroutine(ShowOnForASecond());

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

        private IEnumerator ShowOnForASecond()
        {
            var previousSprite = SpriteRenderer.sprite;

            SpriteRenderer.sprite = AlertedSprite;
            yield return new WaitForSeconds(1);

            // Might have disarmed in this second.
            if (previousSprite == OnSprite)
                SpriteRenderer.sprite = previousSprite;
        }
    }
}