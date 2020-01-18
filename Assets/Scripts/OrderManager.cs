using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //countdown timer
        orderTimer -= Time.deltaTime;

        //check when the timer reaches 0;
        if (orderTimer <= 0)
        {
            //go through the list of homes and see if they can order a package
            for (int i = 0; i < homes.Length; i++)
            {
                //check if the home being compared has ordered a package, if so break the loop
                if (homes[i].packageOrdered == false)
                {
                    //print("order has been placed");

                    homes[i].Order();
                    break;
                }
                //else
                //    print("no room for orders");
            }

            orderTimer = resetTimer;
        }
    }
}
