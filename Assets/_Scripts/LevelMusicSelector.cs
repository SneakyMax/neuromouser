using UnityEngine;
using System.Collections;

public class LevelMusicSelector : MonoBehaviour {

	[FMODUnity.EventRef]
	public string MusicTrackOne = "event:/Track_1";
	public string MusicTrackThree = "event:/Track_3";

	// Use this for initialization
	void Start () {
		if (UnityEngine.Random.value > 0.5f) 
		{
			FMODUnity.RuntimeManager.PlayOneShot (MusicTrackOne, transform.position);
		}
		if (UnityEngine.Random.value < 0.5f) 
		{
				FMODUnity.RuntimeManager.PlayOneShot (MusicTrackThree, transform.position);
		}	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
