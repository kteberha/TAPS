using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Teleport();
        }
        else if (collision.CompareTag("Package"))
        {
            collision.GetComponent<Package>().RemovePackages();
            Destroy(collision.gameObject);
        }
        else
            Destroy(collision.gameObject);
    }
}
