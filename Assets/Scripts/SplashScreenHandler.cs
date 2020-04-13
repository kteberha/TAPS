using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource1, audioSource2;

    bool skipped = false;
    private void Awake()
    {
        audioSource1.Play();
    }

    private void Start()
    {
        audioSource2.PlayScheduled(AudioSettings.dspTime + audioSource1.clip.length);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) && !skipped)
        {
            skipped = true;
            animator.Play("SplashScreenAnim", 0, 0.96f);

            audioSource2.Stop();//cancel scheduled audio
            audioSource1.Stop();//stop the current audio
            audioSource2.Play();//play the looping audio
        }
    }
}
