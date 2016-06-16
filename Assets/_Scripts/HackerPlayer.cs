using UnityEngine;
using System.Collections;

/// <summary>
/// Hacker-mouse player handling (including input).
/// </summary>
public class HackerPlayer : MonoBehaviour
{
	/// <summary>
	/// Gets the currently-accessed terminal.
	/// </summary>
	/// <value>The currently-accessed terminal.</value>
	public int CurrentTerminal
	{
		get
		{
			return currentTerminal;
		}
	}

	/// <summary>
	/// The power level remaining for the hacker.
	/// </summary>
	public int PowerLevelRemaining = 3;

	/// <summary>
	/// The currently accessed terminal.
	/// </summary>
	private int currentTerminal = 0;

	/// <summary>
	/// Player can currently move horizontally.
	/// </summary>
	private bool canMoveHoriz = true;

	/// <summary>
	/// Player can currently move vertically.
	/// </summary>
	private bool canMoveVert = true;

	/// <summary>
	/// The repeat time for limiting keystrokes.
	/// </summary>
	private float repeatTime = 0.25f;

	/// <summary>
	/// Head to next terminal.
	/// </summary>
	private void GoToNextTerminal()
	{
		if ((currentTerminal + 1) == HackerInterface.Instance.Terminals.Length)
		{
			return;
			// currentTerminal = 0; // uncomment for wraparound
		}
		else // else kept in case we want wraparound
		{
			++currentTerminal;
		}

		transform.position = HackerInterface.Instance.Terminals[currentTerminal].transform.position;
	}

	/// <summary>
	/// Head to previous terminal.
	/// </summary>
	private void GoToPrevTerminal()
	{
		if ((currentTerminal - 1) < 0)
		{
			return;
			//currentTerminal = HackerInterface.instance.terminals.Length - 1; // uncomment for wraparound
		}
		else // else kept in case we want wraparound
		{
			--currentTerminal;
		}
		transform.position = HackerInterface.Instance.Terminals[currentTerminal].transform.position;
	}

	/// <summary>
	/// Increases the terminal power level if possible.
	/// </summary>
	private void IncreaseTerminalPowerLevel()
	{
		if (PowerLevelRemaining <= 0)
		{
			return;
		}

		if (HackerInterface.Instance.Terminals[currentTerminal].IncreasePower() == true)
		{
			--PowerLevelRemaining;
		}
	}

	/// <summary>
	/// Decreases the terminal power level if possible.
	/// </summary>
	private void DecreaseTerminalPowerLevel()
	{
		if (HackerInterface.Instance.Terminals[currentTerminal].DecreasePower() == true)
		{
			++PowerLevelRemaining;
		}
	}

	/// <summary>
	/// This turns on the horizontal axis for the player after a number of seconds.
	/// </summary>
	/// <returns>IEnumerator</returns>
	private IEnumerator HorizontalAxisRepeatSwitch()
	{
		yield return new WaitForSeconds(repeatTime);
		canMoveHoriz = true;
	}

	/// <summary>
	/// This turns on the vertical axis for the player after a number of seconds.
	/// </summary>
	/// <returns>IENumerator</returns>
	private IEnumerator VerticalAxisRepeatSwitch()
	{
		yield return new WaitForSeconds(repeatTime);
		canMoveVert = true;
	}
		
	/// <summary>
	/// Update the player this frame.
	/// </summary>
	private void Update()
	{
		float horizontal = Input.GetAxisRaw("HorizontalHackerAxis");
		float vertical = Input.GetAxisRaw("VerticalHackerAxis");
		if (canMoveHoriz == true)
		{
			if (horizontal > float.Epsilon)
			{
				GoToNextTerminal();
				canMoveHoriz = false;
				StartCoroutine("HorizontalAxisRepeatSwitch");
			}
			else if (horizontal < -(float.Epsilon))
			{
				GoToPrevTerminal();
				canMoveHoriz = false;
				StartCoroutine("HorizontalAxisRepeatSwitch");
			}
		}

		if (canMoveVert == true)
		{
			if (vertical > float.Epsilon)
			{
				IncreaseTerminalPowerLevel();
				canMoveVert = false;
				StartCoroutine("VerticalAxisRepeatSwitch");
			}
			else if (vertical < -(float.Epsilon))
			{
				DecreaseTerminalPowerLevel();
				canMoveVert = false;
				StartCoroutine("VerticalAxisRepeatSwitch");
			}
		}

	}
}
