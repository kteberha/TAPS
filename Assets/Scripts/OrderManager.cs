using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrderManager : MonoBehaviour
{
    public AsteroidHome[] homes;

    public float orderTimer = 5f;
    float resetTimer;


    // Start is called before the first frame update
    void Start()
    {
        resetTimer = orderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            //countdown timer
            orderTimer -= Time.deltaTime;

            //check when the timer reaches 0;
            if (orderTimer <= 0)
            {
                //print("attempt order");
                //go through the list of homes and see if they can order a package
                for (int i = 0; i < homes.Length; i++)
                {
                    ///////////////////////Make this so that it won't keep picking the same house/////////////
                    //print("checking: " + homes[i].name);
                    //check if the home being compared has ordered a package, if so break the loop
                    if (homes[i].packageBeenOrdered == false)
                    {
                        homes[i].Order();
                        break;
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////
                }

                orderTimer = resetTimer;//reset the order timer.
            }
        }
    }

    /// <summary>
    /// FOR DEMO TUTORIAL
    /// </summary>
    public void OrderPackage()
    {
        print("ordering package");
        for (int i = 0; i < homes.Length; i++)
        {
            ///////////////////////Make this so that it won't keep picking the same house/////////////
            //print("checking: " + homes[i].name);
            //check if the home being compared has ordered a package, if so break the loop
            if (homes[i].packageBeenOrdered == false)
            {
                homes[i].Order();
                break;
            }
        }
        print("ordered");
    }
}
