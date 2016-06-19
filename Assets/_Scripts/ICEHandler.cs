using System;
using UnityEngine;
using System.Collections;
using Assets._Scripts;

/// <summary>
/// Handles ICEs.
/// </summary>
public class ICEHandler : MonoBehaviour
{
	/// <summary>
	/// Power level callback delegate for the OnPowerChange event
	/// </summary>
	public delegate void PowerLevelCallback(int currentPowerLevel);

	/// <summary>
	/// Fires when the power changes settings.
	/// </summary>
	public event PowerLevelCallback OnPowerChange;

    /// <summary>Called when the raw power number changes.</summary>
    public event Action<int> PowerChanged;

	/// <summary>
	/// Gets the power level.
	/// </summary>
	/// <value>The power level.</value>
	public int PowerLevel
	{
		get
		{
			return currentPowerLevel;
		}
	}

	/// <summary>
	/// The max power.
	/// </summary>
	public float MaxPower = 16.0f;

	/// <summary>The level1 power minimum.</summary>
	public int Level1PowerMin = 4;

	/// <summary>
	/// The level2 power minimum.
	/// </summary>
	public int Level2PowerMin = 8;

	/// <summary>
	/// The level3 power minimum.
	/// </summary>
	public int Level3PowerMin = 12;

	/// <summary>
	/// The max y value for positioning. Will reach this at MaxPower.
	/// </summary>
	public float MaxY = 0f;

	/// <summary>
	/// The level0 power decay per second.
	/// </summary>
	public float Level0PowerDecay = -0.5f;

	/// <summary>
	/// The level1 power decay per second.
	/// </summary>
	public float Level1PowerDecay = -1f;

	/// <summary>
	/// The level2 power decay per second.
	/// </summary>
	public float Level2PowerDecay = -2f;

	/// <summary>
	/// The level3 power decay per second.
	/// </summary>
	public float Level3PowerDecay = -3f;

	/// <summary>
	/// The current power.
	/// </summary>
	public float CurrentPower = 0.0f;

	/// <summary>
	/// The current power level.
	/// </summary>
	private int currentPowerLevel = 0;

	/// <summary>
	/// The origin.
	/// </summary>
	private Vector3 origin;

	/// <summary>
	/// This Start() sets up the origin.
	/// </summary>
	private void Start()
	{
		origin = transform.position;
	}

    public void AbsorbPower(float power)
    {
        CurrentPower = Mathf.Min(MaxPower, CurrentPower + power);
    }

	/// <summary>
	/// Changes the power level and updates subscribers to OnPowerChange.
	/// </summary>
	/// <param name="newPowerLevel">New power level.</param>
	private void ChangePowerLevel(int newPowerLevel)
	{
		currentPowerLevel = newPowerLevel;
		if (OnPowerChange != null)
			OnPowerChange(currentPowerLevel);
	}

	/// <summary>
	/// Updates the power level if needed.
	/// </summary>
	private void UpdatePowerLevel()
	{
		if (CurrentPower >= Level3PowerMin)
		{
			if (currentPowerLevel != 3)
				ChangePowerLevel(3);
		}
		else if (CurrentPower >= Level2PowerMin)
		{
			if (currentPowerLevel != 2)
				ChangePowerLevel(2);
		}
		else if (CurrentPower >= Level1PowerMin )
		{
			if (currentPowerLevel != 1)
				ChangePowerLevel(1);
		}
		else
		{
			if (currentPowerLevel != 0)
				ChangePowerLevel(0);			
		}
	}

	/// <summary>
	/// Handles the power decay.
	/// </summary>
	private void HandlePowerDecay()
	{
		if ( CurrentPower >= Level3PowerMin )
			CurrentPower += ( Time.deltaTime * Level3PowerDecay );
		else if ( CurrentPower >= Level2PowerMin )
			CurrentPower += ( Time.deltaTime * Level2PowerDecay );
		else if ( CurrentPower >= Level1PowerMin )
			CurrentPower += ( Time.deltaTime * Level1PowerDecay );
		else
		{
			CurrentPower += ( Time.deltaTime * Level0PowerDecay );
		}

		if ( CurrentPower < float.Epsilon )
			CurrentPower = 0f;

		UpdatePowerLevel();
	}

	/// <summary>
	/// Sets the position of the ICE every frame.
	/// </summary>
	private void FixedUpdate()
	{
        //if(CurrentPower < MaxPower)
	        //CurrentPower += 0.2f;
		HandlePowerDecay();
		//transform.position = new Vector3(origin.x, origin.y + ((CurrentPower / MaxPower) * MaxY));
	}
}
