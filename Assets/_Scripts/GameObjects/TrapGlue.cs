using UnityEngine;

namespace Assets._Scripts.GameObjects
{
	[RequireComponent (typeof(Collider2D))]
	[RequireComponent (typeof(Animator))]
	public class TrapGlue : InGameObject
	{
		[FMODUnity.EventRef]
		public string glueSound = "event:/glue_walk";

		public override int Layer { get { return 1; } }

		public int Level { get; set; }

		public override bool IsDynamic { get { return true; } }

	    protected bool IsArmed { get; set; }

	    private bool playerSlowed = false;

		public override bool IsTraversableAt(GridPosition position)
		{
			//return !armed;
			return true;
		}

		public override void GameStart()
		{
		    IsArmed = true;
			HackerInterface.Instance.OnTrapPowerChanged += OnTrapPowerChanged;
			Level = 2;
			GetComponent<Animator>().SetTrigger("Armed");
		}

	    /// <summary>
		/// Sets the power of the trap and armed.
		/// </summary>
		/// <param name="newTrapPower">New trap power.</param>
		private void OnTrapPowerChanged(int newTrapPower)
		{
			if ((Level > newTrapPower) && !IsArmed)
			{
				IsArmed = true;
				GetComponent<Animator>().SetTrigger("Armed");
			}
			else if ((Level <= newTrapPower) && IsArmed)
			{
				IsArmed = false;
				GetComponent<Animator>().SetTrigger("Disarmed");
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
			if (IsArmed)
			{
				if (otherCollider.tag == "Player")
				{

					FMODUnity.RuntimeManager.PlayOneShot (glueSound, transform.position);
					SlowPlayer(otherCollider.GetComponent<RunnerPlayer>());
				}
				// TODO if tag == cat and level == 3
			}
		}

		public void OnTriggerStay2D( Collider2D otherCollider)
		{
			if (otherCollider.tag == "Player")
			{
				if (IsArmed && !playerSlowed)
				{
					SlowPlayer(otherCollider.GetComponent<RunnerPlayer>());
				}
				else if (!IsArmed && playerSlowed)
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
				FMODUnity.RuntimeManager.PlayOneShot (glueSound, transform.position);
				UnslowPlayer(otherCollider.GetComponent<RunnerPlayer>());
			}
			// TODO if tag == cat and level == 3
		}
	}
}