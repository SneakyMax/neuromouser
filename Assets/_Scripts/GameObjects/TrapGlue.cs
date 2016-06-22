using System;
using System.Collections;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
	[RequireComponent (typeof(Collider2D))]
	public class TrapGlue : InGameObject
	{
		public override int Layer { get { return 1; } }

		public int Level { get; set; }

		public bool IsHorizontal { get; set; }

		public override bool IsDynamic { get { return true; } }

		protected bool armed = true;

		private bool playerSlowed = false;

		private Collider2D trapCollider = null;

		public override bool IsTraversableAt(GridPosition position)
		{
			//return !armed;
			return true;
		}

		public override void GameStart()
		{
			HackerInterface.Instance.OnTrapPowerChanged += OnTrapPowerChanged;
			trapCollider = GetComponent<Collider2D>();
			Level = 2;
		}

		/// <summary>
		/// Sets the power of the trap and armed.
		/// </summary>
		/// <param name="newTrapPower">New trap power.</param>
		private void OnTrapPowerChanged(int newTrapPower)
		{
			if ((Level > newTrapPower) && !armed)
			{
				armed = true;
			}
			else if ((Level <= newTrapPower) && armed)
			{
				armed = false;
			}
			// TODO level 3 stuff.
		}

		private void SlowPlayer(RunnerPlayer player)
		{
			playerSlowed = true;
			++player.GlueTrapsAffectingPlayer;
		}

		private void UnslowPlayer(RunnerPlayer player)
		{
			playerSlowed = false;
			--player.GlueTrapsAffectingPlayer;
		}

		// TODO Slow and Unslow cats

		public void OnTriggerEnter2D(Collider2D otherCollider)
		{
			if (armed)
			{
				if (otherCollider.tag == "Player")
				{
					SlowPlayer(otherCollider.GetComponent<RunnerPlayer>());
				}
				// TODO if tag == cat and level == 3
			}
		}

		public void OnTriggerStay2D( Collider2D otherCollider)
		{
			if (otherCollider.tag == "Player")
			{
				if (armed && !playerSlowed)
				{
					SlowPlayer(otherCollider.GetComponent<RunnerPlayer>());
				}
				else if (!armed && playerSlowed)
				{
					UnslowPlayer(otherCollider.GetComponent<RunnerPlayer>());
				}
			}
			// TODO if tag == cat and level == 3
		}

		public void OnTriggerExit2D( Collider2D otherCollider )
		{
			if ((otherCollider.tag == "Player") && playerSlowed)
			{
				UnslowPlayer(otherCollider.GetComponent<RunnerPlayer>());
			}
			// TODO if tag == cat and level == 3
		}
	}
}