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
    static bool gameInitialized = false;

    public static int[] orderScores = new int[10];//all orders fulfilled scores to be saved
    public static int[] packageScores = new int[10];//all package delivered scores to be saved
    public static int[] refundsScores = new int[10];//all refund scores to be saved

    public int currentOrderHighScore = 0;//instance of order score to be compared and rewritten per workday
    public int currentPackageHighScore = 0;//instance of package delivered score to be compared and rewritten per workday
    public int currentRefundLowScore = 99;//instance of refund score to be compared and rewritten per workday

    public static Action<int> InitializeDay;//delegate to call level initialization.
    public static Action InitializeSettings;//delegate to call setting initialization.
    public static Action<int> UpdateMaxStarValue;//delegate to signal slider's max value updating
    #endregion

    //point and package tracking variables
    public static int packagesDelivered = 0;//tallies number of packages delivered throughout workday
    public static int ordersFulfilled = 0;//tallies number of orders fulfilled throughout workday
    public static int refundsOrdered = 0;//tallies number of refunds ordered throughout workday

    //workday timer variables
    public float timeInWorkday = 0f;//counts the amount of time that's passed in a workday
    public float workdayLength = 301f;//seconds in a workday

    #region ThingsThatNeedMoved
    //Clock in or out UI text variables
    public TextAsset inkEndScript;
    #endregion


    //Pause menu & game state variables
    public static Action onPaused; //delegate to be called when the game is paused
    public static Action onResumed; //delegate to be called when game is resumed
    public static Action WorkdayStarted;//delegate event to be called when day has started
    public static Action WorkdayEnded;//delegate event to be called when day is over
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
        if (!gameInitialized)
        {
            GameInit();
            gameInitialized = true;
        }
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

    void Start()
    {
        state = GAMESTATE.CLOCKEDOUTSTART;

        if (WorkdayStarted != null)
        {
            WorkdayStarted();
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

                    if (WorkdayEnded != null)
                    {
                        WorkdayEnded();
                        print("workday has ended");
                    }
                        //have the workday over text appear and fade before loading the scene
                    //StartCoroutine(Clockout());
                }
            }
        }
    }

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
        {
            print("no new order score");
            return orderScores[_workday];
        }
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
    public static void CreateFiles()
    {
        SaveSystem.CreateNewGameData();
    }

    /// <summary>
    /// Saves all the game data, used when the game closes
    /// </summary>
    public static void SaveAllGameData()
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
    public static GameData LoadAllGameData()
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