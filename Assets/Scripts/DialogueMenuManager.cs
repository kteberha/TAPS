using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueMenuManager : MonoSingleton<DialogueMenuManager>
{
    public GameObject player;
    public GameObject menu;

    public GameObject goalsScreen;

    Rigidbody2D rb;
    Vector3 pTempVelocity;
    bool menuActive = false;

    Camera main;
    Transform dialogueCameraTransform;


    [Header("DEMO TUTORIAL")]
    public GameObject dialogueScreen;


    private void Start()
    {
        main = Camera.main;
        rb = player.GetComponent<Rigidbody2D>();
        dialogueCameraTransform = PlayerController.Instance.dialogueCameraPoint;
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
        GameManager.Instance.state = gs;
        //menuController.paused = true;
        //Time.timeScale = 0f;
        menuActive = true;
        menu.gameObject.SetActive(true);
        player.GetComponent<AudioSource>().Stop();
    }

    void TutorialMakeMenu()
    {
        GameManager.Instance.state = GAMESTATE.PAUSED;
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

        if (GameManager.Instance.state == GAMESTATE.CLOCKEDOUTSTART)
        {
            goalsScreen.SetActive(true);
        }
        else if(GameManager.Instance.state == GAMESTATE.CLOCKEDOUTEND)
        {
            StartCoroutine(GameManager.Instance.ShutDown());
        }
    }

    public void ClockIn()
    {
        goalsScreen.SetActive(false);
        GameManager.Instance.state = GAMESTATE.CLOCKEDIN;
        GameManager.Instance.textAnimation.Play();
        NarrativeDialogue.Instance.story = NarrativeDialogue.Instance.NewStory();
    }

    public void SkipDialogue()
    {
        EndDialogue();
    }
}
