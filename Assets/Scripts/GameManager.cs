using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GAMESTATE
{
    PAUSED, CLOCKEDIN, CLOCKEDOUT
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

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            print("not in the tutorial");
            //print(PlayerPrefs.GetInt("tutorialDone"));
            //start the tutorial dialogue if it hasn't been done before
            if (PlayerPrefs.GetInt("tutorialDone", 0) <= 0)
            {
                dialogueMenuManager.StartDialogue();
                PlayerPrefs.SetInt("tutorialDone", 1);
            }
        }
        else
        {
            
        }

        state = GAMESTATE.CLOCKEDIN;

        pauseCg = pausePanel.GetComponent<CanvasGroup>();
        workdayStatusText.text = "Clocked IN!";
        textAnimation = workdayStatusText.GetComponent<Animation>();

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
                    state = GAMESTATE.CLOCKEDOUT;
                    clockedOut = true;
                    //have the workday over text appear and fade before loading the scene
                    StartCoroutine(Clockout());
                }
            }
        }
    }

    public IEnumerator Clockout()
    {
        state = GAMESTATE.CLOCKEDOUT;

        workdayStatusText.text = "Clocked OUT!";
        textAnimation.Play();

        yield return new WaitForSeconds(textAnimation["WorkdayStatusAnim"].length);

        menuController.ordersFulfilled = ordersFulfilled;
        menuController.refundsOrdered = refundsOrdered;
        menuController.EndOfDay();

        //StopCoroutine(Clockout());
    }
}