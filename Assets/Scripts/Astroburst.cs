using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroburst : MonoBehaviour
{
    public GameObject blackHole;

    PointEffector2D pointEffector;
    MeshRenderer meshRend;
    bool triggered = false;

    public float delayTime = 0.5f;
    public float bHChance = 0.1f;

    static bool firstBurst = true;

    //attempt to make the particles play at the right time
    public ParticleSystem[] CollisionParticles = new ParticleSystem[6];
    public ParticleSystem pCont;

    private void Start()
    {
        pointEffector = GetComponent<PointEffector2D>();
        pointEffector.enabled = false;

        //make sure all the explosion particles aren't playing
        foreach (ParticleSystem _p in CollisionParticles)
            _p.Stop();

        //start the continuous particle effect
        pCont.Play();
        meshRend = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// trigger the explosion if it is hit by a package or player
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Package"))
        {
            if (!triggered)
            {
                triggered = true;
                StartCoroutine(Explode());
            }
        }
    }

    /// <summary>
    /// Coroutine that handles the exploding logic
    /// </summary>
    /// <returns></returns>
    IEnumerator Explode()
    {
        //wait the desired time before detonating
        yield return new WaitForSeconds(delayTime);

        //enable the point effector to force objects away
        pointEffector.enabled = true;
        //stop the continuous particle effect
        pCont.Stop();

        //go through the explosion particle effects and play them
        foreach (ParticleSystem _p in CollisionParticles)
            _p.Play();

        //make the star mesh disappear
        meshRend.enabled = false;
        //play the explosion sound
        GetComponent<AudioSource>().Play();

        //wait for a second before disabling the collider so that the force can be applied
        yield return new WaitForSeconds(1);

        //disable colliders so force doesn't get added by accident when the mesh is gone
        GetComponent<CircleCollider2D>().enabled = false;

        //wait until the particle effect is done emitting to call any more code
        yield return new WaitWhile(ParticleEmitting);

        //determine if a black hole should appear or not. The first star exploding will never be a black hole
        if (!firstBurst)
        {
            float bHCreation = Random.value;
            if (bHChance >= bHCreation)
            {
                Instantiate(blackHole, transform.position, transform.rotation);
            }
        }
        else
        {
            firstBurst = false;
        }

        //destroy the astroburst asset
        Destroy(gameObject);
    }

    /// <summary>
    /// Returns true as long as the particle effect that plays the longest is emitting
    /// </summary>
    /// <returns></returns>
    bool ParticleEmitting()
    {
        return CollisionParticles[5].isEmitting;
    }
}
