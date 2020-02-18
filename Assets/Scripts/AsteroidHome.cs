using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHome : MonoBehaviour
{
    //[System.Serializable]
    //public class pointsAtTime
    //{
    //    public float time; //in seconds
    //    public int points;
    //}
    //public pointsAtTime[] deliverPointsAtTime;
    //public float currentTime = 0f;

    public int points = 1;
    public GameManager gm;

    public bool packageOrdered = false;
    public int numPackages;
    public float orderTime = 120f;//time before an order expires
    public float orderDelayTime = 5f;//time to delay another order after expired or fulfilled orders
    private float delayReset;
    public GameObject offScreenIndicator;
    public DemandController demandController;

    AudioSource audioSource;
    [SerializeField] AudioClip orderedClip, expiredClip, successClip;

    bool doOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        offScreenIndicator.SetActive(false);
        delayReset = orderDelayTime;//set the delay timer restart to whatever the user designates
        orderDelayTime = 0f;//set the delay timer to 0 for the very first package orders
    }

    // Update is called once per frame
    void Update()
    {
        if(demandController.CurrentValue <= 0)//check if the offscreen indicator value is <= 0
        {
            if (!doOnce)//if the order hasn't called the expired method once
            {
                doOnce = true;//trigger the one time bool
                OrderExpired();//set the order to expired
            }
        }
        else//if the current value of offscreen indicator is > 0
        {
            doOnce = false;//ensure the do once bool is false
        }
    }

    /// <summary>
    /// The home places an order for packages to be delivered
    /// </summary>
    public void Order()
    {
        if (packageOrdered == false)//check that this house hasn't ordered a package already
        {
            if (orderDelayTime <= 0)//make sure the delay time is 0 so that an order isn't placed right after one is fulfilled or expires
            {
                print(this.name + " has ordered successfully");
                audioSource.clip = orderedClip;//set the proper audio clip
                audioSource.Play();//play audio
                packageOrdered = true;//set order status

                numPackages = 2;//determine number of packages ordered

                //eventually get to types of packages//

                ///////////////////////////////////////

                demandController.maxValue = orderTime;//set the slider's max time value
                demandController.CurrentValue = orderTime;//set the slider's current value

                offScreenIndicator.SetActive(true);//set assigned demand indicator to be active with assigned time
            }
            else//run the delay then place the order
            {
                print(this.name + " needs to wait");
                StartCoroutine(OrderDelay());
            }
        }
    }

    /// <summary>
    /// Add points to player's score based on the time of package's arrival
    /// </summary>
    public void Deliver()
    {
        print("packages left to deliver: " + numPackages);

        if (numPackages - 1 <= 0)//check number of packages left to deliver

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
        offScreenIndicator.SetActive(false);

        packageOrdered = false;

        audioSource.clip = successClip;
        audioSource.Play();

        //int pointsToAward = 1;
        //foreach (pointsAtTime p in deliverPointsAtTime)
        //{
        //    if (p.time <= currentTime)
        //        pointsToAward = p.points;
        //}
        gm.packagesDelivered += 1;
        gm.points += points;
        gm.ordersFulfilled += 1;

        orderDelayTime = delayReset;//set the order delay timer so that it will trigger the delay branch in the order method.
    }

    public void OrderExpired()
    {
        if (demandController.CurrentValue <= 0)
        {
            audioSource.clip = expiredClip;//set the audio clip
            audioSource.Play();//play the audio
            offScreenIndicator.SetActive(false);//turn off the offscreen indicator
            packageOrdered = false;//toggle the house's package ordered state to false so it knows it can order another package
            numPackages = 0;
            gm.refundsOrdered += 1;//increment the refunds ordered variable in the game manager
            orderDelayTime = delayReset;//set the order delay timer so that it will trigger the delay branch in the order method.
        }
    }


    /// <summary>
    /// Delays the ordering process for homes that have just had orders fulfilled or expired
    /// </summary>
    /// <returns></returns>
    IEnumerator OrderDelay()
    {
        yield return new WaitForSeconds(orderDelayTime);
        orderDelayTime = 0;
        Order();
    }
}
