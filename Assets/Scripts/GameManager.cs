using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GAMESTATE
{
    PAUSED, CLOCKEDIN, CLOCKEDOUTSTART, CLOCKEDOUTEND
}

public enum INPUTTYPE
{
    KEYBOARD, CONTROLLER
}

public class GameManager : MonoBehaviour
{
    public static GAMESTATE state;//for tracking the game state for functionality
    public static INPUTTYPE inputType;//for tracking input type
    public static int workDayIndex = 0;//int for accessing workday values and loading scene indexs(day+1): 0 = workday 1
    public static int workDayActual = 1;//int for loading correct scenes

    #region WorkdayInitVariables
    protected static GameData gameData;

    public static int[] orderScores = new int[10];//all orders fulfilled scores to be saved
    public static int[] packageScores = new int[10];//all package delivered scores to be saved
    public static int[] refundsScores = new int[10];//all refund scores to be saved

    public int currentOrderHighScore;//instance of order score to be compared and rewritten per workday
    public int currentPackageHighScore;//instance of package delivered score to be compared and rewritten per workday
    public int currentRefundLowScore;//instance of refund score to be compared and rewritten per workday

    public static Action<int> InitializeDay;//delegate to call level initialization.
    public static Action InitializeSettings;//delegate to call setting initialization.
    public static Action<int> UpdateMaxStarValue;//delegate to signal slider's max value updating
    #endregion

    //point and package tracking variables
    [HideInInspector] public int packagesDelivered = 0;
    [HideInInspector] public int ordersFulfilled = 0;
    [HideInInspector] public int refundsOrdered = 99;
    [HideInInspector] public int points = 0;

    [HideInInspector] public int bestDeliveryAmount = 0;
    [HideInInspector] public int fewestRefundsAmount = 0;

    //workday timer variables
    public float timeInWorkday = 0f;
    public float workdayLength = 301f;

    #region ThingsThatNeedMoved
    //Clock in or out UI text variables
    public Text workdayStatusText;
    [HideInInspector]public Animation textAnimation;

    //Zip Transition Animations
    public GameObject zipFaceObject;
    public Animator zipAnimator;

    //final points to judge
    CanvasGroup zipAnimCG;
    public TextAsset inkEndScript;
    #endregion


    //Pause menu & game state variables
    public static Action onPaused; //delegate to be called when the game is paused
    public static Action onResumed; //delegate to be called when game is resumed

    public static Action workdayStarted;//delegate event to be called when day has started
    public static Action workdayEnded;//delegate event to be called when day is over
    [HideInInspector]
    public GameObject pausePanel;


    #region TutorialStuff
    ////Tutorial variables
    [Header("Tutorial Variables")]
    public GameObject tutorialManager;

    bool test = false;
    #endregion


    private void Awake()
    {
        GameInit();
    }

    private void OnEnable()
    {
        AsteroidHome.UpdateScore += OrdersFulfilledUpdate;
        AsteroidHome.UpdatePackagesDelivered += PackagesDeliveredUpdate;
        AsteroidHome.UpdateRefunds += RefundsUpdate;
        SceneManager.sceneLoaded += LevelInit;
    }
    private void OnDisable()
    {
        //unsubscribe from events
        AsteroidHome.UpdateScore -= OrdersFulfilledUpdate;
        AsteroidHome.UpdatePackagesDelivered -= PackagesDeliveredUpdate;
        AsteroidHome.UpdateRefunds -= RefundsUpdate;
        SceneManager.sceneLoaded -= LevelInit;
    }

    /// <summary>
    /// Initializes the scene and subscribed scripts based on given workday
    /// </summary>
    void GameInit()
    {
        print("start game init");

        //check if data exists, if not, create new files.
        if (LoadAllGameData() == null)
        {
            Debug.LogWarning("creating new save files");
            CreateFiles();
        }
        else
            LoadAllGameData();


    }

