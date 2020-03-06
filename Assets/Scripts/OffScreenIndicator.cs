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

    [SerializeField] Image[] packageSlotImages;//references the images in the circle that will have their sprites swapped
    [SerializeField] Sprite[] packageSprites;//references to use for swapping out the image sprites in the package slots
    Sprite packageTypeAssigned;//temporary holder to assign each appropriate slot

    [SerializeField] GameObject arrowHolder;// to indicate where the associated home is


    // Update is called once per frame
    void Update()
    {
        // Put it on the edge
        Vector3 edgePt = Camera.main.WorldToScreenPoint(target.position);
        edgePt.x = Mathf.Clamp(edgePt.x, Screen.width * 0.06f, Screen.width * 0.94f);
        edgePt.y = Mathf.Clamp(edgePt.y, Screen.height * 0.1f, Screen.height * 0.9f);
        transform.position = edgePt;

        Scale();
        RotateArrow();
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
        imageScaleVal = Mathf.Lerp(0.5f, .85f, distanceFromTarget);

        this.transform.localScale = Vector3.one * imageScaleVal;
        //multiply image scale by distance and clamp image scale between 0.5 and 1
        //this will need some extra manipulation
    }

    void RotateArrow()
    {
        Vector3 v = Camera.main.WorldToScreenPoint(target.position);//get the target position in world space

        arrowHolder.transform.rotation = Quaternion.LookRotation(v, -transform.forward);//rotate the arrow holder toward the target
        arrowHolder.transform.rotation = new Quaternion(0, 0, arrowHolder.transform.rotation.z, arrowHolder.transform.rotation.w);
    }

    /// <summary>
    /// Updates the "order ticket" that appears on the offscreen indicator with images of packages
    /// </summary>
    /// <param name="_asteroidHome"></param>
    public void OrderTicketUpdate(AsteroidHome _asteroidHome)
    {
        //print("ui should update");//testing

        int i = 0;

        foreach (GameObject package in _asteroidHome.packagesOrdered)//go through each item in packages ordered list
        {
            string type = package.name;

            switch (type)//determine sprite based on package name saved above
            {
                case ("SquarePackage"):
                    packageTypeAssigned = packageSprites[0];
                    //print("SquarePackage Sprite");//testing
                    break;
                case ("ConePackage"):
                    packageTypeAssigned = packageSprites[1];
                    //print("ConePackage Sprite");//testing
                    break;
                case ("EggPackage"):
                    packageTypeAssigned = packageSprites[2];
                    //print("EggPackage Sprite");//testing
                    break;
                default:
                    packageTypeAssigned = null;
                    print("Error assigning package image: Offscreen Indicator");
                    break;
            }


            packageSlotImages[i].sprite = packageTypeAssigned;//alter the sprite in the image slot

            //print(packageSlotImages[i].sprite.name);//testing


            if (i + 1 < _asteroidHome.packagesOrdered.Count)
            {
                i++; //increment the index holder to work with the next icon slot
                //print("There are more packages to loop through");//testing
            }
            else
            {
                //print("Set other slot images to nothing");//testing
                for (int n = i + 1; n < packageSlotImages.Length; n++)//loop through the remaining slots
                {
                    //print("N " + n);//testing
                    packageSlotImages[n].color = Color.clear;//make the sprites invisible
                    //print("slot: " + (n + 1) + " is invisible");//testing
                }
                break;//break the foreach loop
            }
        }
    }

    public void CheckHouseRendered(Renderer _houseRenderer)
    {
        if (_houseRenderer.isVisible)
            arrowHolder.SetActive(false);
        else
            arrowHolder.SetActive(true);
    }
}
