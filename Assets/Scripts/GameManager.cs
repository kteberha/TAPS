﻿using System.Collections;
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
    TUTORIAL, WORKDAY1, WORKDAY2, WORKDAY3, WORKDAY4, WORKDAY5
}

public enum INPUTTYPE
{
    KEYBOARD, CONTROLLER
}

public class GameManager : MonoSingleton<GameManager>
{
    public GAMESTATE state;//for tracking the game state for functionality
    public WORKDAY workDay;//for tracking the workday
    public INPUTTYPE inputType;//for tracking input type

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
    bool clockedOut = false;

    //Zip Transition Animations
    public GameObject zipFaceObject;
    public Animator zipAnimator;
    AnimatorClipInfo[] clipInfo;

    //////might not need these/////
    int wakeUpHash = Animator.StringToHash("Base Layer.WakeUpTransition");
    int excitedHash = Animator.StringToHash("Base Layer.ExcitedResults");
    int pleasedHash = Animator.StringToHash("Base Layer.PleasedResults");
    int disappointedHash = Animator.StringToHash("Base Layer.DisappointedResults");
    int ughHash = Animator.StringToHash("Base Layer.UghResults");
    int clockOutHash = Animator.StringToHash("Base Layer.ClockoutTransition");
    ////////////////////////////

    //final points to judge
    public Slider starSlider;
    CanvasGroup zipAnimCG;
    public TextAsset inkEndScript;


    //Pause menu & game state variables
    [HideInInspector]
    public bool paused = false;
    public GameObject pausePanel;



    ////Tutorial variables
    [Header("Tutorial Variables")]
    public GameObject tutorialManager;

    bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        zipAnimCG = zipFaceObject.GetComponent<CanvasGroup>();
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

        state = GAMESTATE.CLOCKEDOUTSTART;

        StartCoroutine(WakeUp());

        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            textAnimation.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {

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
                    clockedOut = true;
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