    void LevelInit(Scene _newDay, LoadSceneMode _loadSceneMode = LoadSceneMode.Single)
    {
        print("workday initializing");


        //check that we're not in the main menu
        if (_newDay.buildIndex != 0)
        {
            print($"initialized in workday {_newDay.buildIndex}");
            workDayActual = _newDay.buildIndex;//this gets saved so that the player returns to the correct scene if they quit
            workDayIndex = _newDay.buildIndex - 1;//set the workday int for proper array access index

            currentOrderHighScore = orderScores[workDayIndex];//set the high score comparison variable for future writing
            currentPackageHighScore = packageScores[workDayIndex];//set the high score comarison variable for future writing
            currentRefundLowScore = refundsScores[workDayIndex];//set the low score for comparison variable for future writing

            //set all values that are scored in a workday to 0
            OrdersFulfilledUpdate(0);
            PackagesDeliveredUpdate(0);
            RefundsUpdate(0);
            //////
        }
        else
            print("currently in main menu");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (workdayStatusText != null)
        {
            workdayStatusText.text = "Clocked IN!";//set the animated text object
            textAnimation = workdayStatusText.GetComponent<Animation>();
        }

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            //state = GAMESTATE.CLOCKEDOUTSTART;
            state = GAMESTATE.CLOCKEDIN;
            //StartCoroutine(WakeUp());
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region SAVE/LOADTESTING
        if(Input.GetKeyDown(KeyCode.L))
        {
            print("attempt to load");
            LoadAllGameData();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            print("saved");
            SaveSystem.SaveGameData(gameData);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            print("deleted saves");
            SaveSystem.DeleteAllFiles();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(2);
        }
        #endregion

        //pause & resume game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == GAMESTATE.PAUSED)
            {
                if(onResumed != null)
                {
                    onResumed();
                }
            }
            else
            {
                if (onPaused != null)
                {
                    onPaused();
                }
            }
        }

