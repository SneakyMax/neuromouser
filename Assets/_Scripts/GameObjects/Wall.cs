using System;
using System.Linq;
using Assets._Scripts.LevelEditor;
using Assets._Scripts.LevelEditor.Objects;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [UnityComponent]
    public class Wall : InGameObject
    {
        [AssignedInUnity]
        public Sprite CardboardWallSprite;

        [AssignedInUnity]
        public Sprite CardboardWallSideSprite;

        [AssignedInUnity]
        public Sprite NiceWallSprite;

        [AssignedInUnity]
        public Sprite NiceWallSideSprite;

        [AssignedInUnity]
        public Sprite MetalWallSprite;

        [AssignedInUnity]
        public Sprite MetalWallSideSprite;

        [AssignedInUnity]
        public Sprite GlassWallSprite;

        [AssignedInUnity]
        public Sprite GlassWallSideSprite;

        [AssignedInUnity]
        public Transform SpriteChild;

        [AssignedInUnity]
        public Transform SideWallOverlayChild;

        public override int Layer { get { return 2; } }

        public WallType WallType { get; set; }
        
        public override void Deserialize(string serialized)
        {
            try
            {
                WallType = (WallType)Convert.ToInt32(serialized);
            }
            catch (FormatException)
            {
                WallType = WallType.Metal;
            }
            
            var spriteRenderer = SpriteChild.GetComponent<SpriteRenderer>();
            var overlaySpriteRenderer = SideWallOverlayChild.GetComponent<SpriteRenderer>();

            switch (WallType)
            {
                case WallType.Cardboard:
                    spriteRenderer.sprite = CardboardWallSprite;
                    overlaySpriteRenderer.sprite = CardboardWallSideSprite;
                    break;
                case WallType.Nice:
                    spriteRenderer.sprite = NiceWallSprite;
                    overlaySpriteRenderer.sprite = NiceWallSideSprite;
                    break;
                case WallType.Metal:
                    spriteRenderer.sprite = MetalWallSprite;
                    overlaySpriteRenderer.sprite = MetalWallSideSprite;
                    break;
                case WallType.Glass:
                    spriteRenderer.sprite = GlassWallSprite;
                    overlaySpriteRenderer.sprite = GlassWallSideSprite;
                    break; //TODO can see through glass
            }

            spriteRenderer.gameObject.layer = LevelLoader.RunnerLayer;
            overlaySpriteRenderer.gameObject.layer = LevelLoader.RunnerLayer;
        }

        public override void PostAllDeserialized()
        {
            CheckForWallAbove();
        }

        private void CheckForWallAbove()
        {
            var above = PlacementGrid.Instance.GetGridPosition(transform.position) + new GridPosition(0, 1);
            var objectsAbove = LevelLoader.GetGridObjectsThatStartedAtPosition(above);

            SideWallOverlayChild.gameObject.SetActive(objectsAbove.OfType<Wall>().Any());
            SideWallOverlayChild.GetComponent<SpriteRenderer>().sortingOrder = SpriteChild.GetComponent<SpriteRenderer>().sortingOrder + 1; // Need to draw the overlay on top.
        }
    }
}