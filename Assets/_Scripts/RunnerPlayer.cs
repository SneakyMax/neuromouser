using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the running player
/// </summary>
public class RunnerPlayer : MonoBehaviour
{
	/// <summary>
	/// Gets or sets the current x tile the running player is on.
	/// </summary>
	/// <value>The current x.</value>
	public int CurrentX
	{
		get
		{
			return Mathf.RoundToInt(transform.position.x);
		}
		set
		{
			transform.position = new Vector3((float)value, transform.position.y);
		}
	}

	/// <summary>
	/// Gets or sets the current x tile the running player is on.
	/// </summary>
	/// <value>The current y.</value>
	public int CurrentY
	{
		get
		{
			return Mathf.RoundToInt(transform.position.y);
		}
		set
		{
			transform.position = new Vector3((float)value, transform.position.x);
		}
	}

	/// <summary>
	/// The full running speed of the running mouse in tiles per second.
	/// </summary>
	public float RunningSpeed = 2f;

	/// <summary>
	/// 
	/// </summary>
	private void FixedUpdate()
	{
		GetComponent<Rigidbody2D>().MovePosition(
						new Vector2((transform.position.x + (Input.GetAxis("Horizontal") * RunningSpeed * Time.deltaTime)),
				        		    (transform.position.y + (Input.GetAxis("Vertical") * RunningSpeed * Time.deltaTime))));
	}
}
