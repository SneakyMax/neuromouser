using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.AI;
using Assets._Scripts.GameObjects;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class CatnipDebuff : MonoBehaviour
    {
        /// <summary>Duration in seconds.</summary>
        [Range(1, 60)]
        public float Duration = 5;

        private IList<Cat> allCats;

        private Coroutine stopCoroutine;

        [UnityMessage]
        public void Start()
        {
            allCats = LevelLoader.Instance.AllInGameObjects.OfType<Cat>().ToList();

            StartCoroutine(RefreshEverySecond());
            stopCoroutine = StartCoroutine(StopAfterDuration());
        }

        private IEnumerator RefreshEverySecond()
        {
            while (true)
            {
                RefreshCatsAttration();
                yield return new WaitForSeconds(1);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public void RefreshDuration()
        {
            StopCoroutine(stopCoroutine);

            stopCoroutine = StartCoroutine(StopAfterDuration());
        }

        private void RefreshCatsAttration()
        {
            foreach (var cat in allCats)
            {
                cat.AI.GetState<AttractedToCatnip>().TryAttractToDebuff(this);
            }
        }

        public IEnumerator StopAfterDuration()
        {
            yield return new WaitForSeconds(Duration);

            foreach (var cat in allCats)
            {
                cat.AI.GetState<AttractedToCatnip>().CancelAttraction();
            }

            Destroy(gameObject);
        }
    }
}