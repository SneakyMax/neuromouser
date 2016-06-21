using System;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
	[RequireComponent (typeof(Animator))]
	[RequireComponent (typeof(Collider2D))]
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

		public override bool IsDynamic { get { return true; } }

		protected bool open = false;

		private Animator animator = null;

		private Collider2D doorCollider = null;

		public override bool IsTraversableAt(GridPosition position)
		{
			return open;
		}

		public override void GameStart()
		{
			HackerInterface.Instance.OnDoorPowerChanged += OnDoorPowerChanged;
			animator = GetComponent<Animator>();
			doorCollider = GetComponent<Collider2D>();
			animator.Play("Opening");
			animator.SetFloat("AnimChangeMultiplier", 0f);
		}

		protected void Open()
		{
			doorCollider.enabled = false;
			animator.SetFloat("AnimChangeMultiplier", 1f);
		}

		protected void Close()
		{
			doorCollider.enabled = true;
			animator.SetFloat("AnimChangeMultiplier", -1f);
		}

        public override void Deserialize(string serialized)
        {
            var split = serialized.Split(',');
            Level = Convert.ToInt32(split[0]);
            IsHorizontal = Convert.ToBoolean(split[1]);

            if (IsHorizontal)
            {
                transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
            }

            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            /*switch (Level)
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
            }*/
        }

		/// <summary>
		/// Sets the power and calls Open() or Close() as necessary.
		/// </summary>
		/// <param name="newDoorPower">New door power.</param>
		private void OnDoorPowerChanged(int newDoorPower)
		{
			if ((Level > newDoorPower) && open)
			{
				open = false;
				Close();
			}
			else if ((Level <= newDoorPower) && !open)
			{
				open = true;
				Open();
			}
		}
    }
}