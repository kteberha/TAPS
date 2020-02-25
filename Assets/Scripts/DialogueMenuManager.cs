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

    Rigidbody2D rb;
    Vector3 pTempVelocity;
    bool menuActive = false;

    Camera main;
    Transform dialogueCameraTransform;

    private void Awake()
    {
        main = Camera.main;
        rb = player.GetComponent<Rigidbody2D>();
        dialogueCameraTransform = player.GetComponent<PlayerController>().dialogueCameraPoint;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartDialogue()
    {
        pTempVelocity = rb.velocity;
        rb.velocity = new Vector3(0f, 0f, 0f);
        main.GetComponent<Camera2DFollow>().enabled = false;
        main.transform.DOMove(new Vector3(dialogueCameraTransform.position.x, dialogueCameraTransform.transform.position.y, dialogueCameraTransform.position.z), 0.3f).OnComplete(MakeMenu).SetEase(DG.Tweening.Ease.InBack);
    }

    public void EndDialogue()
    {
        main.GetComponent<Camera2DFollow>().enabled = true;
        main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -100f);
        rb.velocity = pTempVelocity;
        RemoveMenu();
    }

    void MakeMenu()
    {
        menuController.paused = true;
        Time.timeScale = 0f;
        menuActive = true;
        menu.gameObject.SetActive(true);
        player.GetComponent<AudioSource>().Stop();
    }

    void RemoveMenu()
    {
        menuController.paused = false;
        Time.timeScale = 1f;
        menuActive = false;
        menu.gameObject.SetActive(false);
    }

    public void SkipDialogue()
    {
        EndDialogue();
    }
}
