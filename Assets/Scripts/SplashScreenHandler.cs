using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenHandler : MonoBehaviour
{
    [SerializeField] Animator splashAnimator, cinematicAnimator;
    [SerializeField] AudioSource audioSource1, audioSource2;
    bool splashScreenOver = false;//determines if the splash screens are done/have been skipped


    bool skipped = false;

    private void Awake()
    {
        //cinematicAnimator.enabled = false;
        //splashAnimator.enabled = true;
        Time.timeScale = 1;
        //StartCoroutine(OpeningCine());
    }
    private void Start()
    {
        audioSource1.Play();
        audioSource2.PlayScheduled(AudioSettings.dspTime + audioSource1.clip.length);
    }

    /// <summary>
    /// Helper method to get animation clips
    /// </summary>
    /// <param name="_animator"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    AnimationClip GetAnimationClip(Animator _animator, string _name)
    {
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        AnimationClip startClip = null;
        foreach (AnimationClip c in clips)
            if (c.name == _name)
            {
                //print($"Clip: {_name} found");
                startClip = c;
                break;
            }
        if (startClip.name == null)
            Debug.LogWarning($"no clip with name: {_name} exists");

        return startClip;
    }

    private void Update()
    {
        if (!audioSource1.isPlaying)
        {
            skipped = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            if (!skipped)
                SkipIntro();
        }
    }

    //IEnumerator OpeningCine()
    //{
    //    yield return new WaitForSecondsRealtime(6f);
    //    print("swap animations");
    //    cinematicAnimator.enabled = true;
    //    splashAnimator.enabled = false;
    //}

    void SkipIntro()
    {
        skipped = true;
        splashAnimator.Play("SplashScreenAnim", 0, 0.96f);
        audioSource2.Stop();//cancel scheduled audio
        audioSource1.Stop();//stop the current audio
        audioSource2.Play();//play the looping audio

    }
}
