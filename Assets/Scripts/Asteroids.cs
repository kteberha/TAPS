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

    public Camera m_Camera;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        m_Camera = Camera.main;
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
        if (collision.transform.CompareTag("Player") && coolDownClock <= 0)//check that the player is hitting the asteroid and it has not recently hit it.
        {
            if (gameObject.name.Contains("Breakable")) && player.GetComponent<Rigidbody2D>().velocity.magnitude >= breakThreshold && broken == false)//conditions for shattering the asteroid
            {
                broken = true;//trigger one time bool
                StartCoroutine(Shatter());
            }
            else if (gameObject.name == "Breakable" && player.GetComponent<Rigidbody2D>().velocity.magnitude < breakThreshold)//conditions for breakable asteroid to play sound but not break.
            {
                audioSource.PlayOneShot(breakableResist);
                coolDownClock = playCoolDown;//set the cooldown clock so that sounds don't spawn relentlessly.
            }
            else
            {
                audioSource.Play();
                coolDownClock = playCoolDown;//set the cooldown clock so that sounds don't spawn relentlessly.
            }
        }
    }

    /// <summary>
    /// Breaks the asteroids into pieces and destroys the parent object
    /// </summary>
    /// <returns></returns>
    IEnumerator Shatter()
    {
        audioSource.Play();//play the audio cue
        foreach (ParticleSystem particle in partSystems)//play each of the particle systems
        {
            particle.transform.LookAt(m_Camera.transform.position);
            particle.Play();
        }
        meshRenderer.enabled = false;//make the mesh invisible
        GetComponent<Collider2D>().enabled = false;//disable the collider

        //spawn new non-breakable asteroids
        for (int i = 0; i < asteroidSpawnCount; i++)
        {
            float randX = Random.Range(-5, 5);//give small range on x to spawn away from origin
            float randY = Random.Range(-5, 5);//give small range on y to spawn away from origin
            Transform trans = transform;
            trans.position = new Vector3(trans.position.x + randX, trans.position.y + randY, trans.position.z);//give mini asteroid new altered transform from parent asteroid.

            GameObject temp = Instantiate(asteroidSpawn, trans.position, trans.rotation);//spawns new asteroid at the new position
            temp.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);//adjust the size of the smaller asteroids
            temp.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), 0));//give the asteroids a slight push
        }

        yield return new WaitWhile(IsEmitting);

        Destroy(gameObject);//destroy the parent object after the particle systems are done playing
    }

    bool IsEmitting()
    {
        foreach (ParticleSystem particle in partSystems)
            if (particle.isEmitting)
                return true;
        return false;
    }
}
