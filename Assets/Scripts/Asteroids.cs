using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroids : MonoBehaviour
{
    public float playCoolDown = 0.5f;
    private float coolDownClock = 0f;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        coolDownClock -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && coolDownClock <= 0)
        {
            audioSource.Play();
            coolDownClock = playCoolDown;
        }
    }
}
