using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SpriteRenderer))]
public class TimerDigit : MonoBehaviour
{
	public int CurrentDigit
	{
		get
		{
			return currentDigit;
		}
		set
		{
			currentDigit = value;
			SetSpriteDigit();
		}
	}

	public Sprite Digit0Sprite = null;
	public Sprite Digit1Sprite = null;
	public Sprite Digit2Sprite = null;
	public Sprite Digit3Sprite = null;
	public Sprite Digit4Sprite = null;
	public Sprite Digit5Sprite = null;
	public Sprite Digit6Sprite = null;
	public Sprite Digit7Sprite = null;
	public Sprite Digit8Sprite = null;
	public Sprite Digit9Sprite = null;
	public Sprite DigitEmptySprite = null;

	/// <summary>
	/// The current digit. If this is anything other than 0-9 it will use DigitEmptySprite.
	/// </summary>
	private int currentDigit = -1;

	private void SetSpriteDigit()
	{
		switch (currentDigit)
		{
			case 0:
				GetComponent<SpriteRenderer>().sprite = Digit0Sprite;
				break;
			case 1:
				GetComponent<SpriteRenderer>().sprite = Digit1Sprite;
				break;
			case 2:
				GetComponent<SpriteRenderer>().sprite = Digit2Sprite;
				break;
			case 3:
				GetComponent<SpriteRenderer>().sprite = Digit3Sprite;
				break;
			case 4:
				GetComponent<SpriteRenderer>().sprite = Digit4Sprite;
				break;
			case 5:
				GetComponent<SpriteRenderer>().sprite = Digit5Sprite;
				break;
			case 6:
				GetComponent<SpriteRenderer>().sprite = Digit6Sprite;
				break;
			case 7:
				GetComponent<SpriteRenderer>().sprite = Digit7Sprite;
				break;
			case 8:
				GetComponent<SpriteRenderer>().sprite = Digit8Sprite;
				break;
			case 9:
				GetComponent<SpriteRenderer>().sprite = Digit9Sprite;
				break;
			default:
				GetComponent<SpriteRenderer>().sprite = DigitEmptySprite;
				break;
		}
	}
}
