using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroburst : MonoBehaviour
{
    public GameObject blackHole;

    public Material normalMaterial;
    public Material bHMaterial;

    PointEffector2D pointEffector;
    ParticleSystem partSystem;
    MeshRenderer meshRend;
    bool partIsEmitting = false;
    bool triggered = false;

    public float delayTime = 0.5f;

    public bool spawnBH = false;

    private void Start()
    {
        pointEffector = GetComponent<PointEffector2D>();
        pointEffector.enabled = false;
        partSystem = GetComponent<ParticleSystem>();
        partSystem.Stop();
        meshRend = GetComponent<MeshRenderer>();
        //assign the material given to the astroburst depending on whether it will spawn a black hole
        if (spawnBH)
            meshRend.material = normalMaterial;
        else
            meshRend.material = bHMaterial;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!triggered)
            {
                triggered = true;
                StartCoroutine(Explode());
            }
        }
    }

    IEnumerator Explode()
    {
        //trigger color change to indicate imminent detonation
        Color astroColor = GetComponent<MeshRenderer>().material.color;
        astroColor = new Color(1,0,0);

        //wait the desired time before detonating
        yield return new WaitForSeconds(delayTime);

        pointEffector.enabled = true;
        partSystem.Play();
        meshRend.enabled = false;
        GetComponent<AudioSource>().Play();

        //wait for a second before disabling the collider so that the force can be applied
        yield return new WaitForSeconds(1);

        //disable colliders so force doesn't get added by accident when the mesh is gone
        GetComponent<CircleCollider2D>().enabled = false;

        yield return new WaitWhile(ParticleEmitting);

        if(spawnBH)
        {
            Instantiate(blackHole, transform.position, transform.rotation);
            print("black hole");
        }


        Destroy(gameObject);
    }

    bool ParticleEmitting()
    {
        return partSystem.isEmitting;
    }
}
