using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public Camera mainCamera;
    public Transform player;
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            if (playerController.heldPackages.Count > 0)
            {
                RaycastHit hit;

                lineRenderer.enabled = true;

                lineRenderer.SetPosition(0, player.position);

                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = player.position.z - Camera.main.transform.position.z;
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                if (Physics.Linecast(player.position, mousePosition, out hit))
                {
                    mousePosition = hit.point;
                }

                lineRenderer.SetPosition(1, mousePosition);
            }
        }
        if (Input.GetButtonUp("Fire2"))
        {
            lineRenderer.enabled = false;
        }
    }
}
