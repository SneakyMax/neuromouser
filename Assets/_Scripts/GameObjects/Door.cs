using System;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    public class Door : InGameObject
    {
        [AssignedInUnity]
        public Sprite Level1DoorSprite;

        [AssignedInUnity]
        public Sprite Level2DoorSprite;

        [AssignedInUnity]
        public Sprite Level3DoorSprite;

        public override int Layer { get { return 2; } }

        public int Level { get; set; }

        public bool IsHorizontal { get; set; }

        public bool IsOpen { get; set; } //TODO

        public override void Deserialize(string serialized)
        {
            var split = serialized.Split(',');
            Level = Convert.ToInt32(split[0]);
            IsHorizontal = Convert.ToBoolean(split[1]);

            if (IsHorizontal)
            {
                transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
            }

            var spriteRenderer = GetComponent<SpriteRenderer>();
            switch (Level)
            {
                case 1:
                    spriteRenderer.sprite = Level1DoorSprite;
                    break;
                case 2:
                    spriteRenderer.sprite = Level2DoorSprite;
                    break;
                case 3:
                    spriteRenderer.sprite = Level3DoorSprite;
                    break;
            }
        }
    }
}