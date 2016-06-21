using System;
using Assets._Scripts.LevelEditor.Objects;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [UnityComponent]
    public class Floor : InGameObject
    {
        [AssignedInUnity]
        public Sprite WoodFloorSprite;

        [AssignedInUnity]
        public Sprite MetalFloorSprite;

        [AssignedInUnity]
        public Sprite CarpetFloorSprite;

        public override int Layer { get { return 0; } }

        public FloorType FloorType { get; set; }

        public override void Deserialize(string serialized)
        {
            try
            {
                FloorType = (FloorType)Convert.ToInt32(serialized);
            }
            catch (FormatException)
            {
                FloorType = FloorType.Wood;
            }

            var spriteRenderer = GetComponent<SpriteRenderer>();

            switch (FloorType)
            {
                case FloorType.Wood:
                    spriteRenderer.sprite = WoodFloorSprite;
                    break;
                case FloorType.Metal:
                    spriteRenderer.sprite = MetalFloorSprite;
                    break;
                case FloorType.Carpet:
                    spriteRenderer.sprite = CarpetFloorSprite;
                    break;
            }
        }
    }
}