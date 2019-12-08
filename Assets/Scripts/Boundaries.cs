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
            playerController.Teleport();
        }
        if (player.transform.position.x > leftrightBoundaries.x || player.transform.position.x < -(leftrightBoundaries.x))
        {
            playerController.Teleport();
        }
    }
}
