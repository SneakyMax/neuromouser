using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts
{
    [RequireComponent (typeof(Image)), UnityComponent]
    public class TransitionScreen : MonoBehaviour
    {
        public delegate void OnTransitionReturn();

        public event OnTransitionReturn TransitionReturn;

        private bool buttonDelay = true;

        public void ShowTransition(Sprite showSprite)
        {
            var image = GetComponent<Image>();

            if (showSprite != null)
            {
                enabled = true;
                image.enabled = true;
                image.sprite = showSprite;
                image.color = image.color.WithAlpha(1);

                StartCoroutine( DelayButtonPress() );
            }
            else
            {
                DisableTransition();
            }
        }

        [UnityMessage]
        private void Start()
        {
            GetComponent<Image>().enabled = false;
            enabled = false;
        }

        private IEnumerator DelayButtonPress()
        {
            buttonDelay = true;
            yield return new WaitForSeconds( 1.0f );
            buttonDelay = false;
        }

        private void DisableTransition()
        {
            enabled = false;
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().enabled = false;
            if (TransitionReturn != null)
            {
                TransitionReturn();
            }
        }

        private void Update()
        {
            if (GetAnyButtonPress() && !buttonDelay)
            {
                DisableTransition();
            }
        }

        private bool GetAnyButtonPress()
        {
            return (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical") ||
                    Input.GetButtonUp("HorizontalHackerAxis") || Input.GetButtonUp("VerticalHackerAxis") ||
                    Input.GetButtonUp("Fire1") || Input.GetButtonUp("Jump") || Input.GetButtonUp("Submit") ||
                    Input.GetButtonUp("Cancel") || Input.GetButtonUp("Escape") || Input.GetButtonUp("Chewing"));
        }
    }
}
