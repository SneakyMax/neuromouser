using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the running player
/// </summary>
public class RunnerPlayer : MonoBehaviour
{
	/// <summary>
	/// Current player facing direction.
	/// </summary>
	public enum CurrentFacing
	{
		Left,
		Right,
		Up,
		Down
	}

	/// <summary>
	/// Gets the current direction the mouse is facing.
	/// </summary>
	/// <value>Gets the current facing direction.</value>
	public CurrentFacing CurrentFacingDirection
	{
		get
		{
			return facing;
		}
	}

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
	/// Is the player movement frozen?
	/// </summary>
	public bool PlayerMovementFrozen = false;

	/// <summary>
	/// The direction the player is currently facing.
	/// </summary>
	private CurrentFacing facing = CurrentFacing.Up;

	/// <summary>
	/// Handles the player movement.
	/// </summary>
	private void HandlePlayerMovement()
	{
		float horizontalAxis = Input.GetAxisRaw("Horizontal");
		float verticalAxis = Input.GetAxisRaw("Vertical");

		if (horizontalAxis > float.Epsilon)
		{
			horizontalAxis = (RunningSpeed * Time.deltaTime);
			facing = CurrentFacing.Right;
		}
		else if (horizontalAxis < -float.Epsilon)
		{
			horizontalAxis = (-RunningSpeed * Time.deltaTime);
			facing = CurrentFacing.Left;
		}
		// vertical facing > horizontal facing
		if (verticalAxis > float.Epsilon)
		{
			verticalAxis = (RunningSpeed * Time.deltaTime);
			facing = CurrentFacing.Up;
		}
		else if (verticalAxis < -float.Epsilon)
		{
			verticalAxis = (-RunningSpeed * Time.deltaTime);
			facing = CurrentFacing.Down;
		}
		GetComponent<Rigidbody2D>().MovePosition(new Vector2((transform.position.x + horizontalAxis),
                                                             (transform.position.y + verticalAxis)));
	}

	/// <summary>
	/// Moves the player and changes the facing direction.
	/// </summary>
	private void FixedUpdate()
	{
		if (!PlayerMovementFrozen)
			HandlePlayerMovement();
	}
}
