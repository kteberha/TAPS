using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Rope : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private float counter;
    private float dist;

    public Transform playerTransform;
    public Transform destinationTransform;

    public float lineDrawSpeed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, playerTransform.position);

        dist = Vector3.Distance(playerTransform.position, destinationTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(counter < dist)
        {
            counter += .1f / lineDrawSpeed;

            float x = Mathf.Lerp(0, dist, counter);

            Vector3 pointA = playerTransform.position;
            Vector3 pointB = destinationTransform.position;

            //Get the unit vector in the desired direction, multiply by the desired length and add the starting point.
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;

            lineRenderer.SetPosition(1, pointAlongLine);
        }
        lineRenderer.SetPosition(0, playerTransform.position);
    }

}
