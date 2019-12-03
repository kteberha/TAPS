using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public Transform player;
    public GameObject menu;

    Camera main;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("m"))
            {
            Debug.Log("m");
            main.GetComponent<Camera2DFollow>().enabled = false;
            main.transform.DOMove(new Vector3(player.transform.position.x, player.transform.position.y, 1.0f), 0.3f).OnComplete(MakeMenu).SetEase(Ease.InBack);
          
            }
    }
    void MakeMenu()
    {
        menu.gameObject.SetActive(true);
    }
}
