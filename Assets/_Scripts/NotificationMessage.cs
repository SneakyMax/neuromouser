using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts
{
    [UnityComponent]
    public class NotificationMessage : MonoBehaviour
    {
        [AssignedInUnity]
        public float Duration;

        [AssignedInUnity]
        public float MoveAmount = 10;

        [UnityMessage]
        public void Start()
        {
            Destroy(gameObject, Duration);

            var rectTransform = GetComponent<RectTransform>();

            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + MoveAmount, Duration);

            var text = GetComponent<Text>();

            text.DOFade(0, Duration);
        }
    }
}