using System;
using UnityEngine;

namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class Door : PlacedObject, IHasRotation
    {
        [AssignedInUnity]
        public Sprite Level1;

        [AssignedInUnity]
        public Sprite Level2;

        [AssignedInUnity]
        public Sprite Level3;

        private int level;

        public int Level { get { return level; } set { level = value; LevelChanged(level); } }

        public bool IsHorizontal { get; set; }

        private static readonly int[] layers = { 1, 2, 3 };
        public override int[] Layers { get { return layers; } }

        public override string Serialize()
        {
            return String.Format("{0},{1}", Level, IsHorizontal);
        }

        public override void Deserialize(string serialized)
        {
            var split = serialized.Split(',');
            Level = Convert.ToInt32(split[0]);
            IsHorizontal = Convert.ToBoolean(split[1]);

            if (IsHorizontal)
            {
                UnityObject.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
            }
        }

        private void LevelChanged(int newLevel)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();

            if (newLevel == 1)
                spriteRenderer.sprite = Level1;
            else if (newLevel == 2)
                spriteRenderer.sprite = Level2;
            else if (newLevel == 3)
                spriteRenderer.sprite = Level3;
        }
    }
}