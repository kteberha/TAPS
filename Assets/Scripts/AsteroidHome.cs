using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsteroidHome : MonoBehaviour
{
    [SerializeField]
    [Range(1, 3)]
    int packageOrderCapacity;//determines the max number of packages a house can request per order
    [SerializeField]
    [Range(0f, 1f)]
    float twoPackagePerc, threePackagePerc;//these are the numbers used to determine the chance of ordering multiple packages

    //Variables regarding the packages
    [HideInInspector]
    public bool packageBeenOrdered = false;//whether house has ordered a package or not
    [HideInInspector]
    public int numPackagesOrdered;//number of packages ordered by a house
    public float countdownTime = 120f;//time before an order expires
    public float orderDelayTime = 5f;//time to delay another order after expired or fulfilled orders

    public float timerBonus = 15f;//number of seconds that will be granted as bonus for delivering a package to a house with multiple packages.
    public float refillSpeed = 1f;//number of seconds for the demand meter to refill when bonus time added.

    Coroutine addTime;//Coroutine to store for looping within the coroutine


    private Dictionary<int, GameObject> _packTypes = new Dictionary<int, GameObject>();
    [SerializeField] private GameObject[] _packageTypes = new GameObject[3];//stores the 3 types of packages
    public List<GameObject> packagesOrdered;//the list of packages the house currently has remaining in their order

    private float _delayReset;//value to reset the delay to

    //variables for offscreen indicator
    public GameObject offScreenIndicatorObj;
    OffScreenIndicator _offScreenIndicator;
    public DemandController demandController;
    [SerializeField] Renderer[] houseRenderers;

    //variables for audio
    [SerializeField] AudioSource OrderedAudioSource, ExpiredAudioSource, SuccessAudioSource;

    bool doOnce = false;

    public static Action<int> UpdateScore;
    public static Action<int> UpdatePackagesDelivered;
    public static Action<int> UpdateRefundPackages;
    public static Action<int> UpdateRefunds;

    [Header("DEMO TUTORIAL")]
    public TutorialManager tutorialManager;



    // Start is called before the first frame update
    void Start()
    {
        //add all of the package objects to the dictionary
        //0 = square, 1 = cone, 2 = egg
        for(int i = 0; i < 3; i++)
        {
            _packTypes.Add(i, _packageTypes[i]);
        }

        offScreenIndicatorObj.SetActive(false);//make sure no offscreen indicator starts turned on
        _delayReset = orderDelayTime;//set the delay timer restart to whatever the user designates
        orderDelayTime = 0f;//set the delay timer to 0 for the very first package orders
        _offScreenIndicator = offScreenIndicatorObj.GetComponent<OffScreenIndicator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(demandController.CurrentValue <= 0)//check if the offscreen indicator value is <= 0
        {
            if (!doOnce)//if the order hasn't called the expired method once
            {
                doOnce = true;//trigger the one time bool
                StartCoroutine(OrderExpired());//set the order to expired
            }
        }
        else//if the current value of offscreen indicator is > 0
        {
            doOnce = false;//ensure the do once bool is false
        }

        //Check if house is rendered so that the direction arrow can be rendered or not
        if (_offScreenIndicator.isActiveAndEnabled)
            foreach(Renderer rend in houseRenderers)
                _offScreenIndicator.CheckHouseRendered(rend);

    }

    /// <summary>
    /// The home places an order for packages to be delivered
    /// </summary>
    public void Order()
    {
        //check that this house hasn't ordered a package already
        if (!packageBeenOrdered)
        {
            //make sure the delay time is 0 so that an order isn't placed right after one is fulfilled or expires
            if (orderDelayTime <= 0)
            {
                //print(this.name + " has ordered successfully");
                OrderedAudioSource.Play();//play the ordered audio clip
                packageBeenOrdered = true;//set order status

                #region NumberOfPackagesToOrder
                float packageNumSeed = UnityEngine.Random.value;

                //decide how many packages should be ordered
                switch(packageOrderCapacity)
                {
                    case 1:
                        numPackagesOrdered = 1;
                        break;
                    case 2:
                        if (packageNumSeed < twoPackagePerc)
                            numPackagesOrdered = 1;
                        else
                            numPackagesOrdered = 2;
                        break;
                    case 3:
                        if (packageNumSeed < twoPackagePerc)
                            numPackagesOrdered = 1;
                        else if (packageNumSeed < threePackagePerc)
                            numPackagesOrdered = 2;
                        else
                            numPackagesOrdered = 3;
                        break;
                }
                #endregion

                #region PackageTypesToSelect
                float packTypeSeed = UnityEngine.Random.value;

                //Create 3 package ordering possibilities
                switch (numPackagesOrdered)
                {
                    #region SinglePackageOrders
                    case 1:
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
                    case 2:
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

                    #region TriplePackageOrders
                    case 3:
                        #region SameOfAKind
                        if (packTypeSeed <= 0.1f)
                        {
                            print("ordered 3 boxes");
                            for (int i = 0; i < 3; i++)
                                packagesOrdered.Add(_packageTypes[0]);
                        }
                        else if (packTypeSeed <= 0.2f)
                        {
                            print("ordered 3 cones");
                            for (int i = 0; i < 3; i++)
                                packagesOrdered.Add(_packageTypes[1]);
                        }
                        else if (packTypeSeed <= 0.3f)
                        {
                            print("ordered 3 eggs");
                            for (int i = 0; i < 3; i++)
                                packagesOrdered.Add(_packageTypes[2]);
                        }
                        #endregion

                        #region TwoOneCombo
                        else if (packTypeSeed <= 0.4f)
                        {
                            print("ordered 2 box 1 cone");
                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packageTypes[0]);
                            packagesOrdered.Add(_packageTypes[1]);
                        }
                        else if (packTypeSeed <= 0.5f)
                        {
                            print("ordered 2 box 1 egg");
                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packageTypes[0]);
                            packagesOrdered.Add(_packageTypes[2]);
                        }
                        else if (packTypeSeed <= 0.6f)
                        {
                            print("ordered 2 cone 1 box");
                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packageTypes[1]);
                            packagesOrdered.Add(_packageTypes[0]);
                        }
                        else if(packTypeSeed <= 0.7f)
                        {
                            print("ordered 2 cone 1 egg");
                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packageTypes[1]);
                            packagesOrdered.Add(_packageTypes[2]);
                        }
                        else if(packTypeSeed <= 0.8f)
                        {
                            print("ordered 2 egg 1 box");
                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packageTypes[2]);
                            packagesOrdered.Add(_packageTypes[0]);
                        }
                        else if(packTypeSeed <= 0.9f)
                        {
                            print("ordered 2 egg 1 cone");
                            for (int i = 0; i < 2; i++)
                                packagesOrdered.Add(_packageTypes[2]);
                            packagesOrdered.Add(_packageTypes[1]);
                        }
                        #endregion

                        #region VariedThreeOrder
                        else
                        {
                            print("ordered 3 of a kind");
                            for (int i = 0; i < 3; i++)
                                packagesOrdered.Add(_packageTypes[i]);
                        }
                        #endregion
                        break;
                    #endregion

                    default:
                        print("error with ordering packages");
                        break;
                }
                #endregion


                demandController.maxValue = countdownTime;//set the slider's max time value
                demandController.CurrentValue = countdownTime;//set the slider's current value
                offScreenIndicatorObj.SetActive(true);//set assigned demand indicator to be active with assigned time
            }
            //run the delay then place the order
            else
            {
                print($"{this.name} needs to wait {orderDelayTime} seconds");
                StartCoroutine(OrderDelay());
            }

            _offScreenIndicator.OrderTicketUpdate(this);//update the offscreen indicator to display packages
        }
    }

    /// <summary>
    /// Check if the house has more packages to be delivered. If not, award points. If yes, award bonus time.
    /// </summary>
    public void OrderStatusCheck()
    {
        _offScreenIndicator.OrderTicketUpdate(this);//update the package ticket image

        //check number of packages left to deliver
        if (packagesOrdered.Count == 0)
        {
            StartCoroutine(OrderFulfilled());//start order fulfilled coroutine
        }
        else
            addTime = StartCoroutine(AddTime(0f));//start the add time coroutine with starting lerp value of 0
    }

    /// <summary>
    /// When all the packages are delivered as requested, award the points to the player
    /// </summary>
    IEnumerator OrderFulfilled()
    {
        offScreenIndicatorObj.transform.Find("Success").gameObject.SetActive(true);//place the check mark on top of the portraits.
        SuccessAudioSource.Play();//play the audio
        packageBeenOrdered = false;//mark the house as able to order packages again
        //call the event that updates the deliveries completed score
        if (UpdateScore != null)
            UpdateScore(1);
        if (UpdatePackagesDelivered != null)
            UpdatePackagesDelivered(numPackagesOrdered);
        orderDelayTime = _delayReset;//set the order delay timer so that it will trigger the delay branch in the order method.

        yield return new WaitForSeconds(2);//wait before disabling the off screen indicator
        offScreenIndicatorObj.transform.Find("Success").gameObject.SetActive(false);//toggle the check mark off
        offScreenIndicatorObj.SetActive(false);//disable the off screen indicator
    }

    /// <summary>
    /// Cancels the current order and notifies the game manager
    /// </summary>
    IEnumerator OrderExpired()
    {
        //Check that the time has run out for the order
        if (demandController.CurrentValue <= 0)
        {
            //toggle the X over the order ticket

            ExpiredAudioSource.Play();//play the audio
            packageBeenOrdered = false;//toggle the house's package ordered state to false so it knows it can order another package
            //call the refund update action
            if (UpdateRefunds != null)
                UpdateRefunds(1);
            if (UpdateRefundPackages != null)
                UpdateRefundPackages(-numPackagesOrdered);
            numPackagesOrdered = 0;//set the number of packages ordered by the house back to 0
            orderDelayTime = _delayReset;//set the order delay timer so that it will trigger the delay branch in the order method.
            packagesOrdered.Clear();//remove all packages ordered
            offScreenIndicatorObj.transform.Find("Failed").gameObject.SetActive(true);//toggle the x to active

            yield return new WaitForSeconds(2);
            offScreenIndicatorObj.SetActive(false);//turn off the offscreen indicator
            offScreenIndicatorObj.transform.Find("Failed").gameObject.SetActive(false);//reset the x to off
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
        orderDelayTime = _delayReset;
    }

    //variables for add time Coroutine
    float _beginValue;
    float _endValue;
    /// <summary>
    /// Refills the demand bar by a bonus amount when a package is delivered
    /// </summary>
    /// <returns></returns>
    IEnumerator AddTime(float _lerpPercent, bool _firstCall = true)
    {
        //set the start and end lerp values the first time this is called
        if (_firstCall)
        {
            _beginValue = demandController.CurrentValue;
            if (_beginValue + timerBonus >= demandController.maxValue)
                _endValue = demandController.maxValue;
            else
                _endValue = _beginValue + timerBonus;
        }

        while (_lerpPercent < 1f)
        {
            demandController.CurrentValue = Mathf.SmoothStep(_beginValue, _endValue, _lerpPercent);//update the demand value with lerp
            _lerpPercent += Time.deltaTime / refillSpeed;//increase the lerp alpha
            yield return new WaitForEndOfFrame();
        }
    }
}
