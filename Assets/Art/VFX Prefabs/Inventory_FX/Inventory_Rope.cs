using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Rope : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, player.postion);
    }
}
