using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public Transform target;

    float distanceFromTarget;
    float sizeMultiplier;
    public Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Put it on the edge
        Vector3 edgePt = Camera.main.WorldToScreenPoint(target.position);
        edgePt.x = Mathf.Clamp(edgePt.x, Screen.width * 0.04f, Screen.width * 0.96f);
        edgePt.y = Mathf.Clamp(edgePt.y, Screen.height * 0.07f, Screen.height * 0.93f);
        transform.position = edgePt;

    }

    void Scale()
    {
        //get the float value for the distance to multiply to the scale
        //SqrMagnitude because its faster than Magnitube
        distanceFromTarget = Vector3.SqrMagnitude(target.position - playerTransform.position);
        
        //multiply image scale by distance and clamp image scale between 0.5 and 1
        //this will need some extra manipulation

    }
}
