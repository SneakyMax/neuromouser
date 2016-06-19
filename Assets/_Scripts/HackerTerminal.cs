using UnityEngine;
using System.Collections;

/// <summary>
/// Hacker terminal.
/// </summary>
public class HackerTerminal : MonoBehaviour
{
	/// <summary>
	/// Used for calling when the power from the terminal is changed.
	/// </summary>
	public delegate void TerminalPowerChange(int currentPower);

	/// <summary>
	/// This event fires when the power from the terminal is changed.
	/// Note: I recommend tying into this during Start() instead of OnEnable() to eliminate
	/// any null reference issues. -Joe
	/// </summary>
	public event TerminalPowerChange OnPowerChanged;

	/// <summary>
	/// Gets the amount of power allocated to the terminal.
	/// </summary>
	/// <value>The terminal power.</value>
	public int TerminalPower
	{
		get
		{
			return terminalPower;
		}
	}

	/// <summary>
	/// PowerToggle level 1 instance.
	/// </summary>
	public PowerToggle PowerToggle1 = null;

	/// <summary>
	/// PowerToggle level 2 instance.
	/// </summary>
	public PowerToggle PowerToggle2 = null;

	/// <summary>
	/// PowerToggle level 3 instance.
	/// </summary>
	public PowerToggle PowerToggle3 = null;

	/// <summary>
	/// The ICE instance.
	/// </summary>
	public ICEHandler PowerReader = null;

	/// <summary>
	/// The amount of power allocated to the terminal.
	/// </summary>
	private int terminalPower = 0;

	/// <summary>
	/// Does basic error checking.
	/// </summary>
	/// <exception cref="UnityException">Throws if instances aren't set.</exception>
	private void Awake()
	{
		if (PowerReader == null)
		{
			throw new UnityException("PowerReader not set for terminal");
		}
	}

	/// <summary>
	/// Sets up the callback.
	/// </summary>
	private void Start()
	{
		PowerReader.OnPowerChange += OnPowerChange;
	}

	/// <summary>
	/// Handles the OnPowerChange event
	/// </summary>
	/// <param name="newPower">New power value.</param>
	/// <exception cref="UnityException">If newPower is not value from 0-3.</exception>
	private void OnPowerChange(int newPower)
	{
	    if (PowerToggle1 == null || PowerToggle2 == null || PowerToggle3 == null)
	        return;

		terminalPower = newPower;

		switch (newPower)
		{
			case 0:
				PowerToggle1.PowerOnStatus = false;
				PowerToggle2.PowerOnStatus = false;
				PowerToggle3.PowerOnStatus = false;
				break;
			case 1:
				PowerToggle1.PowerOnStatus = true;
				PowerToggle2.PowerOnStatus = false;
				PowerToggle3.PowerOnStatus = false;
				break;
			case 2:
				PowerToggle1.PowerOnStatus = true;
				PowerToggle2.PowerOnStatus = true;
				PowerToggle3.PowerOnStatus = false;
				break;
			case 3:
				PowerToggle1.PowerOnStatus = true;
				PowerToggle2.PowerOnStatus = true;
				PowerToggle3.PowerOnStatus = true;
				break;
			default:
				throw new UnityException("Terminal OnPowerChange received invalid power value.");
		}

		OnPowerChanged(terminalPower);
	}
}
