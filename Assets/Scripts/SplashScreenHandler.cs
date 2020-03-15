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
        if (Input.GetKeyDown(KeyCode.Escape) && !skipped)
        {
            skipped = true;
            animator.Play("SplashScreenAnim", 0, 0.96f);

            audioSource2.Stop();
            audioSource1.Stop();
            audioSource2.Play();
        }
    }
}
