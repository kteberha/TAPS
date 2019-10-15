﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private List<GameObject> heldPackages = new List<GameObject>();
    
    public float playerSpeed = 2.5f;
    public float maxSpeed = 10;

    public float shootForce = 10f;
    public float shootCooldown = 1f;
    private float shootCooldownClock = 0f;
    public float shootPackageCollisionImmuneTime = 0.5f;

    public float inventoryDistance = 5f;
    public float inventoryDampingRatio = 1f;
    public float inventoryFrequency = 0.5f;

    //variables for the lineRenderer
    private LineRenderer lineRenderer;
    private float counter;
    private float dist = 0f;
    public float lineDrawSpeed = 6f;

    //Audio Variables
    private AudioSource P_audioSource;
    public AudioClip extinguisherStart;
    public AudioClip extinguisherLoop;
    public AudioClip extinguisherEnd;
    public AudioClip throwSound;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        lineRenderer = GetComponent<LineRenderer>();

        P_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Accelerate") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Propulsion();
        }
        if (Input.GetKeyUp("mouse 1") && heldPackages.Count > 0 && shootCooldownClock <= 0)
        {
            Shoot();
            shootCooldownClock = shootCooldown;
        }

        //Clocks
        shootCooldownClock -= Time.deltaTime;

        //Check if player has packages to draw lines to
        if (heldPackages.Count > 0)
        {
            //Set the Origin of the line renderer to the player position
            lineRenderer.SetPosition(0, this.transform.position);
            

            //Set the positions of the following line render points to all of the transforms in the heldPackages list
            for (int i = 0; i <= heldPackages.Count - 1 ; i++)
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

        if(Input.GetMouseButtonDown(0))
        {
            StartCoroutine("StartExtinguisher");
        }

        if(Input.GetMouseButtonUp(0))
        {
            //stop fire extinguisher loop
            P_audioSource.loop = false;
            P_audioSource.clip = extinguisherEnd;
            P_audioSource.Play();
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
    }

    void Shoot()
    {
        if (heldPackages.Count > 0)
        {
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

            P_audioSource.PlayOneShot(throwSound);

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
        if (other.gameObject.CompareTag("Package"))
        {
            if (!heldPackages.Contains(other.gameObject))
            {
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
                print(other.gameObject.name);
            }

            //tell the line renderer to draw to the newest added point (line renderer will always have 2 positions though)
            lineRenderer.positionCount = heldPackages.Count + 1;
        }
    }

    IEnumerator StartExtinguisher()
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
}
