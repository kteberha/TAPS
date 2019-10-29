using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Put it on the edge
        Vector3 edgePt = Camera.main.WorldToScreenPoint(target.position);
        edgePt.x = Mathf.Clamp(edgePt.x, Screen.width * 0.01f, Screen.width * 0.99f);
        edgePt.y = Mathf.Clamp(edgePt.y, Screen.height * 0.01f, Screen.height * 0.99f);
        transform.position = edgePt;

    }
}
