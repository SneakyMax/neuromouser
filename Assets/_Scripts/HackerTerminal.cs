using UnityEngine;
using System.Collections;

public class HackerTerminal : MonoBehaviour
{
	//[SerializeField]
	//private GameObject
	// increases the power to the terminal if possible(return true), otherwise can not (return false)
	public bool IncreasePower()
	{
		if ((terminalPower + 1) > maxPower)
			return false;
		
		++terminalPower;

		return true;
	}

	// decreases the power to the terminal if possible(return true), otherwise can not (return false)
	public bool DecreasePower()
	{
		if ((terminalPower - 1) < 0)
			return false;
		
		--terminalPower;

		return true;
	}

	[SerializeField]
	private int maxPower = 3; // maximum amount of allowable power for this terminal

	private int terminalPower = 0; // amount of power allocated to the terminal
	public int TerminalPower // returns amount of power allocated to the terminal
	{
		get
		{
			return terminalPower;
		}
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		// kept for possible animation state
	}
}
