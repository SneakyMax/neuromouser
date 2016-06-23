using System.Collections;
using FMODUnity;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    public class TrapCatnip : InGameObject
    {
        public override int Layer { get { return 1; } }

        [EventRef]
        public string CatnipSound;

        [AssignedInUnity]
        public GameObject CatnipDebuffPrefab;

        [AssignedInUnity]
        public Sprite OffSprite;

        [AssignedInUnity]
        public Sprite OnSprite;

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
            if (terminalPower >= 2)
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
            IsArmed = true;
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

        private void CheckCollision(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") == false || IsArmed == false || isAlerted)
                return;

            isAlerted = true;

            StartCoroutine(ShowOnForASecond());

            RuntimeManager.PlayOneShot(CatnipSound, transform.position);

            var player = other.gameObject.GetComponent<RunnerPlayer>();

            var existingDebuff = player.GetComponentInChildren<CatnipDebuff>();
            if (existingDebuff != null)
            {
                existingDebuff.RefreshDuration();
                return;
            }

            var debuffInstance = Instantiate(CatnipDebuffPrefab);
            debuffInstance.transform.SetParent(player.gameObject.transform, false);
        }

        [UnityMessage]
        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") == false)
                return;

            isAlerted = false;
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