using UnityEngine;
using System.Collections;

/// <summary>
/// Door behavior base class for door opening and closing based on door terminal power.
/// </summary>
public class DoorBehavior : MonoBehaviour
{
	public int DoorLevel
	{
		get
		{
			return doorLevel;
		}
	}

	protected bool open = false;
	protected int doorLevel = 1;

	/// <summary>
	/// This is called during Start(). It is meant to be overridden
	/// </summary>
	protected virtual void Init()
	{
	}

	/// <summary>
	/// This is called when the door terminal power changes above the door level.
	/// </summary>
	protected virtual void Close()
	{
	}
		
	/// <summary>
	/// This is called when the door terminal power changes above the door level.
	/// </summary>
	protected virtual void Open()
	{
	}

	private void Start()
	{
		HackerInterface.Instance.OnDoorPowerChanged += OnDoorPowerChanged;
		Init();
	}

	/// <summary>
	/// Sets the power and calls Open() or Close() as necessary.
	/// </summary>
	/// <param name="newDoorPower">New door power.</param>
	private void OnDoorPowerChanged(int newDoorPower)
	{
		if ((doorLevel > newDoorPower) && open)
		{
			open = false;
			Close();
		}
		else if ((doorLevel <= newDoorPower) && !open)
		{
			open = true;
			Open();
		}
	}
}
