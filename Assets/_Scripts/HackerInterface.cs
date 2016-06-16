using UnityEngine;
using System.Collections;

/// <summary>
/// Hacker-mouse's interface.
/// </summary>
public class HackerInterface : MonoBehaviour
{
	/// <summary>
	/// The singleton instance of the interface.
	/// </summary>
	public static HackerInterface Instance = null;

	/// <summary>
	/// The array of hacking terminals
	/// </summary>
	public HackerTerminal[] Terminals;

	/// <summary>
	/// Called when the script is loaded
	/// </summary>
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
		if (Terminals.Length == 0)
		{
			throw new UnityException("Error: Terminals not set up with HackerInterface!");
		}
	}
}
