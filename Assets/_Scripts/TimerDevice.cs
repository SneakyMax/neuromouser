using UnityEngine;
using System.Collections;

public class TimerDevice : MonoBehaviour
{
	public delegate void OnTimerZero();

	public event OnTimerZero TimerZero;

	public float SecondsLeft = 0f;

	public bool TimerRunning = false;

	public TimerDigit SecondsOnes = null;
	public TimerDigit SecondsTens = null;
	public TimerDigit MinutesOnes = null;
	public TimerDigit MinutesTens = null;

	/// <summary>
	/// Sets the timer digits based on SecondsLeft.
	/// </summary>
	private void SetTimerDigits()
	{
		int secondsLeft = Mathf.FloorToInt(SecondsLeft);

		if ( secondsLeft >= 3600 )
		{
			SecondsOnes.CurrentDigit = 9;
			SecondsTens.CurrentDigit = 5;
			MinutesOnes.CurrentDigit = 9;
			MinutesTens.CurrentDigit = 5;
		}
		else
		{
			int minutesLeft = secondsLeft / 60;
			MinutesTens.CurrentDigit = minutesLeft / 10;
			MinutesOnes.CurrentDigit = minutesLeft % 10;
			SecondsTens.CurrentDigit = ( ( secondsLeft % 60 ) / 10 );
			SecondsOnes.CurrentDigit = ( ( secondsLeft % 60 ) % 10 );
		}
	}

	private void Update()
	{
		if (TimerRunning)
		{
			SecondsLeft -= Time.deltaTime;
			if ( SecondsLeft < 0f )
			{
				SecondsLeft = 0f;
				TimerRunning = false;
				if ( TimerZero != null )
				{
					TimerZero();
				}
			}
		}
		SetTimerDigits();
	}
}
