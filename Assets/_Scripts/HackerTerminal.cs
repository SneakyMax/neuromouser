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
	public delegate void TerminalPowerChange();

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
	/// The power toggle references.
	/// </summary>
	public PowerToggle [] PowerToggles = null;

	/// <summary>
	/// The amount of power allocated to the terminal.
	/// </summary>
	private int terminalPower = 0;

	/// <summary>
	/// Increases the power if possible.
	/// </summary>
	/// <returns><c>true</c>, if power was increased, <c>false</c> otherwise.</returns>
	public bool IncreasePower()
	{
		if ((terminalPower + 1) > PowerToggles.Length)
			return false;
		
		PowerToggles[terminalPower++].PowerOnStatus = true;

		if (OnPowerChanged != null)
			OnPowerChanged();

		return true;
	}

	/// <summary>
	/// Decreases the power if possible.
	/// </summary>
	/// <returns><c>true</c>, if power was decreased, <c>false</c> otherwise.</returns>
	public bool DecreasePower()
	{
		if ((terminalPower - 1) < 0)
			return false;

		--terminalPower;

		PowerToggles[terminalPower].PowerOnStatus = false;

		if (OnPowerChanged != null)
			OnPowerChanged();

		return true;
	}

	/// <summary>
	/// Start this instance.
	/// <exception cref="UnityException">Thrown if there are no powertoggles referenced.</exception>
	/// </summary>
	private void Start()
	{
		if ( PowerToggles == null )
		{
			throw new UnityException("Error: No PowerToggles to use in HackerTerminal");
		}
	}
}
