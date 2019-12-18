using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroids : MonoBehaviour
{
    public GameObject player;
    public float breakThreshold = 10f;
    public float playCoolDown = 0.5f;
    public AudioClip breakableResist;
    private float coolDownClock = 0f;
    private AudioSource audioSource;
    private ParticleSystem partSystem;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(gameObject.name == "Breakable")
        {
            partSystem = transform.Find("Particle").GetComponent<ParticleSystem>();
            partSystem.Stop();
            meshRenderer = transform.Find("Astroid").GetComponent<MeshRenderer>();
            print("found everything");
        }
    }

    private void Update()
    {
        coolDownClock -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && coolDownClock <= 0)
        {
            if (gameObject.name == "Breakable" && player.GetComponent<Rigidbody2D>().velocity.magnitude >= breakThreshold)
            {
                StartCoroutine(Shatter());
                print("shatter");
            }
            else if(gameObject.name == "Breakable" && player.GetComponent<Rigidbody2D>().velocity.magnitude < breakThreshold)
            {
                audioSource.PlayOneShot(breakableResist);
                coolDownClock = playCoolDown;
            }
            else
            {
                audioSource.Play();
                coolDownClock = playCoolDown;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && coolDownClock <= 0)
        {
            if (gameObject.name == "Breakable" && player.GetComponent<Rigidbody2D>().velocity.magnitude >= breakThreshold)
            {
                StartCoroutine(Shatter());
                print("shatter");
            }
            else if (gameObject.name == "Breakable" && player.GetComponent<Rigidbody2D>().velocity.magnitude < breakThreshold)
            {
                audioSource.PlayOneShot(breakableResist);
                coolDownClock = playCoolDown;
            }
            else
            {
                audioSource.Play();
                coolDownClock = playCoolDown;
            }
        }
    }

    IEnumerator Shatter()
    {
        audioSource.Play();
        partSystem.Play();
        meshRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitWhile(IsEmitting);

        Destroy(gameObject);
    }

    bool IsEmitting()
    {
        return partSystem.isEmitting;
    }
}
