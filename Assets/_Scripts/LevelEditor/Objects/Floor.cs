using System;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [Serializable]
    public enum FloorType
    {
        Carpet, Metal, Wood
    }

    [UnityComponent]
    public class Floor : PlacedObject
    {
        private static readonly int[] layers = { 0 };
        public override int[] Layers { get { return layers; } }

        [AssignedInUnity]
        public Sprite CarpetFloorSprite;

        [AssignedInUnity]
        public Sprite WoodFloorSprite;

        [AssignedInUnity]
        public Sprite MetalFloorSprite;

        private FloorType floorType;
        public FloorType FloorType { get { return floorType; } set { floorType = value; FloorTypeChanged(); } }

        public override string Serialize()
        {
            return ((int)FloorType).ToString();
        }

        public override void Deserialize(string serialized)
        {
            FloorType = serialized.Length == 0 ? FloorType.Wood : (FloorType)Convert.ToInt32(serialized);
        }

        private void FloorTypeChanged()
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (FloorType == FloorType.Carpet)
                spriteRenderer.sprite = CarpetFloorSprite;
            else if (FloorType == FloorType.Wood)
                spriteRenderer.sprite = WoodFloorSprite;
            else if (FloorType == FloorType.Metal)
                spriteRenderer.sprite = MetalFloorSprite;
        }
    }
}