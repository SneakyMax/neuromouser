using System;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
	[RequireComponent (typeof(Animator))]
	[RequireComponent (typeof(Collider2D))]
    public class Door : InGameObject
    {
		public Color Level1Color;

		public Color Level2Color;

		public Color Level3Color;

        public override int Layer { get { return 2; } }

        public int Level { get; set; }

        public bool IsHorizontal { get; set; }

		public override bool IsDynamic { get { return true; } }

	    protected bool IsOpen { get; set; }

	    private Animator animator;

		private Collider2D doorCollider;

		public override bool IsTraversableAt(GridPosition position)
		{
			return IsOpen;
		}

		public override void GameStart()
		{
			HackerInterface.Instance.OnDoorPowerChanged += OnDoorPowerChanged;
			animator = GetComponent<Animator>();
			doorCollider = GetComponent<Collider2D>();
            
			switch ( Level )
			{
				case 1:
					GetComponent<SpriteRenderer>().color = Level1Color;
					break;
				case 2:
					GetComponent<SpriteRenderer>().color = Level2Color;
					break;
				case 3:
					GetComponent<SpriteRenderer>().color = Level3Color;
					break;
				default:
					GetComponent<SpriteRenderer>().color = Level3Color;
					break;
			}

		    animator.SetBool("IsOpen", false);
		}

	    protected override void Cleanup()
	    {
	        HackerInterface.Instance.OnDoorPowerChanged -= OnDoorPowerChanged;
	    }

	    protected void Open()
		{
			doorCollider.enabled = false;
		    animator.SetBool("IsOpen", true);
		}

		protected void Close()
		{
			doorCollider.enabled = true;
		    animator.SetBool("IsOpen", false);
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

            //var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
			if ((Level > newDoorPower) && IsOpen)
			{
				IsOpen = false;
				Close();
			}
			else if ((Level <= newDoorPower) && !IsOpen)
			{
				IsOpen = true;
				Open();
			}
		}
    }
}