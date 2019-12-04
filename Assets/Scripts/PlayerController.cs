﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Menu UI References
    [SerializeField]
    GameObject UICanvas;
    MenuController menuController;

    private Rigidbody2D rb;
    public List<GameObject> heldPackages = new List<GameObject>();
    
    public float playerSpeed = 2.5f;
    public float maxSpeed = 10;
    //public float playerDamping = 1f;
    Vector3 offset;

    public float shootForce = 10f;
    public float shootCooldown = 1f;
    private float shootCooldownClock = 0f;
    public float shootPackageCollisionImmuneTime = 0.5f;

    public float inventoryDistance = 5f;
    public float inventoryDampingRatio = 1f;
    public float inventoryFrequency = 0.5f;

    ParticleSystem mainStreamSys;
    ParticleSystem splatterSys;
    ParticleSystem burstSys;
    bool burstPlayed = false;

    //variables for the lineRenderer
    public LineRenderer lineRenderer;
    private float counter;
    private float dist = 0f;
    public float lineDrawSpeed = 6f;

    //Audio Variables
    private AudioSource P_audioSource;
    public AudioClip extinguisherStart;
    public AudioClip extinguisherLoop;
    public AudioClip extinguisherEnd;
    public AudioClip throwSound;
    public AudioClip collisionSound;

    //Teleporter variables
    public GameObject teleporter;
    private Transform teleportTransform;
    public float teleportCooldown;

    private Camera mainCamera;

    //Animator variables
    public GameObject playerModel;
    Animator animator;
    public float hitSpeed;


    // Start is called before the first frame update
    void Start()
    {
        menuController = UICanvas.GetComponent<MenuController>();

        rb = GetComponent<Rigidbody2D>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        lineRenderer = GetComponent<LineRenderer>();

        P_audioSource = GetComponent<AudioSource>();

        teleportTransform = teleporter.transform;

        mainStreamSys = transform.Find("foam_FX").Find("mainstream_Part").GetComponent<ParticleSystem>();
        splatterSys = transform.Find("foam_FX").Find("splatter_Part").GetComponent<ParticleSystem>();
        burstSys = transform.Find("foam_FX").Find("wideburst_Part").GetComponent<ParticleSystem>();

        animator = playerModel.GetComponent<Animator>();

       //for damping the player character's rotation when it collides with an object. see LateUpdate () for the remainder of the code that affects this.
        /*offset = transform.position - playerModel.transform.position;*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!menuController.paused)
        {
            //variables to trigger fire extinguisher particle effects
            var mainEmission = mainStreamSys.emission;
            var splatEmission = splatterSys.emission;

            if (Input.GetMouseButton(0) || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                Propulsion();

                //setting the particle system to play when propulsion is occuring
                mainEmission.enabled = true;
                splatEmission.enabled = true;

                if (burstPlayed == false)
                {
                    burstSys.Emit(15);
                    burstPlayed = true;
                }

            }
            else
            {
                //setting particle systems to "stop" when the player isnt using input.
                mainEmission.enabled = false;
                splatEmission.enabled = false;

                burstPlayed = false;
            }

            if (Input.GetKeyUp("mouse 1") && heldPackages.Count > 0 && shootCooldownClock <= 0)
            {
                Shoot();
                shootCooldownClock = shootCooldown;
            }

            //Teleport on given key down
            if (Input.GetKeyDown(KeyCode.R))
            {
                Teleport();
            }

            //Clocks
            shootCooldownClock -= Time.deltaTime;

            //Check if player has packages to draw lines to
            if (heldPackages.Count > 0)
            {
                //Set the Origin of the line renderer to the player position
                lineRenderer.SetPosition(0, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 20f));

                //Set the positions of the following line render points to all of the transforms in the heldPackages list
                for (int i = 0; i <= heldPackages.Count - 1; i++)
                {

                    if (i < heldPackages.Count - 1)
                    {
                        //determine distance from player to package at the end of the line
                        dist = Vector3.Distance(heldPackages[i].transform.position, heldPackages[i + 1].transform.position);
                    }


                    if (counter < dist)
                    {
                        counter += 0.1f / lineDrawSpeed;

                        float x = Mathf.Lerp(0, dist, counter);

                        //make sure it doesn't try to draw to a non existant box beyond the end of the list
                        if (i < heldPackages.Count - 1)
                        {
                            Vector3 pointA = heldPackages[i].transform.position;
                            Vector3 pointB = heldPackages[i + 1].transform.position;

                            //Get the unit vector in the desired direction, multiply by the desired length and add the starting point.
                            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
                        }
                    }

                    lineRenderer.SetPosition(i + 1, heldPackages[i].transform.position);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine("StartExtinguisherSound");
            }

            if (Input.GetMouseButtonUp(0))
            {
                //stop fire extinguisher loop
                StopCoroutine("StartExtinguisherSound");

                P_audioSource.loop = false;
                P_audioSource.clip = extinguisherEnd;
                P_audioSource.Play();

                //play idle animation
                animator.SetBool("IsFlying", false);
            }
        }
    }

    void Propulsion()
    {
        Vector3 playerPos = this.transform.position; //player position
        Vector3 mousePos = Input.mousePosition; //mouse position
        mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.z * -1));

        float mousePosX = mousePos.x - playerPos.x;//gets the distance between object and mouse position for x        
        float mousePosY = mousePos.y - playerPos.y;//gets the distance between object and mouse position for y;

        Vector2 dir = new Vector2(mousePosX, mousePosY);
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        dir = dir.normalized * -1;

        this.rb.AddForce(dir * playerSpeed);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        //animation adjustments
        animator.SetBool("IsFlying", true);
    }

    void Shoot()
    {
        if (heldPackages.Count > 0)
        {
            //turn off inventory bubble
            heldPackages[0].gameObject.GetComponent<Package>().Throw();

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0f;
            mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.z * -1));

            Vector2 heading = mousePos - heldPackages[0].transform.position;
            float distance = heading.magnitude;
            Vector2 direction = heading / distance;

            Destroy(heldPackages[0].GetComponent<SpringJoint2D>());
            heldPackages[0].GetComponent<BoxCollider2D>().usedByEffector = true;
            heldPackages[0].GetComponent<Package>().DoNotCollideWithPlayer(shootPackageCollisionImmuneTime);
            heldPackages[0].GetComponent<Rigidbody2D>().AddForce(direction * shootForce);            
            heldPackages.Remove(heldPackages[0]);

            //cue throwing sound
            P_audioSource.PlayOneShot(throwSound, 0.5f);

            if (heldPackages.Count > 0)
            {
                heldPackages[0].GetComponent<SpringJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
            }

            //adjust the lineRenderer positions array so that it doesn't keep drawing to a thrown box
            lineRenderer.positionCount = heldPackages.Count + 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.CompareTag("Package"))
        {
            P_audioSource.PlayOneShot(collisionSound, .75f); //play collision audio

            //only play the hit animation when zip hits not a package at a certain speed
            if(rb.velocity.magnitude > hitSpeed)
            {
                animator.SetTrigger("Hit");
            }
            
        }
        if (other.gameObject.CompareTag("Package"))
        {
            if (!heldPackages.Contains(other.gameObject))
            {
                //turn on inventory bubble
                other.gameObject.GetComponent<Package>().Pickup();

                SpringJoint2D rope = other.gameObject.AddComponent<SpringJoint2D>();
                int num = heldPackages.Count;
                if (num == 0)
                {
                    rope.connectedBody = GetComponent<Rigidbody2D>();
                }
                else
                {
                    rope.connectedBody = heldPackages[num - 1].GetComponent<Rigidbody2D>();
                }
                rope.autoConfigureDistance = false;
                rope.distance = inventoryDistance;
                rope.dampingRatio = inventoryDampingRatio;
                rope.frequency = inventoryFrequency;

                heldPackages.Add(other.gameObject);
            }
            else
            {
                //print(other.gameObject.name);
            }

            //tell the line renderer to draw to the newest added point
            lineRenderer.positionCount = heldPackages.Count + 1;
        }
    }

    IEnumerator StartExtinguisherSound()
    {
        //set & play fire extinguisher start sound
        P_audioSource.clip = extinguisherStart;
        P_audioSource.Play();

        yield return new WaitForSeconds(P_audioSource.clip.length);

        //set & play loop sound after setting it to loop
        P_audioSource.clip = extinguisherLoop;
        P_audioSource.loop = true;
        P_audioSource.Play();
    }

    /// <summary>
    /// this teleports the player to a fixed location
    /// </summary>
    public void Teleport()
    {
        //remove all packages from the player inventory, remove the bubble, and update line renderer
        for (int i = 0; i < heldPackages.Count; i++)
        {
            heldPackages[i].GetComponent<Package>().Throw();
            Destroy(heldPackages[i].GetComponent<SpringJoint2D>());
        }
        heldPackages.Clear();
        lineRenderer.positionCount = heldPackages.Count;

        rb.velocity = new Vector2(0f,0f); // set velocity to 0 to discontiue movement
        this.transform.position = teleportTransform.position; // set player position to the teleporter's
    }

/*    private void LateUpdate()
    {
        Vector3 desiredPosition = playerModel.transform.position + offset;
        transform.position = desiredPosition;
        Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * playerDamping);
        transform.position = position;

        transform.LookAt(playerModel.transform.position);
    }
*/
    /// <summary>
    /// this was supposed to make the player character teleport when it collided with the wall, but it didn't work for some reason. See boundaries script for my temp solution. -Emma
    /// </summary>
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.tag == "Wall")
    //    {
    //        print("Triggered");
    //        Teleport();
    //    }
    //}
}