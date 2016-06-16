using UnityEngine;
using System.Collections;

public class HackerPlayer : MonoBehaviour
{
	public int powerLevelRemaining = 3; // the power level remaining of the hacker

	private int currentTerminal = 0; // the terminal currently being accessed

	public int CurrentTerminal // returns the terminal currently being accessed
	{
		get
		{
			return currentTerminal;
		}
	}

	// head to the next terminal, or wraparound
	private void GoToNextTerminal()
	{
		if ((currentTerminal + 1) == HackerInterface.instance.terminals.Length)
		{
			currentTerminal = 0;
		}
		else
		{
			++currentTerminal;
		}

		transform.position = HackerInterface.instance.terminals[currentTerminal].transform.position;
	}

	// head to the previous terminal, or wraparound
	private void GoToPrevTerminal()
	{
		if ((currentTerminal - 1) < 0)
		{
			currentTerminal = HackerInterface.instance.terminals.Length - 1;
		}
		else
		{
			--currentTerminal;
		}
		transform.position = HackerInterface.instance.terminals[currentTerminal].transform.position;
	}

	// increase the current power level of the terminal
	private void IncreaseTerminalPowerLevel()
	{
		if (powerLevelRemaining <= 0)
		{
			return;
		}

		if (HackerInterface.instance.terminals[currentTerminal].IncreasePower() == true)
		{
			--powerLevelRemaining;
		}
	}

	// decrease the current power level of the terminal
	private void DecreaseTerminalPowerLevel()
	{
		if (HackerInterface.instance.terminals[currentTerminal].DecreasePower() == true)
		{
			++powerLevelRemaining;
		}
	}

	// Use this for initialization
	void Start()
	{
	}

	private bool canMoveHoriz = true;

	IEnumerator HorizontalAxisRepeatSwitch()
	{
		yield return new WaitForSeconds(repeatTime);
		canMoveHoriz = true;
	}

	private bool canMoveVert = true;

	IEnumerator VerticalAxisRepeatSwitch()
	{
		yield return new WaitForSeconds(repeatTime);
		canMoveVert = true;
	}

	private float repeatTime = 0.25f;

	// Update is called once per frame
	void Update()
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
