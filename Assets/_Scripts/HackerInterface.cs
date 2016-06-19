using UnityEngine;
using System.Collections;

/// <summary>
/// Hacker-mouse's interface.
/// </summary>
public class HackerInterface : MonoBehaviour
{
	/// <summary>
	/// This delegate lets a script know that the terminal power has changed.
	/// </summary>
	/// <param name="terminalPower">The current power level of the terminal</param>
	public delegate void TerminalPowerChanged(int terminalPower);

	/// <summary>
	/// This event fires when the camera terminal power has changed.
	/// </summary>
	public event TerminalPowerChanged OnCameraPowerChanged;

	/// <summary>
	/// This event fires when the trap terminal power has changed.
	/// </summary>
	public event TerminalPowerChanged OnTrapPowerChanged;

	/// <summary>
	/// This event fires when the door terminal power has changed.
	/// </summary>
	public event TerminalPowerChanged OnDoorPowerChanged;

	/// <summary>
	/// This event fires when the cat terminal power has changed.
	/// </summary>
	public event TerminalPowerChanged OnCatPowerChanged;

	/// <summary>
	/// The singleton instance of the interface.
	/// </summary>
	public static HackerInterface Instance = null;

	/// <summary>
	/// The camera terminal.
	/// </summary>
	public HackerTerminal TerminalCamera = null;

	/// <summary>
	/// The trap terminal.
	/// </summary>
	public HackerTerminal TerminalTraps = null;

	/// <summary>
	/// The door terminal.
	/// </summary>
	public HackerTerminal TerminalDoors = null;

	/// <summary>
	/// The cat terminal.
	/// </summary>
	public HackerTerminal TerminalCats = null;

	/// <summary>
	/// Raises the camera power changed event.
	/// </summary>
	/// <param name="currentPower">Current camera terminal power.</param>
	public void OnCameraPowerChange(int currentPower)
	{
		if (OnCameraPowerChanged != null)
		{
			OnCameraPowerChanged(currentPower);
		}
	}

	/// <summary>
	/// Raises the trap power changed event.
	/// </summary>
	/// <param name="currentPower">Current trap terminal power.</param>
	public void OnTrapPowerChange(int currentPower)
	{
		if (OnTrapPowerChanged != null)
		{
			OnTrapPowerChanged(currentPower);
		}
	}

	/// <summary>
	/// Raises the door power changed event.
	/// </summary>
	/// <param name="currentPower">Current door terminal power.</param>
	public void OnDoorPowerChange(int currentPower)
	{
		if (OnDoorPowerChanged != null)
		{
			OnDoorPowerChanged(currentPower);
		}
	}

	/// <summary>
	/// Raises the cat power changed event.
	/// </summary>
	/// <param name="currentPower">Current cat terminal power.</param>
	public void OnCatPowerChange(int currentPower)
	{
		if (OnCatPowerChanged != null)
		{
			OnCatPowerChanged(currentPower);
		}
	}

	/// <summary>
	/// Called when the script is loaded
	/// </summary>
	/// <exception cref="UnityException">Thrown if any terminal is unassociated.</exception>
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		//DontDestroyOnLoad(gameObject); // possibly not needed anymore
		if ((TerminalCamera == null) || (TerminalTraps == null) || (TerminalDoors == null) || (TerminalCats == null))
		{
			throw new UnityException("Error: Terminals not set up with HackerInterface!");
		}
	}

	/// <summary>
	/// Called when the game starts. Sets up the power changed events.
	/// </summary>
	private void Start()
	{
		TerminalCamera.OnPowerChanged += OnCatPowerChange;
		TerminalTraps.OnPowerChanged += OnTrapPowerChange;
		TerminalDoors.OnPowerChanged += OnDoorPowerChange;
		TerminalCats.OnPowerChanged += OnCameraPowerChange;
	}
}
