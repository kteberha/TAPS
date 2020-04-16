using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource intro, loop1, loop2, outro;

    GameManager gm;
    float _totalTime;
    bool shouldLoop = false;

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
            //print("time samples: " + loop1.timeSamples);
            //print("time: " + loop1.time);
            if(loop1.time >= (loop1.clip.length - 0.2f) && !shouldLoop)
            {
                print("play loop 2");
                loop2.Play();
                loop1.Stop();
            }
        }
        else if(loop2.isPlaying)
        {
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

    //IEnumerator JuggleMusic()
    //{
    //    //leave some time for the hype loop to repeat if day is long
    //    if(gm.workdayLength > 151)
    //    {
    //        print("long day for music");

    //        //start the hype section at 2 minute mark, make the loop exit at the 1 minute mark
    //        if (gm.workdayLength - gm.timeInWorkday <= 45)
    //            SwapClip(4);
    //        else if (gm.workdayLength - gm.timeInWorkday <= 121)
    //        {
    //            SwapClip(3);
    //            yield return new WaitForSeconds(30f);
    //            StartCoroutine(JuggleMusic());
    //        }
    //        else
    //        {
    //            SwapClip(Random.Range(0, 3));
    //            yield return new WaitForSeconds(30f);
    //            StartCoroutine(JuggleMusic());
    //        }

    //        }
    //    else
    //    {
    //        print("short day for music");
    //        if(gm.workdayLength - gm.timeInWorkday > 150)
    //        {
    //            SwapClip(Random.Range(0, 3));
    //            yield return new WaitForSeconds(30f);
    //            StartCoroutine(JuggleMusic());
    //        }
    //        else if (gm.workdayLength - gm.timeInWorkday <= 100)
    //        {
    //            SwapClip(3);
    //            StartCoroutine(JuggleMusic());
    //        }
    //        else if (gm.workdayLength - gm.timeInWorkday <= 45)
    //            SwapClip(4);
    //    }
    //}

    //void SwapClip(int clip)
    //{
    //    music.setParameterByName("LoopSection", clip);
    //    print("new clip #" + clip);
    //}

}
