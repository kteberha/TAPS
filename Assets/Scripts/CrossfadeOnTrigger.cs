using UnityEngine;
using System.Collections;

public class CrossfadeOnTrigger : MonoBehaviour
{
	public AudioClip[] tracks;

	public float fadeTime = 1.0f;

	public int currentTrack = 0;

    public bool triggerMusic = false;
	
	// Update is called once per frame
	void Update ()
	{
		if(triggerMusic)
		{
			if(currentTrack >= tracks.Length)
			{
				currentTrack = 0;
			}
			MusicManager.Crossfade(tracks[currentTrack], fadeTime);

            triggerMusic = false;
		}
	}
}
