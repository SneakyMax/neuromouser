using UnityEngine;
using System.Collections;

/// <summary>
/// This script is used to toggle power of and on to the resources
/// </summary>
public class PowerToggle : MonoBehaviour
{
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PowerToggle"/> power on status.
	/// </summary>
	/// <value><c>true</c> if power on status; otherwise, <c>false</c>.</value>
	public bool PowerOnStatus
	{
		get
		{
			return powerOn;
		}
		set
		{
			if ( value == true )
			{
				powerOn = true;
				GetComponent<SpriteRenderer>().sprite = PowerOnSprite;
			}
			else
			{
				powerOn = false;
				GetComponent<SpriteRenderer>().sprite = PowerOffSprite;
			}
		}
	}

	/// <summary>
	/// The power on sprite.
	/// </summary>
	public Sprite PowerOnSprite;

	/// <summary>
	/// The power off sprite.
	/// </summary>
	public Sprite PowerOffSprite;

	/// <summary>
	/// Is the power on?
	/// </summary>
	private bool powerOn = false;
}
