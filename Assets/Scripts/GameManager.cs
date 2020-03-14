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
    TUTORIAL, WORKDAY1, WORKDAY2, WORKDAY3, WORKDAY4, WORKDAY5
}

public enum INPUTTYPE
{
    KEYBOARD, CONTROLLER
}

public class GameManager : MonoBehaviour
{
    public GAMESTATE state;//for tracking the game state for functionality
    public WORKDAY workDay;//for tracking the workday
    public INPUTTYPE inputType;//for tracking input type

    //point and package tracking variables
    [HideInInspector]public int packagesDelivered = 0;
    [HideInInspector]public int ordersFulfilled = 0;
    [HideInInspector]public int refundsOrdered = 0;
    [HideInInspector]public int points = 0;

    //workday timer variables
    public float timeInWorkday = 0f;
    public float workdayLength = 301f;

    //Clock in or out UI text variables
    public Text workdayStatusText;
    Animation textAnimation;
    bool clockedOut = false;

    //Zip Face Animations
    public GameObject zipFaceObject;
    public Animator zipAnimator;

    //final points to judge
    public Slider starSlider;

    public NarrativeDialogue narrativeDialogue;
    public TextAsset inkEndScript;


    //Pause menu & game state variables
    [HideInInspector]
    public bool paused = false;
    public GameObject pausePanel;
    private CanvasGroup pauseCg;

    ////Tutorial variables
    [Header("Tutorial Variables")]
    public DialogueMenuManager dialogueMenuManager;
    public MenuController menuController;
    public GameObject tutorialManager;

    bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            print("not in the tutorial");
            //print(PlayerPrefs.GetInt("tutorialDone"));
            //start the tutorial dialogue if it hasn't been done before
            //if (PlayerPrefs.GetInt("tutorialDone", 0) <= 0)
            //{
            //    dialogueMenuManager.StartDialogue();
            //    PlayerPrefs.SetInt("tutorialDone", 1);
            //}
        }

        pauseCg = pausePanel.GetComponent<CanvasGroup>();
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
    }


    IEnumerator WakeUp()
    {
        zipFaceObject.SetActive(true);
        AnimatorClipInfo[] clipInfo = zipAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);

        zipFaceObject.SetActive(false);

        dialogueMenuManager.StartDialogue(GAMESTATE.CLOCKEDOUTSTART);

        zipAnimator.SetTrigger("ClockingIn");
    }

    IEnumerator ShutDown()
    {
        zipFaceObject.SetActive(true);
        AnimatorClipInfo[] clipInfo = zipAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);

        //FOR DEMO

    }

    public IEnumerator Clockout()
    {
        state = GAMESTATE.CLOCKEDOUTEND;
        if(starSlider.value >= starSlider.maxValue)
        {
            zipAnimator.SetTrigger("ExcitedResults");
            print("show excited results");
        }
        else if(starSlider.value >= 0.67f && starSlider.value < starSlider.maxValue)
        {
            zipAnimator.SetTrigger("PleasedResults");
            print("show pleased results");
        }
        else if(starSlider.value >= 0.33f && starSlider.value < 0.67f)
        {
            zipAnimator.SetTrigger("DisappointedResults");
            print("show disappointed results");
        }
        else
        {
            zipAnimator.SetTrigger("UghResults");
            print("show ugh results");
        }

        workdayStatusText.text = "Clocked OUT!";
        textAnimation.Play();

        yield return new WaitForSeconds(textAnimation["WorkdayStatusAnim"].length);

        menuController.ordersFulfilled = ordersFulfilled;
        menuController.refundsOrdered = refundsOrdered;
        menuController.EndOfDay();

        //StopCoroutine(Clockout());
    }

    public IEnumerator ZipResults()
    {
        zipFaceObject.SetActive(true);
        AnimatorClipInfo[] clipInfo = zipAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);
        narrativeDialogue.inkJSONAsset = inkEndScript;
        dialogueMenuManager.StartDialogue(GAMESTATE.CLOCKEDOUTEND);
    }
}