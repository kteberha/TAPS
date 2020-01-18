using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHome : MonoBehaviour
{
    [System.Serializable]
    public class pointsAtTime
    {
        public float time; //in seconds
        public int points;
    }
    public GameManager gm;

    public pointsAtTime[] deliverPointsAtTime;
    public float currentTime = 0f;
    public float maxTime = 60f;

    public bool packageOrdered;
    public int numPackages;
    public GameObject offScreenIndicator;
    public DemandController demandController;

    // Start is called before the first frame update
    void Start()
    {
        offScreenIndicator.SetActive(false);
        packageOrdered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime < maxTime)
            currentTime += Time.deltaTime;
        OrderExpired();
    }

    /// <summary>
    /// The home places an order for packages to be delivered
    /// </summary>
    public void Order()
    {
        //check that this house hasn't ordered a package already
        if (packageOrdered == false)
        {
            //set order status
            packageOrdered = true;
            //determine number of packages ordered
            numPackages = Random.Range(2, 5);
            //print("number of packages ordered: " + numPackages);

            //eventually get to types of packages
            //

            //set assigned demand indicator to be active with assigned time
            offScreenIndicator.SetActive(true);
            demandController.CurrentValue = 300f;
        }
    }

    /// <summary>
    /// Add points to player's score based on the time of package's arrival
    /// </summary>
    public void Deliver()
    {
        print("packages left to deliver: " + numPackages);

        //check number of packages left to deliver
        if (numPackages - 1 <= 0)
        {
            OrderFulfilled();
        }

        numPackages--;
    }

    /// <summary>
    /// When all the packages are delivered as requested, award the points to the player
    /// </summary>
    void OrderFulfilled()
    {
        print("order fulfilled");

        int pointsToAward = 1;
        foreach (pointsAtTime p in deliverPointsAtTime)
        {
            if (p.time <= currentTime)
                pointsToAward = p.points;
        }

        gm.packagesDelivered += 1;
        gm.points += pointsToAward;

        currentTime = 0f;

        offScreenIndicator.SetActive(false);

        packageOrdered = false;
    }

    public void OrderExpired()
    {
        if (demandController.CurrentValue <= 0)
        {
            offScreenIndicator.SetActive(false);
            packageOrdered = false;
            numPackages = 0;
        }
    }
}
