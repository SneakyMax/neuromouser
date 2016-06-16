using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.LevelEditor
{
    /// <summary>Something in the palette you can click on.</summary>
    [UnityComponent]
    public class PaletteItem : MonoBehaviour
    {
        [AssignedInUnity]
        public GameObject ToolPrefab;

        private Image bgImage;
        private float startAlpha;

        [UnityMessage]
        public void Start()
        {
            bgImage = GetComponent<Image>();
            startAlpha = bgImage.color.a;
        }

        [UnityMessage]
        public void Update()
        {
            var cursorPosition = Input.mousePosition;
            var myBounds = ((RectTransform)transform).GetWorldRect();

            if (myBounds.Contains(cursorPosition))
            {
                Hover();
            }
            else
            {
                UnHover();
            }
        }

        private void Hover()
        {
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, 1.0f);
        }

        private void UnHover()
        {
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, startAlpha);
        }
    }
}