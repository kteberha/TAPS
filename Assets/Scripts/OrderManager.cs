using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrderManager : MonoSingleton<OrderManager>
{
    public AsteroidHome[] homes;//array of the homes to pull from

    [SerializeField]
    float orderTimer = 5f;//amount of time before a new house will place an order

    float resetTimer;//resets the order timer when appropriate

    int homeIndex = 0;//int to select which house is placing an order


    // Start is called before the first frame update
    void Start()
    {
        resetTimer = orderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        //check that the game is in the right state to do the order timing logic
        if (GameManager.Instance.state == GAMESTATE.CLOCKEDIN)
        {
            //make sure we're not in the tutorial scene
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
