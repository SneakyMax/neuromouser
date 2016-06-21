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
        private bool isHorizontal;

        public int Level { get { return level; } set { level = value; Refresh(); } }

        public bool IsHorizontal { get { return isHorizontal; } set { isHorizontal = value; Refresh(); } }

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
            
            Refresh();
        }

        private void Refresh()
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (Level == 1)
                spriteRenderer.sprite = Level1;
            else if (Level == 2)
                spriteRenderer.sprite = Level2;
            else if (Level == 3)
                spriteRenderer.sprite = Level3;


            transform.rotation = Quaternion.AngleAxis(IsHorizontal ? 90 : 0, Vector3.forward);
        }
    }
}