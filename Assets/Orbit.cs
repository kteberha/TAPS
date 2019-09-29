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

    public float maxDistanceFromCenter = 25;

    //rotation
    public float rotationSpeed = 80.0f;
    [SerializeField]
    private float currentRotationSpeed = 0f;
    public float rotationAccel = 10f;
    public float rotationDecel = 5f;

    //release slingshot
    bool hasReleased;
    Vector3 releaseVelocity;
    private Vector3 posSpeed;
    private Vector3 lastPos;

    void Start()
    {
        centerObj = GameObject.FindWithTag("ShippingLane");
        center = centerObj.transform;
        originalZ = this.transform.position.z;

        float dist = Vector3.Distance(center.position, transform.position);
        lastPos = transform.position;
    }

    void Update()
    {
        //calculate distance from Shipping Lane center
        float dist = Vector3.Distance(center.position, transform.position);

        

        if (dist > maxDistanceFromCenter) //if the object is outside of the Shipping Lane's range, decrease rotation speed
        {
            if (!hasReleased)
                Release();
            currentRotationSpeed = 0;//-= rotationDecel;
            if (currentRotationSpeed < 0)
                currentRotationSpeed = 0;
        }
        else //if the object is within the Shipping Lane's range, increase rotation speed
        {
            hasReleased = false;
            currentRotationSpeed += rotationAccel;
            if (currentRotationSpeed > rotationSpeed)
                currentRotationSpeed = rotationSpeed;
        }
        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.up);
        //GetComponent<Rigidbody2D>().AddRelativeForce(localForward * 1f);
        //GetComponent<Rigidbody2D>().AddTorque(1f * Time.deltaTime);
        transform.RotateAround(center.position, axis, currentRotationSpeed * Time.deltaTime);

        if (lastPos != transform.position)
        {
            posSpeed = transform.position - lastPos;
            posSpeed /= Time.deltaTime;
            lastPos = transform.position;
        }


    }

    // Call this when you want to let the sling loose to fly in a straight line.
    public void Release()
    {
        Vector3 offsetFromCenter = transform.position - center.position;
        float radius = offsetFromCenter.magnitude;

        Vector3 travelDirection = Vector3.Cross(axis, offsetFromCenter).normalized;

        releaseVelocity = posSpeed;
        this.GetComponent<Rigidbody2D>().velocity = releaseVelocity;

        hasReleased = true;
    }


}
