using System;
using Assets._Scripts;
using UnityEngine;

public class ICEHandler : MonoBehaviour
{
	/// <summary>Fires when the power changes settings.</summary>
	public event Action<int> OnPowerLevelChange;

    /// <summary>Current power level - 0, 1, 2, 3</summary>
	public int PowerLevel {get { return currentPowerLevel; } }
    
	public int MaxPower = 16;

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

    /// <summary>Seconds between level 0 decay pulses.</summary>
    [Range(0, 5)]
    public float Level0PowerDecayRate = 3.0f;

    /// <summary>Seconds between level 1 decay pulses.</summary>
    [Range(0, 5)]
    public float Level1PowerDecayRate = 2.0f;

    /// <summary>Seconds between level 2 decay pulses.</summary>
    [Range(0, 5)]
    public float Level2PowerDecayRate = 1.0f;

    /// <summary>Seconds between level 3 decay pulses.</summary>
    [Range(0, 5)]
    public float Level3PowerDecayRate = 0.5f;
    
	public float CurrentPower;
    
	private int currentPowerLevel;
    
    [AssignedInUnity]
    public GameObject CounterPulsePrefab;

    private float timeUntilNextPulse;
    private float pulseTimeAccumulator;

    private PowerLevelIndicator powerLevelIndicator;

    public void AbsorbPower(float power)
    {
        CurrentPower = Mathf.Clamp(CurrentPower + power, 0, MaxPower);
    }

    [UnityMessage]
    public void Start()
    {
        powerLevelIndicator = GetComponent<PowerLevelIndicator>();

        ChangePowerLevel(0);
    }

	/// <summary>
	/// Changes the power level and updates subscribers to OnPowerChange.
	/// </summary>
	/// <param name="newPowerLevel">New power level.</param>
	private void ChangePowerLevel(int newPowerLevel)
	{
		currentPowerLevel = newPowerLevel;

        timeUntilNextPulse = newPowerLevel == 0 ?
            Level0PowerDecayRate : newPowerLevel == 1 ?
            Level1PowerDecayRate : newPowerLevel == 2 ?
            Level2PowerDecayRate : newPowerLevel == 3 ?
            Level3PowerDecayRate : 0;

	    pulseTimeAccumulator = 0;

		if (OnPowerLevelChange != null)
			OnPowerLevelChange(currentPowerLevel);
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

    [UnityMessage]
    public void Update()
    {
        UpdatePowerLevel();
        HandlePowerDecay();
    }
    
	private void HandlePowerDecay()
	{
	    if (timeUntilNextPulse < float.Epsilon)
	        return;

	    pulseTimeAccumulator += Time.deltaTime;

	    if (pulseTimeAccumulator >= timeUntilNextPulse)
	    {
	        CreatePulse();
	        pulseTimeAccumulator -= timeUntilNextPulse;
	    }
	}

    private void CreatePulse()
    {
        var pulse = Instantiate(CounterPulsePrefab);
        pulse.transform.position = powerLevelIndicator.CounterStartPosition;

        var counterPulse = pulse.GetComponent<CounterPulse>();
        counterPulse.ParentHandler = this;
    }

    public void Reset()
    {
        CurrentPower = 0;
    }
}
