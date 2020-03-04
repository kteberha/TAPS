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

    public GameManager gm;

    //Variables regarding the packages
    [HideInInspector]public bool packageBeenOrdered = false;//whether house has ordered a package or not
    [HideInInspector]public int numPackagesOrdered;//number of packages ordered by a house
    public float orderTime = 120f;//time before an order expires
    public float orderDelayTime = 5f;//time to delay another order after expired or fulfilled orders
    public int points = 1;

    public GameObject[] packageTypes = new GameObject[3];
    public List<GameObject> packagesOrdered;


    private float delayReset;//value to reset the delay to

    //variables for offscreen indicator
    public GameObject offScreenIndicator;
    public DemandController demandController;


    //variables for audio
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
        if (packageBeenOrdered == false)//check that this house hasn't ordered a package already
        {
            if (orderDelayTime <= 0)//make sure the delay time is 0 so that an order isn't placed right after one is fulfilled or expires
            {
                print(this.name + " has ordered successfully");
                audioSource.clip = orderedClip;//set the proper audio clip
                audioSource.Play();//play audio
                packageBeenOrdered = true;//set order status

                //////////Decide how many packages to order/////////////
                float packageNumSeed = Random.value;
                if (packageNumSeed <= 0.5f)
                    numPackagesOrdered = 1;
                else
                    numPackagesOrdered = 2;


                ////////////////////////////////////////////////////////


                ///////////determine types of packages/////////////
                #region
                float packTypeSeed = Random.value;
                switch (numPackagesOrdered)
                {
                    case (1):
                        if (packTypeSeed <= 0.3f)
                        {
                            print("ordered Square");
                            foreach (GameObject pack in packageTypes)
                            {
                                if (pack.name.Equals("SquarePackage"))
                                {
                                    packagesOrdered.Add(pack);
                                    break;
                                }
                            }
                        }
                        else if (packTypeSeed <= 0.6)
                        {
                            print("ordered Cone");
                            foreach (GameObject pack in packageTypes)
                            {
                                if (pack.name.Equals("ConePackage"))
                                {
                                    packagesOrdered.Add(pack);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            print("ordered Egg");
                            foreach (GameObject pack in packageTypes)
                            {
                                if (pack.name.Equals("EggPackage"))
                                {
                                    packagesOrdered.Add(pack);
                                    break;
                                }
                            }
                        }

                        break;

                    case (2):
                        if(packTypeSeed <= 0.17f)
                        {
                            print("ordered 2 boxes");
                            foreach(GameObject pack in packageTypes)
                            {
                                if(pack.name.Equals("SquarePackage"))
                                {
                                    for (int i = 0; i < 2; i++)
                                        packagesOrdered.Add(pack);
                                }

                                break;
                            }
                        }
                        else if(packTypeSeed <= 0.34f)
                        {
                            print("ordered 2 cones");
                            foreach(GameObject pack in packageTypes)
                            {
                                if(pack.name.Equals("ConePackage"))
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        packagesOrdered.Add(pack);
                                    }
                                    break;
                                }
                            }
                        }
                        else if(packTypeSeed <= 0.51f)
                        {
                            print("ordered 2 eggs");
                            foreach(GameObject pack in packageTypes)
                            {
                                if(pack.name.Equals("EggPackage"))
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        packagesOrdered.Add(pack);
                                    }
                                    break;
                                }
                            }
                        }

                        else if (packTypeSeed <= 0.68f)
                        {
                            print("ordered 1 box, 1 cone");
                            foreach (GameObject pack in packageTypes)
                            {
                                if (pack.name.Equals("SquarePackage"))
                                    packagesOrdered.Add(pack);
                                if (pack.name.Equals("ConePackage"))
                                    packagesOrdered.Add(pack);
                            }
                        }

                        else if (packTypeSeed <= 0.85f)
                        {
                            print("ordered 1 box, 1 egg");
                            foreach (GameObject pack in packageTypes)
                            {
                                if (pack.name.Equals("SquarePackage"))
                                    packagesOrdered.Add(pack);
                                if (pack.name.Equals("EggPackage"))
                                    packagesOrdered.Add(pack);
                            }
                        }
                        else
                        {
                            print("ordered 1 cone, 1 egg");
                            foreach (GameObject pack in packageTypes)
                            {
                                if (pack.name.Equals("ConePackage"))
                                    packagesOrdered.Add(pack);
                                if (pack.name.Equals("EggPackage"))
                                    packagesOrdered.Add(pack);
                            }
                        }
                        break;

                    default:
                        print("error with ordering packages");
                        break;
                }

                //foreach (GameObject i in packagesOrdered)
                //{
                //    print(i);
                //}
                #endregion
                ///////////////////////////////////////////////////

                demandController.maxValue = orderTime;//set the slider's max time value
                demandController.CurrentValue = orderTime;//set the slider's current value

                offScreenIndicator.SetActive(true);//set assigned demand indicator to be active with assigned time
            }
            else//run the delay then place the order
            {
                print(this.name + " needs to wait");
                StartCoroutine(OrderDelay());
            }

            offScreenIndicator.GetComponent<OffScreenIndicator>().OrderTicketUpdate(this);//update the offscreen indicator to display packages
        }
    }

    /// <summary>
    /// Add points to player's score based on the time of package's arrival
    /// </summary>
    public void OrderStatusCheck()
    {
        offScreenIndicator.GetComponent<OffScreenIndicator>().OrderTicketUpdate(this);

        if (packagesOrdered.Count == 0)//check number of packages left to deliver

        {
            OrderFulfilled();
        }
    }

    /// <summary>
    /// When all the packages are delivered as requested, award the points to the player
    /// </summary>
    void OrderFulfilled()
    {
        offScreenIndicator.SetActive(false);

        packageBeenOrdered = false;

        audioSource.clip = successClip;
        audioSource.Play();

        //int pointsToAward = 1;
        //foreach (pointsAtTime p in deliverPointsAtTime)
        //{
        //    if (p.time <= currentTime)
        //        pointsToAward = p.points;
        //}
        gm.packagesDelivered += 1;
        gm.points += numPackagesOrdered;
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
            packageBeenOrdered = false;//toggle the house's package ordered state to false so it knows it can order another package
            numPackagesOrdered = 0;
            gm.refundsOrdered += 1;//increment the refunds ordered variable in the game manager
            orderDelayTime = delayReset;//set the order delay timer so that it will trigger the delay branch in the order method.

            packagesOrdered.Clear();//remove all packages ordered
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
