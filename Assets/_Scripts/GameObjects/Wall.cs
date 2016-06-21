using System;
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
        public Sprite NiceWallSprite;

        [AssignedInUnity]
        public Sprite MetalWallSprite;

        [AssignedInUnity]
        public Sprite GlassWallSprite;

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
            
            var spriteRenderer = GetComponent<SpriteRenderer>();

            switch (WallType)
            {
                case WallType.Cardboard:
                    spriteRenderer.sprite = CardboardWallSprite;
                    break;
                case WallType.Nice:
                    spriteRenderer.sprite = NiceWallSprite;
                    break;
                case WallType.Metal:
                    spriteRenderer.sprite = MetalWallSprite;
                    break;
                case WallType.Glass:
                    spriteRenderer.sprite = GlassWallSprite;
                    break; //TODO can see through glass
            }
        }
    }
}