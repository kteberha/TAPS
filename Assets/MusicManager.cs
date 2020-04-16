using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource intro, loop1, loop2, outro;

    GameManager gm;
    float _totalTime;
    [SerializeField]bool shouldLoop = false;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        loop1.PlayScheduled(AudioSettings.dspTime + intro.clip.length + 0.2f);
        StartCoroutine(CheckForLoopChange());
    }

    IEnumerator CheckForLoopChange()
    {
        if (loop1.isPlaying)
        {
            if (shouldLoop)
            {
                loop1.loop = true;
            }
            if (loop1.time >= (loop1.clip.length - 0.2f) && !shouldLoop)
            {
                print("play loop 2");
                loop2.Play();
                loop1.Stop();
                if (gm.workdayLength > 180)
                    shouldLoop = true;
            }
        }
        else if(loop2.isPlaying)
        {
            if (gm.workdayLength - gm.timeInWorkday < 50)
                shouldLoop = false;
            if(shouldLoop)
            {
                loop2.loop = true;
            }
            else if(loop2.time >= (loop2.clip.length - 0.2f))
            {
                outro.Play();
                loop2.Stop();
            }
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(CheckForLoopChange());
    }
}
