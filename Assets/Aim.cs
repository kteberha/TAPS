using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
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
        RaycastHit hit;

            lineRenderer.SetPosition(0, player.position);

           
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = player.position.z - Camera.main.transform.position.z;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

             if (Physics.Linecast(player.position, mousePosition, out hit)) {
            mousePosition = hit.point;
        }   

            lineRenderer.SetPosition(1,  mousePosition);
        Debug.Log(mousePosition);
    }
}
