using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    public class TrapCatnip : InGameObject
    {
        public override int Layer { get { return 2; } }

        [AssignedInUnity]
        public GameObject CatnipDebuffPrefab;

        [UnityMessage]
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") == false)
                return;

            var player = other.gameObject.GetComponent<RunnerPlayer>();

            var existingDebuff = player.GetComponentInChildren<CatnipDebuff>();
            if (existingDebuff != null)
            {
                existingDebuff.RefreshDuration();
                return;
            }

            var debuffInstance = Instantiate(CatnipDebuffPrefab);
            debuffInstance.transform.SetParent(player.gameObject.transform, false);

            //TODO sound and particles
        }
    }
}