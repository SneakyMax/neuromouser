using System;
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
        public Sprite CardboardWallSprite;

        [AssignedInUnity]
        public Sprite MetalWallSprite;

        [AssignedInUnity]
        public Sprite NiceWallSprite;

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
            var spriteRenderer = GetComponent<SpriteRenderer>();

            if (WallType == WallType.Glass)
                spriteRenderer.sprite = GlassWallSprite;
            else if (WallType == WallType.Cardboard)
                spriteRenderer.sprite = CardboardWallSprite;
            else if (WallType == WallType.Metal)
                spriteRenderer.sprite = MetalWallSprite;
            else if (WallType == WallType.Nice)
                spriteRenderer.sprite = NiceWallSprite;
        }
    }
}