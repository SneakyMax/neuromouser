using System;
using UnityEngine;
using System.Collections;

namespace Assets._Scripts.GameObjects
{
	[RequireComponent (typeof(Collider2D))]
	public class ExitPoint : InGameObject
	{
		public override int Layer { get { return 1; } }

		public override bool IsTraversableAt(GridPosition position)
		{
			return false;
		}

		public void OnTriggerEnter2D(Collider2D otherCollider)
		{
			GameStateController.Instance.PlayerGotToExit();
		}
	}
}