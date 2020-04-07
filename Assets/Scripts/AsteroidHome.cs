using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsteroidHome : MonoBehaviour
{
    //Variables regarding the packages
    [HideInInspector]
    public bool packageBeenOrdered = false;//whether house has ordered a package or not
    [HideInInspector]
    public int numPackagesOrdered;//number of packages ordered by a house
    public float orderTime = 120f;//time before an order expires
    public float orderDelayTime = 5f;//time to delay another order after expired or fulfilled orders
    public int points;//number of points the house will award for completing an order

    private Dictionary<int, GameObject> _packTypes = new Dictionary<int, GameObject>();
    [SerializeField]
    private GameObject[] _packageTypes = new GameObject[3];
    public List<GameObject> packagesOrdered;


    private float delayReset;//value to reset the delay to

    [Header("DEMO TUTORIAL")]
    public TutorialManager tutorialManager;

    //variables for offscreen indicator
    public GameObject offScreenIndicatorObj;
    OffScreenIndicator offScreenIndicator;
    public DemandController demandController;
    [SerializeField] Renderer[] houseRenderers;


    //variables for audio
    [SerializeField] AudioSource OrderedAudioSource, ExpiredAudioSource, SuccessAudioSource;

    bool doOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        //add all of the package objects to the dictionary
        //0 = square, 1 = cone, 2 = egg
        for(int i = 0; i < 3; i++)
        {
            _packTypes.Add(i, _packageTypes[i]);
        }
        offScreenIndicatorObj.SetActive(false);
        delayReset = orderDelayTime;//set the delay timer restart to whatever the user designates
        orderDelayTime = 0f;//set the delay timer to 0 for the very first package orders
        offScreenIndicator = offScreenIndicatorObj.GetComponent<OffScreenIndicator>();
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

        //Check if house is rendered so that the direction arrow can be rendered or not
        if (offScreenIndicator.isActiveAndEnabled)
            foreach(Renderer rend in houseRenderers)
                offScreenIndicator.CheckHouseRendered(rend);

    }

    /// <summary>
    /// The home places an order for packages to be delivered
    /// </summary>
    public void Order()
    {
        //check that this house hasn't ordered a package already
        if (packageBeenOrdered == false)
        {
            //make sure the delay time is 0 so that an order isn't placed right after one is fulfilled or expires
            if (orderDelayTime <= 0)
            {
                //print(this.name + " has ordered successfully");
                OrderedAudioSource.Play();//play the ordered audio clip
                packageBeenOrdered = true;//set order status

                #region NumberOfPackagesToOrder
                float packageNumSeed = Random.value;
                if (packageNumSeed <= 0.45f)
                    numPackagesOrdered = 1;
                else
                    numPackagesOrdered = 2;
                #endregion

                #region PackageTypesToSelect
                float packTypeSeed = Random.value;

                switch (numPackagesOrdered)
                {
                    #region SinglePackageOrders
                    case (1):
                        if (packTypeSeed <= 0.3f)
                        {
                            print("ordered 1 Square");
                            packagesOrdered.Add(_packTypes[0]);
                        }
                        else if (packTypeSeed <= 0.6)
                        {
                            print("ordered 1 Cone");
                            packagesOrdered.Add(_packTypes[1]);
                        }
                        else
                        {
                            print("ordered 1 Egg");
                            packagesOrdered.Add(_packTypes[2]);
                        }
                        break;
                    #endregion

                    #region DoublePackageOrders
                    case (2):
                        #region RepeatedPackageTypes
                        if (packTypeSeed <= 0.17f)
                        {
                            print("ordered 2 Boxes");
                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packTypes[0]);
                        }
                        else if(packTypeSeed <= 0.34f)
                        {
                            print("ordered 2 Cones");
                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packTypes[1]);
                        }
                        else if(packTypeSeed <= 0.51f)
                        {
                            print("ordered 2 Eggs");

                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packTypes[2]);
                        }
                        #endregion

                        #region VariedPackageTypes
                        else if (packTypeSeed <= 0.68f)
                        {
                            print("ordered 1 box, 1 cone");
                            packagesOrdered.Add(_packTypes[0]);
                            packagesOrdered.Add(_packTypes[1]);
                        }

                        else if (packTypeSeed <= 0.85f)
                        {
                            print("ordered 1 box, 1 egg");
                            packagesOrdered.Add(_packTypes[0]);
                            packagesOrdered.Add(_packTypes[2]);
                        }
                        else
                        {
                            print("ordered 1 cone, 1 egg");
                            packagesOrdered.Add(_packTypes[1]);
                            packagesOrdered.Add(_packTypes[2]);
                        }
                        break;
                        #endregion
                    #endregion

                    default:
                        print("error with ordering packages");
                        break;
                }
                #endregion


                demandController.maxValue = orderTime;//set the slider's max time value
                demandController.CurrentValue = orderTime;//set the slider's current value

                offScreenIndicatorObj.SetActive(true);//set assigned demand indicator to be active with assigned time
            }
            //run the delay then place the order
            else
            {
                print(this.name + " needs to wait");
                StartCoroutine(OrderDelay());
            }

            offScreenIndicatorObj.GetComponent<OffScreenIndicator>().OrderTicketUpdate(this);//update the offscreen indicator to display packages
        }
    }

    /// <summary>
    /// Add points to player's score based on the time of package's arrival
    /// </summary>
    public void OrderStatusCheck()
    {
        offScreenIndicatorObj.GetComponent<OffScreenIndicator>().OrderTicketUpdate(this);

        //check number of packages left to deliver
        if (packagesOrdered.Count == 0)
        {
            OrderFulfilled();

            //////////////FOR DEMO TUTORIAL SCENE////////////////
            if (SceneManager.GetActiveScene().name == "TutorialScene")
            {
                orderDelayTime = 0f;
                Order();
            }
            /////////////////////////////////////////////////////////
        }

        ///Tutorial stuff///////
        if(SceneManager.GetActiveScene().name == "TutorialScene")
        {
            if(tutorialManager.i == 24)
            {
                tutorialManager.i++;
                tutorialManager.ToggleDialogueOn();
            }
        }
        ///////////////////////////////////////////////////////
    }

    /// <summary>
    /// When all the packages are delivered as requested, award the points to the player
    /// </summary>
    void OrderFulfilled()
    {
        offScreenIndicatorObj.SetActive(false);

        packageBeenOrdered = false;

        SuccessAudioSource.Play();

        GameManager.Instance.packagesDelivered += numPackagesOrdered;//reward based on number of packages delivered
        GameManager.Instance.points += numPackagesOrdered;//reward based on the number of packages delivered
        GameManager.Instance.ordersFulfilled += 1;

        orderDelayTime = delayReset;//set the order delay timer so that it will trigger the delay branch in the order method.
    }

    /// <summary>
    /// Cancels the current order and notifies the game manager
    /// </summary>
    public void OrderExpired()
    {
        //Check that the time has run out for the order
        if (demandController.CurrentValue <= 0)
        {
            ExpiredAudioSource.Play();//play the audio
            offScreenIndicatorObj.SetActive(false);//turn off the offscreen indicator
            packageBeenOrdered = false;//toggle the house's package ordered state to false so it knows it can order another package
            numPackagesOrdered = 0;
            GameManager.Instance.refundsOrdered += 1;//increment the refunds ordered variable in the game manager
            orderDelayTime = delayReset;//set the order delay timer so that it will trigger the delay branch in the order method.

            packagesOrdered.Clear();//remove all packages ordered
        }

        ///Special condition only for the tutorial scene
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            orderDelayTime = 0;
            Order();
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
        orderDelayTime = delayReset;
    }
}
