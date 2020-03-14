using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueMenuManager : MonoBehaviour
{
    public GameObject player;
    public GameObject menu;
    public MenuController menuController;

    public GameObject goalsScreen;

    Rigidbody2D rb;
    Vector3 pTempVelocity;
    bool menuActive = false;

    Camera main;
    Transform dialogueCameraTransform;


    [Header("DEMO TUTORIAL")]
    public GameObject dialogueScreen;


    private void Awake()
    {
        main = Camera.main;
        rb = player.GetComponent<Rigidbody2D>();
        dialogueCameraTransform = player.GetComponent<PlayerController>().dialogueCameraPoint;
    }

    public void StartDialogue(GAMESTATE gs)
    {
        pTempVelocity = rb.velocity;
        rb.velocity = new Vector3(0f, 0f, 0f);
        main.GetComponent<Camera2DFollow>().enabled = false;
        //main.transform.DOMove(new Vector3(dialogueCameraTransform.position.x, dialogueCameraTransform.transform.position.y, dialogueCameraTransform.position.z), 0.3f).OnComplete(MakeMenu).SetEase(DG.Tweening.Ease.InBack);
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
        menuController.gm.state = gs;
        //menuController.paused = true;
        Time.timeScale = 0f;
        menuActive = true;
        menu.gameObject.SetActive(true);
        player.GetComponent<AudioSource>().Stop();
    }

    void TutorialMakeMenu()
    {
        menuController.gm.state = GAMESTATE.PAUSED;
        Time.timeScale = 0f;
        menuActive = true;
        menu.gameObject.SetActive(true);

    }

    void RemoveMenu()
    {
        //menuController.paused = false;
        Time.timeScale = 1f;
        menuActive = false;
        menu.gameObject.SetActive(false);

        if (menuController.gm.state == GAMESTATE.CLOCKEDOUTSTART)
        {
            goalsScreen.SetActive(true);
        }
        else if(menuController.gm.state == GAMESTATE.CLOCKEDOUTEND)
        {
            menuController.Wishlist();
        }
    }

    public void ClockIn()
    {
        goalsScreen.SetActive(false);
        menuController.gm.state = GAMESTATE.CLOCKEDIN;
    }

    public void SkipDialogue()
    {
        EndDialogue();
    }
}
