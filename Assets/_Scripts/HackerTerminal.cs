using UnityEngine;
using System.Collections;

public class HackerTerminal : MonoBehaviour
{
	[SerializeField]
	private PowerToggle [] powerToggles = null;
	// increases the power to the terminal if possible(return true), otherwise can not (return false)
	public bool IncreasePower()
	{
		if ((terminalPower + 1) > powerToggles.Length)
			return false;
		
		powerToggles[terminalPower++].PowerOnStatus = true;

		return true;
	}

	// decreases the power to the terminal if possible(return true), otherwise can not (return false)
	public bool DecreasePower()
	{
		if ((terminalPower - 1) < 0)
			return false;

		--terminalPower;

		powerToggles[terminalPower].PowerOnStatus = false;

		return true;
	}

	private int terminalPower = 0; // amount of power allocated to the terminal
	public int TerminalPower // returns amount of power allocated to the terminal
	{
		get
		{
			return terminalPower;
		}
	}

	// Use this for initialization
	void Start()
	{
		if ( powerToggles == null )
		{
			throw new UnityException("Error: No PowerToggles to use in HackerTerminal");
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		// kept for possible animation state
	}
}
