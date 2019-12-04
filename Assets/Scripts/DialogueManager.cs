using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public Transform player;
    public GameObject menu;

    bool menuActive = false;

    Camera main;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("m") && !menuActive)
        {
            main.GetComponent<Camera2DFollow>().enabled = false;
            main.transform.DOMove(new Vector3(player.transform.position.x, player.transform.position.y, 1.0f), 0.3f).OnComplete(MakeMenu).SetEase(Ease.InBack);
        }

        if(Input.GetKeyDown("m") && menuActive)
        {
            main.GetComponent<Camera2DFollow>().enabled = true;
            main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -100f);
            RemoveMenu(); 
        }
    }
    void MakeMenu()
    {
        Time.timeScale = 0f;
        menu.gameObject.SetActive(true);
        menuActive = true;
    }

    void RemoveMenu()
    {
        Time.timeScale = 1f;
        menu.gameObject.SetActive(false);
        menuActive = false;
    }
}
