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
        audioSource2.PlayScheduled(AudioSettings.dspTime + 19.85);
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
