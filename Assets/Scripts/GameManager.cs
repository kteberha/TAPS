using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //point and package tracking variables
    public int packagesDelivered = 0;
    public int ordersFulfilled = 0;
    public int refundsOrdered = 0;
    public int points = 0;

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
    public DialogueMenuManager dialogueManager;
    public MenuController menuController;


    // Start is called before the first frame update
    void Start()
    {
        //print(PlayerPrefs.GetInt("tutorialDone"));
        //start the tutorial dialogue if it hasn't been done before
        if(PlayerPrefs.GetInt("tutorialDone", 0) <= 0)
        {
            dialogueManager.StartDialogue();
            PlayerPrefs.SetInt("tutorialDone", 1);
        }

        pauseCg = pausePanel.GetComponent<CanvasGroup>();
        workdayStatusText.text = "Clocked IN!";
        textAnimation = workdayStatusText.GetComponent<Animation>();

    }

    // Update is called once per frame
    void Update()
    {
        //Workday timer
        timeInWorkday += Time.deltaTime;

        //Workday reset
        if (timeInWorkday > workdayLength)
        {
            if (!clockedOut)
            {
                clockedOut = true;
                //have the workday over text appear and fade before loading the scene
                StartCoroutine(Clockout());
            }
        }

    }

    public IEnumerator Clockout()
    {
        workdayStatusText.text = "Clocked OUT!";
        textAnimation.Play();

        yield return new WaitForSeconds(textAnimation["WorkdayStatusAnim"].length);

        menuController.ordersFulfilled = ordersFulfilled;
        menuController.refundsOrdered = refundsOrdered;
        menuController.EndOfDay();

        //StopCoroutine(Clockout());
    }
}