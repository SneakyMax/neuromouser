using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Collider2D))]
public class StandardDoorBehavior : DoorBehavior
{
	private Animator animator = null;
	private Collider2D doorCollider = null;

	protected override void Init()
	{
		animator = GetComponent<Animator>();
		doorCollider = GetComponent<Collider2D>();
		animator.SetFloat("AnimChangeMultiplier", 0f);
		animator.Play("Opening");
	}

	protected override void Open()
	{
		doorCollider.enabled = false;
		animator.SetFloat("AnimChangeMultiplier", 1f);
		print( "Open " + doorLevel.ToString() );
	}

	protected override void Close()
	{
		doorCollider.enabled = true;
		animator.SetFloat("AnimChangeMultiplier", -1f);
		print( "Close " + doorLevel.ToString() );
	}
}
