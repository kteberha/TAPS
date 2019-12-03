using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    private Vector2 leftrightBoundaries;
    private Vector2 topbottomBoundaries;
    GameObject player;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        leftrightBoundaries = new Vector2(2378, 114);
        topbottomBoundaries = new Vector2(532, 1812);
    }

    private void Update()
    {
        if (player.transform.position.y > topbottomBoundaries.y || player.transform.position.y < -(topbottomBoundaries.y))
        {
            print(player.transform.position);
            print("y");
            playerController.Teleport();
        }
        if (player.transform.position.x > leftrightBoundaries.x || player.transform.position.x < -(leftrightBoundaries.x))
        {
            print(player.transform.position);
            print("x");
            playerController.Teleport();
        }
    }

    /// <summary>
    /// this was supposed to make the player character teleport when it collided with the wall, but it didn't work for some reason. -Emma
    /// </summary>
    //    private void OnTriggerEnter(Collider other)
    //    {
    //        print("Triggered");
    //        if (other.gameObject.tag == "Player") {
    //            player.Teleport();
    //        }
    //    }
}
