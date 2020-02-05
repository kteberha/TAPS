using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroids : MonoBehaviour
{
    public GameObject player, asteroidSpawn;
    public float breakThreshold = 10f;
    public float playCoolDown = 0.5f;
    public AudioClip breakableResist;

    private float coolDownClock = 0f;
    private AudioSource audioSource;
    [SerializeField]private ParticleSystem[] partSystems = new ParticleSystem[2];
    [SerializeField]private MeshRenderer meshRenderer;
    private int asteroidSpawnCount = 2;
    private bool broken = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        coolDownClock -= Time.deltaTime;
    }

    /// <summary>
    /// Check for collisions
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && coolDownClock <= 0)
        {
            if (gameObject.name == "Breakable" && player.GetComponent<Rigidbody2D>().velocity.magnitude >= breakThreshold && broken == false)
            {
                broken = true;
                StartCoroutine(Shatter());
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

    /// <summary>
    /// Breaks the asteroids into pieces and destroys the parent object
    /// </summary>
    /// <returns></returns>
    IEnumerator Shatter()
    {
        audioSource.Play();
        foreach (ParticleSystem particle in partSystems)
            particle.Play();
        meshRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        //spawn new non-breakable asteroids
        for (int i = 0; i < asteroidSpawnCount; i++)
        {
            float randX = Random.Range(-5, 5);
            float randY = Random.Range(-5, 5);
            Transform trans = transform;
            trans.position = new Vector3(trans.position.x + randX, trans.position.y + randY, trans.position.z);

            GameObject temp = Instantiate(asteroidSpawn, trans.position, trans.rotation);
            temp.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            temp.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), 0));
        }

        yield return new WaitWhile(IsEmitting);

        Destroy(gameObject);
    }

    bool IsEmitting()
    {
        foreach (ParticleSystem particle in partSystems)
            if (particle.isEmitting)
                return true;
        return false;
    }
}
