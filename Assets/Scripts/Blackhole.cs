using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().Teleport();
        }
        else if (collision.tag == "Package")
        {
            collision.GetComponent<Package>().RemovePackages();
            Destroy(collision.gameObject);
        }
        else
            Destroy(collision.gameObject);
    }
}
