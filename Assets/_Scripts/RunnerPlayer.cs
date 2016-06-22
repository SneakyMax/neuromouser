using UnityEngine;
using System.Collections;
using Assets._Scripts;
using Assets._Scripts.GameObjects;

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
	/// This speed modifier is used for glue traps.
	/// </summary>
	public float GlueTrapSpeedModifier = .25f;

	/// <summary>
	/// The number of glue traps affecting player. Set by the traps.
	/// </summary>
	public int GlueTrapsAffectingPlayer = 0;

	/// <summary>
	/// The direction the player is currently facing.
	/// </summary>
	private CurrentFacing facing = CurrentFacing.Up;

    private SpriteRenderer spriteRenderer;
    private new Rigidbody2D rigidbody;

    private Vector2 requestedMovement;

    [UnityMessage]
    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
	/// Moves the player and changes the facing direction.
	/// </summary>
    [UnityMessage]
	private void Update()
	{
		if (!PlayerMovementFrozen)
			HandlePlayerMovement();

	    const int layer = 2;

        var bottomOfSpritePosition = spriteRenderer.bounds.min;
	    spriteRenderer.sortingOrder = InGameObject.GetSortPosition(bottomOfSpritePosition, layer);
	}

    [UnityMessage]
    public void FixedUpdate()
    {
        var movement = requestedMovement * Time.deltaTime;
        rigidbody.MovePosition(transform.position + (Vector3)movement);
		rigidbody.AddForce( Vector2.zero ); // required for OnTriggerStay2D when player is not moving
    }

    /// <summary>
    /// Handles the player movement.
    /// </summary>
    private void HandlePlayerMovement()
    {
        var horizontalAxis = Input.GetAxisRaw("Horizontal");
        var verticalAxis = Input.GetAxisRaw("Vertical");

        if (horizontalAxis > float.Epsilon)
        {
            horizontalAxis = 1;
            facing = CurrentFacing.Right;
        }
        else if (horizontalAxis < -float.Epsilon)
        {
            horizontalAxis = -1;
            facing = CurrentFacing.Left;
        }
        // vertical facing > horizontal facing
        if (verticalAxis > float.Epsilon)
        {
            verticalAxis = 1;
            facing = CurrentFacing.Up;
        }
        else if (verticalAxis < -float.Epsilon)
        {
            verticalAxis = -1;
            facing = CurrentFacing.Down;
        }

        //TODO this won't work for analog
		if (GlueTrapsAffectingPlayer > 0)
			requestedMovement = new Vector2(horizontalAxis, verticalAxis).normalized * RunningSpeed * GlueTrapSpeedModifier;
		else
			requestedMovement = new Vector2(horizontalAxis, verticalAxis).normalized * RunningSpeed;
    }
}