        //only count the time if the player is clocked in
        if (state == GAMESTATE.CLOCKEDIN)
        {
            //Workday timer
            timeInWorkday += Time.deltaTime;

            //Workday reset
            if (timeInWorkday > workdayLength)
            {
                if (state == GAMESTATE.CLOCKEDIN)
                {
                    state = GAMESTATE.CLOCKEDOUTEND;
                        //have the workday over text appear and fade before loading the scene
                        StartCoroutine(Clockout());
                }
            }
        }
        //clipInfo = zipAnimator.GetCurrentAnimatorClipInfo(0);

    }

    #region NEEDS TO BE MOVED OR HEAVILY ALTERED
    IEnumerator WakeUp()
    {
        zipFaceObject.SetActive(true);
        zipAnimator.SetTrigger("ClockingIn");
        yield return new WaitForSeconds(28.7f);
        zipAnimator.SetTrigger("DonePlaying");
        zipAnimCG.alpha = 0f;

        //DialogueMenuManager.Instance.StartDialogue(GAMESTATE.CLOCKEDOUTSTART);
        StopCoroutine(WakeUp());
    }

    public IEnumerator ShutDown()
    {
        StopCoroutine(ZipResults());
        zipAnimCG.alpha = 1;
        zipAnimator.SetTrigger("ClockingOut");
        //clipInfo = zipAnimator.GetCurrentAnimatorClipInfo(0);
        //print("shut down clip name: " + clipInfo[0].clip.name);

        yield return new WaitForSeconds(2f);
        //FOR DEMO
        //MenuController.Instance.Wishlist();
        //zipAnimCG.alpha = 0f;
        zipFaceObject.SetActive(false);
    }

    public IEnumerator Clockout()
    {
        state = GAMESTATE.CLOCKEDOUTEND;

        workdayStatusText.text = "Clocked OUT!";
        textAnimation.Play();

        yield return new WaitForSeconds(textAnimation["WorkdayStatusAnim"].length);

        ///////FIX THIS/////////////
        //MenuController.Instance.ordersFulfilled = ordersFulfilled;
        //MenuController.Instance.refundsOrdered = refundsOrdered;
        //StartCoroutine(MenuController.Instance.EndDay());
        ////////////////////////
    }

    public IEnumerator ZipResults()
    {
        //NarrativeDialogue.Instance.inkJSONAsset = inkEndScript;
        zipAnimCG.alpha = 1f;
        //zipFaceObject.SetActive(true);
        #region NEEDS TO BE MODULAR
        if (packagesDelivered >= 15)
        {
            zipAnimator.SetTrigger("ExcitedResults");
            print(packagesDelivered);
            print("show excited results");
        }
        else if (packagesDelivered >= 10 && packagesDelivered < 15)
        {
            zipAnimator.SetTrigger("PleasedResults");
            print(packagesDelivered);
            print("show pleased results");
        }
        else if (packagesDelivered >= 5 && packagesDelivered < 10)
        {
            zipAnimator.SetTrigger("DisappointedResults");
            print(packagesDelivered);
            print("show disappointed results");
        }
        else
        {
            zipAnimator.SetTrigger("UghResults");
            print(packagesDelivered);
            print("show ugh results");
        }
        yield return new WaitForSeconds(2.5f);
        zipAnimator.SetTrigger("DonePlaying");
        zipAnimCG.alpha = 0f;
        //DialogueMenuManager.Instance.StartDialogue(GAMESTATE.CLOCKEDOUTEND);
        //StartCoroutine(ShutDown());
        #endregion
    }
    #endregion

    #region SCORE UPDATE METHODS
    /// <summary>
    /// Sets the number of packages delivered in a workday
    /// </summary>
    /// <param name="_numPackages"></param>
    void PackagesDeliveredUpdate(int _numPackages)
    {
        packagesDelivered += _numPackages;
        //print("Total packages delivered (GM): " + packagesDelivered);
    }

    /// <summary>
    /// Sets the number of orders fulfilled in a workday
    /// </summary>
    /// <param name="_numOrders"></param>
    void OrdersFulfilledUpdate(int _numOrders)
    {
        ordersFulfilled += _numOrders;
        //print("Orders fulfilled (GM): "+ ordersFulfilled);
    }

    /// <summary>
    /// Sets the number of refunds ordered in a workday
    /// </summary>
    /// <param name="_numRefunds"></param>
    void RefundsUpdate(int _numRefunds)
    {
        refundsOrdered += _numRefunds;
        //print(refundsOrdered);
    }

    /// <summary>
    /// Comparison check for order scores where workday 1 = 0
    /// </summary>
    /// <param name="_workday"></param>
    /// <param name="_score"></param>
    public static int CheckOrderScore(int _workday, int _score)
    {
        if (orderScores[_workday] < _score)
        {
            print($"new order high score {_score} on day {_workday}");
            return orderScores[_workday] = _score;
        }
        else
            return orderScores[_workday];
    }

    /// <summary>
    /// Comparison check for package scores where workday 1 = 0
    /// </summary>
    /// <param name="_workday"></param>
    /// <param name="_score"></param>
    public static int CheckPackageScore(int _workday, int _score)
    {
        if (packageScores[_workday] < _score)
        {
            print($"new package high score {_score} on day {_workday}");
            return packageScores[_workday] = _score;
        }
        else
            return packageScores[_workday];
    }

    /// <summary>
    /// Comparison check for refund scores where workday 1 = 0
    /// </summary>
    /// <param name="_workday"></param>
    /// <param name="_score"></param>
    public static int CheckRefundScore(int _workday, int _score)
    {
        if (refundsScores[_workday] > _score)
        {
            print($"new refund low score {_score} on day {_workday}");
            return refundsScores[_workday] = _score;
        }
        else
            return refundsScores[_workday];
    }
    #endregion




    #region SAVE & LOADING
    /// <summary>
    /// Creates new files to save and load to with default values.
    /// </summary>
    public void CreateFiles()
    {
        SaveSystem.CreateNewGameData();
    }

    /// <summary>
    /// Saves all the game data, used when the game closes
    /// </summary>
    public void SaveAllGameData()
    {
        gameData.startingWorkday = workDayActual;
        gameData.SetAllOrderScores(orderScores);
        gameData.SetAllPackageScores(packageScores);
        gameData.SetAllRefundScores(refundsScores);
        SaveSystem.SaveGameData(gameData);
        print("GM saved files");
    }

    /// <summary>
    /// Loads all of the game data, used for when the game launches
    /// </summary>
    public GameData LoadAllGameData()
    {
        gameData = SaveSystem.LoadGameData();
        if (gameData != null)
        {
            workDayIndex = gameData.startingWorkday;
            orderScores = gameData.orderHighScores;
            packageScores = gameData.packageHighScores;
            refundsScores = gameData.refundsHighScores;
            return gameData;
        }
        else
        {
            Debug.LogError("No game data file could be loaded");
            return null;
        }
    }
    #endregion
}