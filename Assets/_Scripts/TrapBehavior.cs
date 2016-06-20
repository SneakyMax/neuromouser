using UnityEngine;
using System.Collections;

public class TrapBehavior : MonoBehaviour
{
	public int TrapLevel
	{
		get
		{
			return trapLevel;
		}
	}

	protected bool armed = false;
	protected int trapLevel = 1;

	/// <summary>
	/// This is called during Start(). It is meant to be overridden
	/// </summary>
	protected virtual void Init()
	{
	}

	/// <summary>
	/// This is called when the trap terminal power level changes below the trap level.
	/// </summary>
	protected virtual void Arm()
	{
	}
		
	/// <summary>
	/// This is called when the trap terminal power changes above the trap level.
	/// </summary>
	protected virtual void Disarm()
	{
	}

	private void Start()
	{
		HackerInterface.Instance.OnTrapPowerChanged += OnTrapPowerChanged;
		Init();
	}

	/// <summary>
	/// Sets the power and calls Arm() or Disarm() as necessary.
	/// </summary>
	/// <param name="newTrapPower">New trap power.</param>
	private void OnTrapPowerChanged(int newTrapPower)
	{
		if ((trapLevel > newTrapPower) && !armed)
		{
			trapLevel = newTrapPower;
			armed = true;
			Arm();
		}
		else if ((trapLevel < newTrapPower) && armed)
		{
			trapLevel = newTrapPower;
			armed = false;
			Disarm();
		}
	}
}
