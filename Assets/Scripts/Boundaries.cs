﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    private Vector2 leftrightBoundaries;
    private Vector2 topbottomBoundaries;
    private PlayerController playerController;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        leftrightBoundaries = new Vector2(2378, 114);
        topbottomBoundaries = new Vector2(532, 1812);
        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player.transform.position.y > topbottomBoundaries.y || player.transform.position.y < -(topbottomBoundaries.y))
        {
            StartCoroutine(playerController.Teleport());
        }
        if (player.transform.position.x > leftrightBoundaries.x || player.transform.position.x < -(leftrightBoundaries.x))
        {
            StartCoroutine(playerController.Teleport());
        }
    }
}
