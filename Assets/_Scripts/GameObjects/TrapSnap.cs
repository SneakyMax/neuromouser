﻿using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
	[RequireComponent (typeof(Collider2D))]
	public class TrapSnap : InGameObject
	{
        [EventRef, UsedImplicitly]
	    public string SnapSound;

		public override int Layer { get { return 1; } }

		public int Level { get; set; }

		public override bool IsDynamic { get { return true; } }

		protected bool armed = true;

		public override bool IsTraversableAt(GridPosition position)
		{
			return !armed;
		}

		public override void GameStart()
		{
			HackerInterface.Instance.OnTrapPowerChanged += OnTrapPowerChanged;
			Level = 1;
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

        // TODO Slow and Unslow cats

        [UnityMessage]
        public void OnTriggerEnter2D(Collider2D otherCollider)
		{
		    if (otherCollider.tag == "Player" && armed)
		    {
		        RuntimeManager.PlayOneShot(SnapSound, transform.position);
		        GameStateController.Instance.PlayerDied();
		    }
			// TODO if tag == cat and level == 3
		}

        [UnityMessage]
		public void OnTriggerStay2D( Collider2D otherCollider)
		{
		    if (otherCollider.tag == "Player" && armed)
		    {
                RuntimeManager.PlayOneShot(SnapSound, transform.position);
                GameStateController.Instance.PlayerDied();
		    }
			// TODO if tag == cat and level == 3
		}
	}
}