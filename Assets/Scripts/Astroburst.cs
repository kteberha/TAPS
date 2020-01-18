using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroburst : MonoBehaviour
{
    public GameObject blackHole;

    PointEffector2D pointEffector;
    ParticleSystem partSystem;
    MeshRenderer meshRend;
    bool partIsEmitting = false;

    public float delayTime = 0;

    private void Start()
    {
        pointEffector = GetComponent<PointEffector2D>();
        pointEffector.enabled = false;
        partSystem = GetComponent<ParticleSystem>();
        partSystem.Stop();
        meshRend = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(delayTime);

        pointEffector.enabled = true;
        partSystem.Play();
        meshRend.enabled = false;
        GetComponent<AudioSource>().Play();

        yield return new WaitWhile(ParticleDone);

        Instantiate(blackHole, transform.position, transform.rotation);
        print("black hole");

        Destroy(gameObject);
    }

    bool ParticleDone()
    {
        return partSystem.isEmitting;
    }
}
