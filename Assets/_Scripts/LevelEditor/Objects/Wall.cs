using System;
using System.Linq;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [Serializable]
    public enum WallType { Glass, Cardboard, Metal, Nice }

    [UnityComponent]
    public class Wall : PlacedObject
    {
        private static readonly int[] layers = { 0, 1, 2, 3 };
        public override int[] Layers { get { return layers; } }
        [AssignedInUnity]
        public Sprite GlassWallSprite;

        [AssignedInUnity]
        public Sprite GlassWallSideSprite;

        [AssignedInUnity]
        public Sprite CardboardWallSprite;

        [AssignedInUnity]
        public Sprite CardboardWallSideSprite;

        [AssignedInUnity]
        public Sprite MetalWallSprite;

        [AssignedInUnity]
        public Sprite MetalWallSideSprite;

        [AssignedInUnity]
        public Sprite NiceWallSprite;

        [AssignedInUnity]
        public Sprite NiceWallSideSprite;

        [AssignedInUnity]
        public Transform MainWallSprite;

        [AssignedInUnity]
        public Transform VerticalWallCover;

        private WallType wallType;
        public WallType WallType { get { return wallType; } set { wallType = value; WallTypeChanged(); } }

        public override string Serialize()
        {
            return ((int)WallType).ToString();
        }

        public override void Deserialize(string serialized)
        {
            WallType = (WallType)Convert.ToInt32(serialized);
        }

        private void WallTypeChanged()
        {
            var spriteRenderer = MainWallSprite.GetComponent<SpriteRenderer>();
            var verticalWallCoverSpriteRenderer = VerticalWallCover.GetComponent<SpriteRenderer>();

            if (WallType == WallType.Glass)
            {
                spriteRenderer.sprite = GlassWallSprite;
                verticalWallCoverSpriteRenderer.sprite = GlassWallSideSprite;
            }
            else if (WallType == WallType.Cardboard)
            {
                spriteRenderer.sprite = CardboardWallSprite;
                verticalWallCoverSpriteRenderer.sprite = CardboardWallSideSprite;
            }
            else if (WallType == WallType.Metal)
            {
                spriteRenderer.sprite = MetalWallSprite;
                verticalWallCoverSpriteRenderer.sprite = MetalWallSideSprite;
            }
            else if (WallType == WallType.Nice)
            {
                spriteRenderer.sprite = NiceWallSprite;
                verticalWallCoverSpriteRenderer.sprite = NiceWallSideSprite;
            }
        }

        public override void AfterPlace()
        {
            CheckForWallAbove();
        }

        public override void PostAllDeserialized()
        {
            CheckForWallAbove();
        }

        public override void NearbyObjectChanged()
        {
            CheckForWallAbove();
        }

        private void CheckForWallAbove()
        {
            var above = PlacementGrid.Instance.GetGridPosition(transform.position) + new GridPosition(0, 1);
            var objectsAbove = WorkingLevel.Instance.GetGridObjectsAt(above);

            VerticalWallCover.gameObject.SetActive(objectsAbove.OfType<Wall>().Any());

            RefreshSpriteOrdering();
        }

        protected override void AfterRefreshSpriteOrdering()
        {
            VerticalWallCover.GetComponent<SpriteRenderer>().sortingOrder++; // Need to draw the overlay on top.
        }
    }
}