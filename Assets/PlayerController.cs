using System.Collections;
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Accelerate") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Propulsion();
        }
        if (Input.GetAxis("Fire2") != 0 && heldPackages.Count > 0 && shootCooldownClock <= 0)
        {
            Shoot();
            shootCooldownClock = shootCooldown;
        }

        //Clocks
        shootCooldownClock -= Time.deltaTime;
    }

    void Propulsion()
    {
        Vector2 playerPos = this.transform.position; //player position
        Vector2 mousePos = Input.mousePosition; //mouse position
        mousePos = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(mousePos);
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
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            Vector2 heading = mousePos - heldPackages[0].transform.position;
            float distance = heading.magnitude;
            Vector2 direction = heading / distance;

            Destroy(heldPackages[0].GetComponent<SpringJoint2D>());
            heldPackages[0].GetComponent<BoxCollider2D>().usedByEffector = true;
            heldPackages[0].GetComponent<Package>().DoNotCollideWithPlayer(shootPackageCollisionImmuneTime);
            heldPackages[0].GetComponent<Rigidbody2D>().AddForce(direction * shootForce);            
            heldPackages.Remove(heldPackages[0]);     

            if (heldPackages.Count > 0)
            {
                heldPackages[0].GetComponent<SpringJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
            }
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
        }
    }
}
