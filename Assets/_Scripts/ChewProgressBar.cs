using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent, RequireComponent(typeof(RectTransform))]
    public class ChewProgressBar : MonoBehaviour
    {
        private RectTransform rectTransform;
        private float fullWidth;

        [UnityMessage]
        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            fullWidth = rectTransform.sizeDelta.x;
        }

        public void SetPercent(float percent)
        {
            rectTransform.sizeDelta = new Vector2(fullWidth * percent, rectTransform.sizeDelta.y);
        }
    }
}