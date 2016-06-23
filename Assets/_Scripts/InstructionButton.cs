using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Scripts
{
	public class InstructionButton : MonoBehaviour
	{
		[AssignedInUnity]
		public Sprite InstructionSprite;

		public Image MainImage;

		private Sprite mainSprite;

		private bool showInstructions = false;

		public void InstructionToggle()
		{
			if (showInstructions == true)
			{
				MainImage.sprite = mainSprite;
				showInstructions = false;
			}
			else
			{
				mainSprite = MainImage.sprite;
				MainImage.sprite = InstructionSprite;
				showInstructions = true;
			}
		}
	}
}