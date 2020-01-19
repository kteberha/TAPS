using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    public MenuController menuController;

    //Camera variables
    private Camera mainCamera;
    public float minZPosition;
    public float maxZPosition;
    public Transform dialogueCameraPoint;

    private Rigidbody2D rb;
    public List<GameObject> heldPackages = new List<GameObject>();

    //movement variables
    public float playerSpeed = 2.5f;
    public float maxSpeed = 10;
    //public float playerDamping = 1f;
    Vector3 offset;
    public float impulseForce;
    public float impulseCoolDown = 3f;
    private float impulseCoolDownClock = 0f;
    bool impulseReady = true;
    public GameObject movementPointer;

    //package throwing variables
    public int maxPackages = 5;
    public float shootForce = 10f;
    public float shootCooldown = 1f;
    private float shootCooldownClock = 0f;
    public float shootPackageCollisionImmuneTime = 0.5f;

    //inventory variables
    public float inventoryDistance = 5f;
    public float inventoryDampingRatio = 1f;
    public float inventoryFrequency = 0.5f;

    //particle system variables
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

    //Teleporter variables
    public GameObject teleporter;
    private Transform teleportTransform;
    public float teleportCooldown;

    //Animator variables
    public GameObject playerModel;
    Animator animator;
    public float hitSpeed;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        lineRenderer = GetComponent<LineRenderer>();

        P_audioSource = GetComponent<AudioSource>();

        teleportTransform = teleporter.transform;

        mainStreamSys = transform.Find("FireExtinguisher").Find("foam_FX").Find("mainstream_Part").GetComponent<ParticleSystem>();
        splatterSys = transform.Find("FireExtinguisher").Find("foam_FX").Find("splatter_Part").GetComponent<ParticleSystem>();
        burstSys = transform.Find("FireExtinguisher").Find("foam_FX").Find("wideburst_Part").GetComponent<ParticleSystem>();

        animator = playerModel.GetComponent<Animator>();

        //for damping the player character's rotation when it collides with an object. see LateUpdate () for the remainder of the code that affects this.
        /*offset = transform.position - playerModel.transform.position;*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!menuController.paused)
        {
            RotatePointer();

            //Clocks
            shootCooldownClock -= Time.deltaTime;
            impulseCoolDownClock -= Time.deltaTime;

            //variables to trigger fire extinguisher particle effects
            var mainEmission = mainStreamSys.emission;
            var splatEmission = splatterSys.emission;

            //When the left mouse button, or directional input is received
            if (Input.GetMouseButton(0) || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                //movement logic
                Propulsion();

                //setting the particle system to play when propulsion is occuring
                mainEmission.enabled = true;
                splatEmission.enabled = true;

                //play the burst particle effect if it hasn't been played yet
                if (burstPlayed == false && impulseReady == true)
                {
                    burstSys.Emit(15);
                    burstPlayed = true;
                    impulseReady = false;
                }
            }
            else
            {
                //set the impulse check to true if cooldown is complete
                if (impulseCoolDownClock < 0)
                    impulseReady = true;

                //setting particle systems to "stop" when the player isnt using input.
                mainEmission.enabled = false;
                splatEmission.enabled = false;


                burstPlayed = false;
            }

            //start fire extinguisher sound
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine("StartExtinguisherSound");
            }

            //Throw packages on right mouse button release
            if (Input.GetKeyUp("mouse 1") && heldPackages.Count > 0 && shootCooldownClock <= 0)
            {
                PackageShoot(heldPackages[0], shootForce);
                shootCooldownClock = shootCooldown;
            }

            PackageSwitch(Input.mouseScrollDelta.y);

            if (Input.GetKeyUp("mouse 2"))
            {
                PackageSwitch(-1);
            }

            //Teleport on given key down
            if (Input.GetKeyDown(KeyCode.T))
            {
                Teleport();
            }

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
        float mousePosY = mousePos.y - playerPos.y;//gets the distance between object and mouse position for y

        Vector2 dir = new Vector2(mousePosX, mousePosY);
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        //determine inverted movement controls and establish direction of travel
        if(menuController.invertedMovement)
            dir = dir.normalized * -1;
        else
            dir = dir.normalized;

        //check to see if movement impulse can be added
        if (burstPlayed == false && impulseReady == true)
        {
            this.rb.AddForce(dir * playerSpeed * impulseForce);
            impulseCoolDownClock = impulseCoolDown;
        }

        //constantly add force in the given direction
        this.rb.AddForce(dir * playerSpeed);

        //prevent player from going faster than maximum speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            print("velocity is over max");
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        //animation adjustments
        animator.SetBool("IsFlying", true);
    }

    void PackageShoot(GameObject packageMoved, float force)
    {
        if (heldPackages.Count > 0)
        {
            //turn off inventory bubble
            packageMoved.gameObject.GetComponent<Package>().Throw();

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0f;
            mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.z * -1));

            Vector2 heading = mousePos - packageMoved.transform.position;
            float distance = heading.magnitude;
            Vector2 direction = heading / distance;

            Destroy(packageMoved.GetComponent<SpringJoint2D>());
            packageMoved.GetComponent<BoxCollider2D>().usedByEffector = true;
            packageMoved.GetComponent<Package>().DoNotCollideWithPlayer(shootPackageCollisionImmuneTime);
            packageMoved.GetComponent<Rigidbody2D>().AddForce(direction * force);
            heldPackages.Remove(packageMoved);

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

    void PackageAdd(GameObject other, bool addToFront = false)
    {
        if (!heldPackages.Contains(other.gameObject) && heldPackages.Count < maxPackages)
        {
            //turn on inventory bubble
            other.gameObject.GetComponent<Package>().Pickup();

            SpringJoint2D rope = other.gameObject.AddComponent<SpringJoint2D>();
            if (!addToFront)
            {
                int num = heldPackages.Count;
                if (num == 0)
                {
                    rope.connectedBody = GetComponent<Rigidbody2D>();
                }
                else
                {
                    rope.connectedBody = heldPackages[num - 1].GetComponent<Rigidbody2D>();
                }
                heldPackages.Add(other.gameObject);
            }
            else
            {
                rope.connectedBody = GetComponent<Rigidbody2D>();
                heldPackages[0].GetComponent<SpringJoint2D>().connectedBody = other.GetComponent<Rigidbody2D>();

                heldPackages.Insert(0, other.gameObject);
            }
            rope.autoConfigureDistance = false;
            rope.distance = inventoryDistance;
            rope.dampingRatio = inventoryDampingRatio;
            rope.frequency = inventoryFrequency;
        }

        //tell the line renderer to draw to the newest added point
        lineRenderer.positionCount = heldPackages.Count + 1;
    }

    void PackageSwitch(float dir)
    {
        if (heldPackages.Count > 1)
        {
            GameObject packageMoved = null;
            if (dir < 0)
                packageMoved = heldPackages[0];
            else if (dir > 0)
            {
                Debug.Log("Scroll up!");
                packageMoved = heldPackages[heldPackages.Count - 1];
            }
            else
                return;

            //remove
            PackageShoot(packageMoved, 0);

            //add
            PackageAdd(packageMoved, dir > 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.CompareTag("Package"))
        {
            //only play the hit animation when zip hits not a package at a certain speed
            if(rb.velocity.magnitude > hitSpeed)
            {
                animator.SetTrigger("Hit");
            }

        }
        if (other.gameObject.CompareTag("Package"))
        {
            PackageAdd(other.gameObject);
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

    /// <summary>
    /// Handles moving the camera closer or further based on player's speed
    /// </summary>
    public float DetermineCameraZ()
    {
        //get the percentage of how fast the player is going compared to their max speed
        float lerpPerc = rb.velocity.magnitude / maxSpeed;
        //lerp to find new camera z position based on speed percentage
        float desiredZ = Mathf.Lerp(minZPosition, maxZPosition, lerpPerc);

        return desiredZ;
    }

    /// <summary>
    /// Rotates the direction indicator around the player based on desired input type.
    /// </summary>
    void RotatePointer()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        Vector3 rotateVector = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.position.y, 0f);
        if(menuController.invertedMovement)
            movementPointer.transform.rotation = Quaternion.LookRotation(-rotateVector, -Vector3.forward);
        else
            movementPointer.transform.rotation = Quaternion.LookRotation(rotateVector, -Vector3.forward);

    }
}
