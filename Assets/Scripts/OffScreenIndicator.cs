using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public Transform target;

    float distanceFromTarget;
    float sizeMultiplier = 4;
    public Transform playerTransform;

    public float minDistance;
    public float maxDistance;

    float imageScaleVal;

    // Update is called once per frame
    void Update()
    {
        // Put it on the edge
        Vector3 edgePt = Camera.main.WorldToScreenPoint(target.position);
        edgePt.x = Mathf.Clamp(edgePt.x, Screen.width * 0.06f, Screen.width * 0.94f);
        edgePt.y = Mathf.Clamp(edgePt.y, Screen.height * 0.1f, Screen.height * 0.9f);
        transform.position = edgePt;

        Scale();
    }

    void Scale()
    {
        //get the float value for the distance to multiply to the scale
        //SqrMagnitude because its faster than Magnitube
        distanceFromTarget = Vector3.Magnitude(target.position - playerTransform.position);
        //print(distanceFromTarget);

        //get decimal value of distance compared to min distance?
        distanceFromTarget = 1 - (distanceFromTarget - minDistance) / (maxDistance - minDistance);
        //print(distanceFromTarget);

        //locks the scale value between the min and max distance values (needs to be converted to a number between 0 & 1)
        imageScaleVal = Mathf.Lerp(0.5f, 1.0f, distanceFromTarget);

        this.transform.localScale = Vector3.one * imageScaleVal;
        //multiply image scale by distance and clamp image scale between 0.5 and 1
        //this will need some extra manipulation
    }
}
