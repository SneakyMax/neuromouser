using UnityEngine;
using System.Collections;

/// <summary>
/// This script moves in a linear fashion at the chosen speeds.
/// </summary>
public class SteadyMove : MonoBehaviour
{
	/// <summary>
	/// The normal Y speed per second.
	/// </summary>
	public float YSpeedPerSecond = 0f;

	/// <summary>
	/// The normal X speed per second.
	/// </summary>
	public float XSpeedPerSecond = 0f;

	/// <summary>
	/// Moves the object to a position based on the time difference and given speed.
	/// </summary>
	private void FixedUpdate()
	{
		transform.position = new Vector3((transform.position.x + (XSpeedPerSecond * Time.deltaTime)),
			                             (transform.position.y + (YSpeedPerSecond * Time.deltaTime)));
	}
}
