using UnityEngine;
using System.Collections;

public class HackerInterface : MonoBehaviour
{
	public static HackerInterface instance = null;
	public HackerTerminal[] terminals; // array of hacking terminals

	// Use this for initialization
	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
		if (terminals.Length == 0)
		{
			throw new UnityException("Error: Terminals not set up with HackerInterface!");
		}
	}

	private void OnLevelWasLoaded(int loadedLevel)
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}


}
