using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 2.5f;
    public float maxSpeed = 10;

    private Rigidbody2D rb;

    public int heldPackages = 0;

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
        if (heldPackages < 0)
        {
            heldPackages = 0;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Package")
        {
            Destroy(collision.gameObject);
            heldPackages += 1;
        }
    }
}
