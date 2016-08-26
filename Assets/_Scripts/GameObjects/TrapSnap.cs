using FMODUnity;
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

	    public override bool IsDynamic { get { return true; } }

	    [AssignedInUnity]
	    public Sprite EnabledSprite;

	    [AssignedInUnity]
	    public Sprite DisabledSprite;

	    private bool armed;
	    private int level;

	    public override bool IsTraversableAt(GridPosition position)
		{
		    return true;
		}

	    public override void GameStart()
		{
			HackerInterface.Instance.OnTrapPowerChanged += OnTrapPowerChanged;
			level = 1;
		    Arm();
		}
		private void OnTrapPowerChanged(int newTrapPower)
		{
			if ((level > newTrapPower) && !armed)
			{
			    Arm();
			}
			else if ((level <= newTrapPower) && armed)
			{
			    Disarm();
			}
		}

	    private void Arm()
	    {
	        armed = true;

	        SpriteRenderer.sprite = EnabledSprite;
	    }

	    private void Disarm()
	    {
	        armed = false;

	        SpriteRenderer.sprite = DisabledSprite;
	    }

	    [UnityMessage]
	    public void OnTriggerEnter2D(Collider2D otherCollider)
	    {
	        if (otherCollider.tag == "Player" && armed)
	        {
	            SnapPlayer();
	        }
	        // TODO if tag == cat and level == 3
	    }

	    [UnityMessage]
		public void OnTriggerStay2D( Collider2D otherCollider)
		{
		    if (otherCollider.tag == "Player" && armed)
		    {
		        SnapPlayer();
		    }
			// TODO if tag == cat and level == 3
		}

	    private void SnapPlayer()
	    {
	        RuntimeManager.PlayOneShot(SnapSound, transform.position);
	        GameStateController.Instance.PlayerDied();
	    }
	}
}