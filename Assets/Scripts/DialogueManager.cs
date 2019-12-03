using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

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
            main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 1.0f);
            //menu.gameObject.SetActive(true);
            }
    }
}
