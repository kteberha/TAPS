using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource2;

    private void Awake()
    {
        audioSource2.PlayScheduled(AudioSettings.dspTime + 19.85);
    }
}
