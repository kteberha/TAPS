using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public Camera mainCamera;
    public Transform player;
    public PlayerController playerController;
    public float maxLength;

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
            if (playerController.heldPackages.Count > 0) //check that the player has packages to throw
            {
                RaycastHit hit;

                lineRenderer.enabled = true;//make the line visible

                lineRenderer.SetPosition(0, player.position);//always start the line at the player position

                Vector3 mousePosition = Input.mousePosition;// get the basis for the end of the line
                mousePosition.z = player.position.z - Camera.main.transform.position.z;// make sure to have the appropriate z value.
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                if (Physics.Linecast(player.position, mousePosition, out hit))
                {
                    mousePosition = hit.point;
                }

                Vector3 difference = mousePosition - player.position;//get the vector between the player and the mouse
                Vector3 direction = difference.normalized;//get the direction of the vector above
                float distance = Mathf.Min(maxLength, difference.magnitude);//"clamp" done by using the smaller of either values
                Vector3 endPosition = player.position + direction * distance;//assign the calculated value to a variable to be used by the line renderer.

                lineRenderer.SetPosition(1, endPosition);//actually set the second point of the line renderer with the calculated vector above
            }
        }
        if (Input.GetButtonUp("Fire2"))
        {
            lineRenderer.enabled = false;
        }
    }
}
