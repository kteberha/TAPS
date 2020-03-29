using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStop : MonoBehaviour
{
    public Camera cam;

    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("collided");
        if(collision.gameObject.CompareTag("Player"))
        {

                cam.GetComponent<UnityStandardAssets._2D.Camera2DFollow>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("triggered");
        if (cam.GetComponent<UnityStandardAssets._2D.Camera2DFollow>().enabled == false)
            cam.GetComponent<UnityStandardAssets._2D.Camera2DFollow>().enabled = true;

    }
}
