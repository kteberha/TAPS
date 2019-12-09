﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using DG.Tweening;

public class DialogueMenuManager : MonoBehaviour
{
    public GameObject player;
    public GameObject menu;

    Rigidbody2D rb;
    Vector3 pTempVelocity;
    bool menuActive = false;
    
    Camera main;
    Transform dialogueCameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        rb = player.GetComponent<Rigidbody2D>();
        dialogueCameraTransform = player.GetComponent<PlayerController>().dialogueCameraPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("m") && !menuActive)
        {
            pTempVelocity = rb.velocity;
            rb.velocity = new Vector3(0f, 0f, 0f);
            main.GetComponent<Camera2DFollow>().enabled = false;
            main.transform.DOMove(new Vector3(dialogueCameraTransform.position.x , dialogueCameraTransform.transform.position.y, dialogueCameraTransform.position.z), 0.3f).OnComplete(MakeMenu).SetEase(Ease.InBack);
        }

        if(Input.GetKeyDown("m") && menuActive)
        {
            main.GetComponent<Camera2DFollow>().enabled = true;
            main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -100f);
            rb.velocity = pTempVelocity;
            RemoveMenu(); 
        }
    }
    void MakeMenu()
    {
        Time.timeScale = 0f;
        menuActive = true;
        menu.gameObject.SetActive(true);
        player.GetComponent<AudioSource>().Stop();
    }

    void RemoveMenu()
    {
        Time.timeScale = 1f;
        menuActive = false;
        menu.gameObject.SetActive(false);
    }
}