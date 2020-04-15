using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    //Camera variables
    private Camera mainCamera;
    public float minZPosition;
    public float maxZPosition;
    public Transform dialogueCameraPoint;
    public float lerpPerc;

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
    [SerializeField] ParticleSystem mainStreamSys, splatterSys, burstSys;
    bool burstPlayed = false;

    //variables for the lineRenderer
    public LineRenderer lineRenderer;
    private float counter;
    private float dist = 0f;
    public float lineDrawSpeed = 6f;

    //Audio Variables
    [SerializeField] private AudioSource ExtinguisherAudioSource;
    [SerializeField] private AudioSource ThrowAudioSource;
    [SerializeField] private AudioSource TeleportAudioSource;
    [SerializeField] private AudioSource PackageCollectAudioSource;
    [SerializeField] AudioClip extinguisherStart, extinguisherLoop, extinguisherEnd;
    [SerializeField] AudioClip[] inventoryAddClips;
    //[SerializeField] AudioClip[] inventorySortClips;

    //Teleporter variables
    public GameObject teleporter;
    private Transform teleportTransform;
    public float teleportCooldown;
    public float dissolveTime;
    [SerializeField] public Material face_mat;
    [SerializeField] public Material body_mat;
    [SerializeField] public Material fe_mat;

    //Animator variables
    public GameObject playerModel;
    Animator animator;
    public float hitSpeed;
    [SerializeField] FaceAnimation faceAnim;

    //collider variables
    [SerializeField] Collider2D[] allColliders;

    [Header("DEMO TUTORIAL")]
    [SerializeField] TutorialManager tutorialManager;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        lineRenderer = GetComponent<LineRenderer>();

        ExtinguisherAudioSource = GetComponent<AudioSource>();

        teleportTransform = teleporter.transform;

        animator = playerModel.GetComponent<Animator>();

        var mainStreamEmission = mainStreamSys.emission;
        var splatterStreamEmission = splatterSys.emission;
        mainStreamEmission.enabled = false;
        splatterStreamEmission.enabled = false;

        face_mat.SetFloat("_Vector1_AlphaClip", 0);
        body_mat.SetFloat("_Vector1_AlphaClip", 0);
        fe_mat.SetFloat("_Vector1_AlphaClip", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            gameObject.transform.rotation = new Quaternion(0,0,0,0);
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
        //if (!menuController.paused)//check the game isn't paused
        if (GameManager.state == GAMESTATE.CLOCKEDIN)
        {

            RotatePointer();//rotate the direction pointer

            //update the new flying VFX here

            ////


            //Clocks
            shootCooldownClock -= Time.deltaTime;
            impulseCoolDownClock -= Time.deltaTime;

            //variables to trigger fire extinguisher particle effects
            var mainEmission = mainStreamSys.emission;
            var splatEmission = splatterSys.emission;

            ///////////This is input stuff that will need adjusted for controller input/////////////
            #region

            if (Input.GetMouseButton(0) || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)//When the left mouse button, or directional input is received
            {
                Propulsion();//movement logic

                //setting the particle system to play when propulsion is occuring
                mainEmission.enabled = true;
                splatEmission.enabled = true;

                if (burstPlayed == false && impulseReady == true)//play the burst particle effect if it hasn't been played yet
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

            if (Input.GetMouseButtonUp(0))
            {
                //stop fire extinguisher loop
                StopCoroutine("StartExtinguisherSound");

                ExtinguisherAudioSource.loop = false;
                ExtinguisherAudioSource.clip = extinguisherEnd;
                ExtinguisherAudioSource.Play();

                //play idle animation
                animator.SetBool("IsFlying", false);
            }

            //Throw packages on right mouse button release
            if (Input.GetKeyUp("mouse 1") && heldPackages.Count > 0 && shootCooldownClock <= 0)
            {
                PackageShoot(heldPackages[0], shootForce);
                shootCooldownClock = shootCooldown;
            }

            //switch the order of the packages based on the mouse wheel
            PackageSwitch(Input.mouseScrollDelta.y);

            //switch the order of the packages based on mouse wheel press
            if (Input.GetKeyUp("mouse 2"))
            {
                PackageSwitch(-1);
            }

            //Teleport on given key down
            if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.T))
            {
                //Teleport();
                StartCoroutine(Teleport());
            }
            #endregion
            ///////////////////////////////////////////////////////////////////////////

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

                    //make sure there is a package to try and redraw to
                    //lineRenderer.positionCount > 0 put this back in if statement if plan doesn't work
                    if (i < lineRenderer.positionCount - 1)
                    {
                        lineRenderer.SetPosition(i + 1, heldPackages[i].transform.position);

                    }
                }
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

        if(MenuController.invertedMovement)//determine inverted movement controls and establish direction of travel

            dir = dir.normalized * -1;
        else
            dir = dir.normalized;

        if (burstPlayed == false && impulseReady == true)//check to see if movement impulse can be added
        {
            this.rb.AddForce(dir * playerSpeed * impulseForce);
            impulseCoolDownClock = impulseCoolDown;
        }

        this.rb.AddForce(dir * playerSpeed);//constantly add force in the given direction

        if (rb.velocity.magnitude > maxSpeed)//prevent player from going faster than maximum speed
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        animator.SetBool("IsFlying", true);//animation adjustments

    }

    void PackageShoot(GameObject packageMoved, float force)
    {
        if (heldPackages.Count > 0)
        {
            //turn off inventory bubble
            packageMoved.gameObject.GetComponent<Package>().Throw();

            //determine the direction for the package to travel
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0f;
            mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.z * -1));
            Vector2 heading = mousePos - packageMoved.transform.position;
            float distance = heading.magnitude;
            Vector2 direction = heading / distance;

            //remove the package being shot
            Destroy(packageMoved.GetComponent<SpringJoint2D>()); //remove the spring arm
            packageMoved.GetComponent<BoxCollider2D>().usedByEffector = true; //activate the effector to add force
            packageMoved.GetComponent<Package>().DoNotCollideWithPlayer(shootPackageCollisionImmuneTime); //make sure package won't collide with player
            packageMoved.GetComponent<Rigidbody2D>().AddForce(direction * force); //add force
            heldPackages.Remove(packageMoved);

            //cue throwing sound
            ThrowAudioSource.Play();

            //attach the first package's spring joint to the one that was behind it
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
        ///tutorial stuff///////////////
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            if (tutorialManager.i == 9 || tutorialManager.i == 12)
            {
                //print("playercontroller");
                tutorialManager.ToggleDialogueOn();
                tutorialManager.i++;
                Time.timeScale = 0f;
            }
        }
        //////////////////////////////////

        if (!heldPackages.Contains(other.gameObject) && heldPackages.Count < maxPackages)
        {
            //set the package touched to a local variable
            Package package = other.GetComponent<Package>();
            package.lineRend = GetComponent<LineRenderer>();

            //turn on inventory bubble
            package.Pickup();

            //add a spring joint to the package to physically bind the packages to the player or other packages
            SpringJoint2D rope = other.gameObject.AddComponent<SpringJoint2D>();

            //determine if the package should be added to the front or back of the line
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

            //pick a random audio clip from the inventory add clip array to play
            int i = (int)Mathf.Floor(Random.Range(0, inventoryAddClips.Length));

            //assign the clip and play
            PackageCollectAudioSource.clip = inventoryAddClips[i];
            PackageCollectAudioSource.Play();
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
                //Debug.Log("Scroll up!");
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
            if(rb.velocity.magnitude > hitSpeed)//only play the hit animation when zip hits not a package at a certain speed

            {
                //toggle the animator
                animator.SetTrigger("Hit");

                //toggle the face animation
                faceAnim.SetFreakOutFace();
            }

        }
        //if (other.gameObject.CompareTag("Package"))//check that zip has hit a package to collect.
        //{
        //    PackageAdd(other.gameObject);
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (!other.gameObject.CompareTag("Package"))
        //{
        //    if (rb.velocity.magnitude > hitSpeed)//only play the hit animation when zip hits not a package at a certain speed

        //    {
        //        animator.SetTrigger("Hit");
        //    }

        //}
        if (other.gameObject.CompareTag("Package"))//check that zip has hit a package to collect.
        {
            PackageAdd(other.gameObject);
        }
    }

    IEnumerator StartExtinguisherSound()
    {
        //set & play fire extinguisher start sound
        ExtinguisherAudioSource.clip = extinguisherStart;
        ExtinguisherAudioSource.Play();

        yield return new WaitForSeconds(ExtinguisherAudioSource.clip.length);

        //set & play loop sound after setting it to loop
        ExtinguisherAudioSource.clip = extinguisherLoop;
        ExtinguisherAudioSource.loop = true;
        ExtinguisherAudioSource.Play();
    }

    /// <summary>
    /// this teleports the player to a fixed location
    /// </summary>
    //public void Teleport() //changed to coroutine
    public IEnumerator Teleport()
    {
        for (int i = 0; i < heldPackages.Count; i++)//remove all packages from the player inventory, remove the bubble, and update line renderer
        {
            heldPackages[i].GetComponent<Package>().Throw();
            Destroy(heldPackages[i].GetComponent<SpringJoint2D>());
        }

        foreach (Collider2D c in allColliders)
            c.enabled = false;


        heldPackages.Clear();//remove all packages from the player's inventory
        lineRenderer.positionCount = heldPackages.Count;//clear all of the line renderer's points

        TeleportAudioSource.Play();//play the teleport sound

        face_mat.DOFloat(1, "_Vector1_AlphaClip", dissolveTime); //dissolve materials
        body_mat.DOFloat(1, "_Vector1_AlphaClip", dissolveTime);
        fe_mat.DOFloat(1, "_Vector1_AlphaClip", dissolveTime);

        rb.velocity = new Vector2(0f,0f); // set velocity to 0 to discontiue movement

        yield return new WaitForSeconds(dissolveTime);

        this.transform.position = teleportTransform.position; // set player position to the teleporter's position

        face_mat.SetFloat("_Vector1_AlphaClip", 0); //reset material alpha clip to default
        body_mat.SetFloat("_Vector1_AlphaClip", 0);
        fe_mat.SetFloat("_Vector1_AlphaClip", 0);

        foreach (Collider2D c in allColliders)
            c.enabled = true;
    }

    /// <summary>
    /// Handles moving the camera closer or further based on player's speed
    /// </summary>
    public float DetermineCameraZ()
    {
        lerpPerc = rb.velocity.magnitude / maxSpeed;//get the percentage of how fast the player is going compared to their max speed

        float desiredZ = Mathf.Lerp(minZPosition, maxZPosition, lerpPerc);//lerp to find new camera z position based on speed percentage

        return desiredZ;
    }

    /// <summary>
    /// Rotates the direction indicator around the player based on desired input type.
    /// </summary>
    void RotatePointer()
    {
//////////////////////This code only works with mouse. Currently working on implementing code for controller input///////////////////////////////////////
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        Vector3 rotateVector = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.position.y, 0f);
        if(MenuController.invertedMovement)
            movementPointer.transform.rotation = Quaternion.LookRotation(-rotateVector, -Vector3.forward);
        else
            movementPointer.transform.rotation = Quaternion.LookRotation(rotateVector, -Vector3.forward);
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
