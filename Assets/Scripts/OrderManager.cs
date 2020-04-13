using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrderManager : MonoBehaviour
{
    public AsteroidHome[] homes;//array of the homes to pull from

    [SerializeField]
    float _orderTimer = 5f;//amount of time before a new house will place an order

    float _resetTimer;//resets the order timer when appropriate

    int _homeIndex = 0;//int to select which house is placing an order (for deepening the ordering system)


    // Start is called before the first frame update
    void Start()
    {
        _resetTimer = _orderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        //check that the game is in the right state to do the order timing logic
        if (GameManager.state == GAMESTATE.CLOCKEDIN)
        {
            //make sure we're not in the tutorial scene
            if (SceneManager.GetActiveScene().name != "TutorialScene")
            {
                _orderTimer -= Time.deltaTime; //countdown timer


                //check when the timer reaches 0;
                if (_orderTimer <= 0)
                {
                    //go through the list of homes and see if they can order a package
                    for (int i = 0; i < homes.Length; i++)
                    {
                        ///////////////////////Make this so that it won't keep picking the same house/////////////
                        //check if the home being compared has ordered a package, if so break the loop
                        if (homes[i].packageBeenOrdered == false)
                        {
                            homes[i].Order();
                            break;
                        }
                        //////////////////////////////////////////////////////////////////////////////////////////
                    }

                    _orderTimer = _resetTimer;//reset the order timer.
                }
            }
        }
    }

    /// <summary>
    /// FOR DEMO TUTORIAL
    /// </summary>
    public void OrderPackage()
    {
        for (int i = 0; i < homes.Length; i++)
        {
            ///////////////////////Make this so that it won't keep picking the same house/////////////
            //check if the home being compared has ordered a package, if so break the loop
            if (homes[i].packageBeenOrdered == false)
            {
                homes[i].Order();
                break;
            }
        }
    }
}
