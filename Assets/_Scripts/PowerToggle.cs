using UnityEngine;
using System.Collections;

public class PowerToggle : MonoBehaviour
{
	private bool powerOn = false; // is the power on
	public bool PowerOnStatus // gets and sets the power on status
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
				GetComponent<SpriteRenderer>().sprite = powerOnSprite;
			}
			else
			{
				powerOn = false;
				GetComponent<SpriteRenderer>().sprite = powerOffSprite;
			}
		}
	}

	[SerializeField]
	private Sprite powerOnSprite; // the power on sprite
	[SerializeField]
	private Sprite powerOffSprite; // the power off sprite

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
