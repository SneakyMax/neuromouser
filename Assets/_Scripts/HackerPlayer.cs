using UnityEngine;
using System.Collections;
using Assets._Scripts;

/// <summary>
/// Hacker-mouse player handling (including input).
/// </summary>
public class HackerPlayer : MonoBehaviour
{
	/// <summary>
	/// The type of attached terminal.
	/// </summary>
	private enum TerminalType
	{
		Cameras,
		Doors,
		Traps,
		Cats
	}

	/// <summary>
	/// The regular shot prefab to instantiate when firing.
	/// </summary>
	public GameObject RegularShot = null;

	/// <summary>
	/// The burst shot prefab to instantiate when firing.
	/// </summary>
	public GameObject BurstShot = null;

	/// <summary>
	/// The repeat time for limiting movement presses.
	/// </summary>
	public float MoveRepeatTime = 0.25f;

	/// <summary>
	/// The time in seconds for a burst to reach completeness.
	/// </summary>
	public float FullBurstTime = 2f;

	/// <summary>
	/// Where above the center of the player to start the shot.
	/// </summary>
	public float ShotStartYOffset = .25f;

	/// <summary>
	/// The current held shot time.
	/// </summary>
	private float currentShotTime = 0f;

	/// <summary>
	/// The currently accessed terminal.
	/// </summary>
	private TerminalType currentTerminal = TerminalType.Cameras;

	/// <summary>
	/// The coroutine set for limiting keystrokes.
	/// </summary>
	private IEnumerator cantMove = null;

	/// <summary>
	/// Set if the player is charging a shot.
	/// </summary>
	private bool chargingShot = false;

    private HackerTerminal currentTerminalObject;

	/// <summary>
	/// Head to next terminal.
	/// </summary>
	private void GoToNextTerminal()
	{
		switch (currentTerminal)
		{
			case (TerminalType.Cameras):
				currentTerminal = TerminalType.Doors;
		        currentTerminalObject = HackerInterface.Instance.TerminalDoors;
				transform.position = HackerInterface.Instance.TerminalDoors.transform.position;
				break;
			case (TerminalType.Doors):
				currentTerminal = TerminalType.Traps;
                currentTerminalObject = HackerInterface.Instance.TerminalTraps;
                transform.position = HackerInterface.Instance.TerminalTraps.transform.position;
				break;
			case (TerminalType.Traps):
				currentTerminal = TerminalType.Cats;
                currentTerminalObject = HackerInterface.Instance.TerminalCats;
                transform.position = HackerInterface.Instance.TerminalCats.transform.position;
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Head to previous terminal.
	/// </summary>
	private void GoToPreviousTerminal()
	{
		switch ( currentTerminal )
		{
			case (TerminalType.Doors):
				currentTerminal = TerminalType.Cameras;
                currentTerminalObject = HackerInterface.Instance.TerminalCamera;
                transform.position = HackerInterface.Instance.TerminalCamera.transform.position;
				break;
			case (TerminalType.Traps):
				currentTerminal = TerminalType.Doors;
                currentTerminalObject = HackerInterface.Instance.TerminalDoors;
                transform.position = HackerInterface.Instance.TerminalDoors.transform.position;
				break;
			case (TerminalType.Cats):
				currentTerminal = TerminalType.Traps;
                currentTerminalObject = HackerInterface.Instance.TerminalTraps;
                transform.position = HackerInterface.Instance.TerminalTraps.transform.position;
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// This turns on the horizontal axis for the player for MoveRepeatTime number of seconds.
	/// </summary>
	/// <returns>IEnumerator</returns>
	private IEnumerator HorizontalAxisRepeatSwitch()
	{
		yield return new WaitForSeconds(MoveRepeatTime);
		cantMove = null;
	}

	/// <summary>
	/// Checks prefabs.
	/// </summary>
	/// <exception cref="UnityException">Thrown if Shot prefabs are not specified.</exception>
	private void Awake()
	{
		if (RegularShot == null)
		{
			throw new UnityException("Error: RegularShot prefab not specified in HackerPlayer.");
		}
		else if (BurstShot == null)
		{
			throw new UnityException("Error: BurstShot prefab not specified in HackerPlayer.");
		}
	}

	/// <summary>
	/// Sets the starting player location.
	/// </summary>
	private void Start()
	{
		transform.position = HackerInterface.Instance.TerminalCamera.transform.position;
	    currentTerminalObject = HackerInterface.Instance.TerminalCamera;
        currentTerminal = TerminalType.Cameras;
	}

	/// <summary>
	/// Update the player this frame.
	/// </summary>
	private void Update()
	{
		float horizontal = Input.GetAxisRaw("HorizontalHackerAxis");

		if (cantMove == null)
		{
			if (horizontal > float.Epsilon)
			{
				GoToNextTerminal();
				TempStopMove();
			}
			else if (horizontal < -float.Epsilon)
			{
				GoToPreviousTerminal();
				TempStopMove();
			}
		}
		else if (Mathf.Abs(horizontal) <= float.Epsilon)
		{
			StopCoroutine(cantMove);
			cantMove = null;
		}

		// current shot time full burst time
		if (Input.GetButtonDown("Fire1"))
		{
			chargingShot = true;
			ShootNormal();
			currentShotTime = Time.deltaTime;
		}
		if (Input.GetButtonUp("Fire1"))
		{
			if (chargingShot && (currentShotTime >= FullBurstTime))
				ShootBurst();

			chargingShot = false;
		}
		else if (chargingShot)
		{
			currentShotTime += Time.deltaTime;
		}

	}

	/// <summary>
	/// Shoots a normal shot.
	/// </summary>
	private void ShootNormal()
	{
	    var shotInstance = Instantiate(RegularShot);

	    var firstPosition = currentTerminalObject.PowerReader.GetComponent<PowerLevelIndicator>().StartPosition.position;

        shotInstance.transform.position = firstPosition;
	    shotInstance.GetComponent<HackerShot>().ParentTerminal = currentTerminalObject;
	}

	/// <summary>
	/// Shoots a burst shot.
	/// </summary>
	private void ShootBurst()
	{
		BurstShot.transform.position = new Vector3(transform.position.x, transform.position.y + ShotStartYOffset);
		Instantiate(BurstShot);
	}

	/// <summary>
	/// Puts a temporary stop in place for repeating keystrokes by starting HorizontalAxisRepeatSwitch()
	/// as a coroutine.
	/// </summary>
	private void TempStopMove()
	{
		cantMove = HorizontalAxisRepeatSwitch();
		StartCoroutine(cantMove);
	}
}
