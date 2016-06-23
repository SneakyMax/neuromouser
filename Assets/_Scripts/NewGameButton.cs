using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Scripts
{
	[UnityComponent]
	public class NewGameButton : MonoBehaviour
	{
		[AssignedInUnity]
		public LoadLevelProxy LoadLevelProxyPrefab;

		[CalledFromUnity]
		public void Load()
		{
			var levelName = "**new game**";

			if (String.IsNullOrEmpty(levelName))
				return;

			var instance = Instantiate(LoadLevelProxyPrefab.gameObject);

			instance.GetComponent<LoadLevelProxy>().LevelName = levelName;

			SceneManager.LoadScene(1);
		}
	}
}