using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    GameObject centerObj;
    private Transform center;
    private Vector3 axis = Vector3.forward;
    private Vector3 desiredPosition;
    private float originalZ = 0;

    public float radius = 2.0f;
    public float radiusSpeed = 0.5f;
    public float rotationSpeed = 80.0f;
    public float maxDistanceFromCenter = 25;

    public bool willOrbit = true;


    // orbits around a "star" at the origin with fixed mass
    public float starMass = 1000f;

    void Start()
    {
        centerObj = GameObject.FindWithTag("ShippingLane");
        center = centerObj.transform;
        originalZ = this.transform.position.z;

        float initV = Mathf.Sqrt(starMass / transform.position.magnitude);
        GetComponent<Rigidbody2D>().velocity = new Vector3(0, initV, 0);

        float dist = Vector3.Distance(center.position, transform.position);
        radius = dist;
    }

    void Update()
    {
        float dist = Vector3.Distance(center.position, transform.position);
        radius = dist;
        if (dist > maxDistanceFromCenter)
            willOrbit = false;
        else
            willOrbit = true;

        if (willOrbit)
        {
            //float r = Vector3.Magnitude(transform.position);
            //float totalForce = -(starMass) / (r * r);
            //Vector3 force = (transform.position).normalized * totalForce;
            //GetComponent<Rigidbody2D>().AddForce(force);
            //Debug.Log(force);

            float distFromEdge = maxDistanceFromCenter - dist;
            float relativeRotationSpeed = rotationSpeed * (distFromEdge / maxDistanceFromCenter);

            transform.RotateAround(center.position, axis, rotationSpeed * Time.deltaTime);
            desiredPosition = (transform.position - center.position).normalized * radius + center.position;

            Vector3 heading = desiredPosition - transform.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance; //normalized direction

            Vector3 force = direction * radiusSpeed;

            GetComponent<Rigidbody2D>().AddForce(force);
            Debug.Log(force);
            //transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed); 
        }
        
    }


}
