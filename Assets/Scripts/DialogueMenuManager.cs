﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueMenuManager : MonoBehaviour
{
    public GameObject player;
    public GameObject menu;
    public GameObject goalsScreen;

    Rigidbody2D rb;
    Vector3 pTempVelocity;

    Camera main;
    Transform dialogueCameraTransform;

    NarrativeDialogue narrativeDialogue;


    [Header("DEMO TUTORIAL")]
    public GameObject dialogueScreen;


    private void Start()
    {
        main = Camera.main;
        rb = player.GetComponent<Rigidbody2D>();
        dialogueCameraTransform = player.GetComponent<PlayerController>().dialogueCameraPoint;
        narrativeDialogue = GetComponent<NarrativeDialogue>();
    }

    public void StartDialogue(GAMESTATE gs)
    {
        pTempVelocity = rb.velocity;
        rb.velocity = new Vector3(0f, 0f, 0f);
        main.GetComponent<Camera2DFollow>().enabled = false;
        MakeMenu(gs);
    }

    public void EndDialogue()
    {
        main.GetComponent<Camera2DFollow>().enabled = true;
        main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -100f);
        rb.velocity = pTempVelocity;
        RemoveMenu();
    }

    void MakeMenu(GAMESTATE gs)
    {
        GameManager.state = gs;
        menu.gameObject.SetActive(true);
        player.GetComponent<AudioSource>().Stop();
    }

    void TutorialMakeMenu()
    {
        GameManager.state = GAMESTATE.PAUSED;
        Time.timeScale = 0f;
        menu.gameObject.SetActive(true);
    }

    void RemoveMenu()
    {
        Time.timeScale = 1f;
        menu.gameObject.SetActive(false);

        if (GameManager.state == GAMESTATE.CLOCKEDOUTSTART)
        {
            goalsScreen.SetActive(true);
        }
        else if(GameManager.state == GAMESTATE.CLOCKEDOUTEND)
        {
            //StartCoroutine(GameManager.Instance.ShutDown());
        }
    }

    public void ClockIn()
    {
        goalsScreen.SetActive(false);
        GameManager.state = GAMESTATE.CLOCKEDIN;
        //GameManager.Instance.textAnimation.Play();
        narrativeDialogue.story = narrativeDialogue.NewStory();
    }
}
