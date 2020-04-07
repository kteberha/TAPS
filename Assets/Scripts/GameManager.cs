using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GAMESTATE
{
    PAUSED, CLOCKEDIN, CLOCKEDOUTSTART, CLOCKEDOUTEND
}

public enum WORKDAY
{
    W1, W2, W3, W4, W5, W6, W7, W8, W9, W10
}

public enum INPUTTYPE
{
    KEYBOARD, CONTROLLER
}

public class GameManager : Singleton<GameManager>
{
    public static GAMESTATE state;//for tracking the game state for functionality
    public static WORKDAY workDay;//for tracking the workday
    public static INPUTTYPE inputType;//for tracking input type

    //point and package tracking variables
    [HideInInspector] public int packagesDelivered = 0;
    [HideInInspector] public int ordersFulfilled = 0;
    [HideInInspector] public int refundsOrdered = 0;
    [HideInInspector] public int points = 0;

    [HideInInspector] public int bestDeliveryAmount = 0;
    [HideInInspector] public int fewestRefundsAmount = 0;

    //workday timer variables
    public float timeInWorkday = 0f;
    public float workdayLength = 301f;

    //Clock in or out UI text variables
    public Text workdayStatusText;
    [HideInInspector]public Animation textAnimation;

    //Zip Transition Animations
    public GameObject zipFaceObject;
    public Animator zipAnimator;
    AnimatorClipInfo[] clipInfo;

    //final points to judge
    public Slider starSlider;
    CanvasGroup zipAnimCG;
    public TextAsset inkEndScript;


    //Pause menu & game state variables
    public static Action onPaused; //delegate to be called when the game is paused
    public static Action onResumed; //delegate to be called when game is resumed
    [HideInInspector]
    public GameObject pausePanel;



    ////Tutorial variables
    [Header("Tutorial Variables")]
    public GameObject tutorialManager;

    bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        //zipAnimCG = zipFaceObject.GetComponent<CanvasGroup>();
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            //print(PlayerPrefs.GetInt("tutorialDone"));
            //start the tutorial dialogue if it hasn't been done before
            //if (PlayerPrefs.GetInt("tutorialDone", 0) <= 0)
            //{
            //    dialogueMenuManager.StartDialogue();
            //    PlayerPrefs.SetInt("tutorialDone", 1);
            //}
        }

        workdayStatusText.text = "Clocked IN!";
        textAnimation = workdayStatusText.GetComponent<Animation>();

        //state = GAMESTATE.CLOCKEDOUTSTART;
        state = GAMESTATE.CLOCKEDIN;
        //StartCoroutine(WakeUp());

        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            textAnimation.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
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

        if (state == GAMESTATE.CLOCKEDIN)//only count the time if the player is clocked in
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


    IEnumerator WakeUp()
    {
        zipFaceObject.SetActive(true);
        zipAnimator.SetTrigger("ClockingIn");
        yield return new WaitForSeconds(28.7f);
        zipAnimator.SetTrigger("DonePlaying");
        zipAnimCG.alpha = 0f;

        DialogueMenuManager.Instance.StartDialogue(GAMESTATE.CLOCKEDOUTSTART);
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
        MenuController.Instance.Wishlist();
        //zipAnimCG.alpha = 0f;
        zipFaceObject.SetActive(false);
    }

    public IEnumerator Clockout()
    {
        state = GAMESTATE.CLOCKEDOUTEND;

        workdayStatusText.text = "Clocked OUT!";
        textAnimation.Play();

        yield return new WaitForSeconds(textAnimation["WorkdayStatusAnim"].length);

        MenuController.Instance.ordersFulfilled = ordersFulfilled;
        MenuController.Instance.refundsOrdered = refundsOrdered;
        MenuController.Instance.EndOfDay();

        //StopCoroutine(Clockout());
    }

    public IEnumerator ZipResults()
    {
        NarrativeDialogue.Instance.inkJSONAsset = inkEndScript;
        zipAnimCG.alpha = 1f;
        //zipFaceObject.SetActive(true);

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
        DialogueMenuManager.Instance.StartDialogue(GAMESTATE.CLOCKEDOUTEND);
        //StartCoroutine(ShutDown());
    }
}