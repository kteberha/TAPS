using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameManager gm;

    [HideInInspector]public bool paused = false;

    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject endDayPanel;
    //////DEMO ONLY/////
    public GameObject wishlist;
    ///////////////////
    //public GameObject map;
    CanvasGroup pauseCg;
    CanvasGroup optionsCg;
    CanvasGroup endDayCg;

    public Text ordersFulfilledText;
    public Text refundsOrderedText;
    public Text bestOrdersFulfilledText;
    public Text bestRefundsText;

    Animation endDayAnim;
    [HideInInspector] public int ordersFulfilled = 0;
    [HideInInspector] public int refundsOrdered = 0;

    public bool invertedMovement = false;
    //public bool mapToggled = true;
    public Toggle invertMoveToggle;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        pauseCg = pausePanel.GetComponent<CanvasGroup>();
        optionsCg = optionsPanel.GetComponent<CanvasGroup>();
        endDayCg = endDayPanel.GetComponent<CanvasGroup>();
        endDayAnim = endDayPanel.GetComponent<Animation>();

        //get and apply saved input options if any exist
        if (!PlayerPrefs.HasKey("InvertedMovement"))
        {
            PlayerPrefs.SetInt("InvertedMovement", 0);
            PlayerPrefs.Save();
            invertedMovement = false;
        }
        else if (PlayerPrefs.GetInt("InvertedMovement") == 0)
        {
            invertedMovement = false;
        }
        else
        {
            invertedMovement = true;
        }
        invertMoveToggle.SetIsOnWithoutNotify(invertedMovement);
    }

    // Update is called once per frame
    void Update()
    {
        //need to make this unviseral key press for controller and keyboard
        if (Input.GetKeyDown(KeyCode.Escape))//pause or unpause the game on a key press
        {
            if (gm.state == GAMESTATE.PAUSED)
                Resume();
            else
                Pause();
        }

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    ToggleMap();
        //}
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseCg.alpha = 1f;
        pauseCg.interactable = true;
        pauseCg.blocksRaycasts = true;
        optionsCg.blocksRaycasts = false;
        optionsCg.interactable = false;
        paused = true;
        gm.state = GAMESTATE.PAUSED;
    }

    /// <summary>
    /// resume the game when the button is pressed
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;
        pauseCg.interactable = false;
        pauseCg.blocksRaycasts = false;
        pauseCg.alpha = 0f;
        optionsCg.interactable = false;
        optionsCg.alpha = 0f;
        paused = false;
        gm.state = GAMESTATE.CLOCKEDIN;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// option the options menu
    /// </summary>
    public void Options()
    {
        pauseCg.alpha = 0f;
        pauseCg.interactable = false;
        pauseCg.blocksRaycasts = false;
        optionsCg.alpha = 1f;
        optionsCg.interactable = true;
        optionsCg.blocksRaycasts = true;
    }

    ///<summary>
    ///inverts the player movement controls
    /// </summary>
    public void InvertMovement()
    {
        if (invertedMovement)
        {
            invertedMovement = false;
            PlayerPrefs.SetInt("InvertedMovement", 0);
            PlayerPrefs.Save();
        }
        else
        {
            invertedMovement = true;
            PlayerPrefs.SetInt("InvertedMovement", 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// returns to the pause menu from the options menu
    /// </summary>
    public void Back()
    {
        pauseCg.alpha = 1f;
        pauseCg.interactable = true;
        pauseCg.blocksRaycasts = true;
        optionsCg.alpha = 0f;
        optionsCg.interactable = false;
        optionsCg.blocksRaycasts = false;
    }

    public void EndOfDay()
    {
        //Check for orders completed high scores and adjust accordingly
        if(PlayerPrefs.GetInt("OrderBest") < ordersFulfilled)
        {
            PlayerPrefs.SetInt("OrderBest", ordersFulfilled);
            //print("new best orders fulfilled");
        }

        //check for refunds placed (lowest) high scores and adjust accordingly
        if (PlayerPrefs.GetInt("RefundsBest") > refundsOrdered)
        {
            PlayerPrefs.SetInt("RefundsBest", refundsOrdered);
            //print("new best refunds avoided");
        }

        ordersFulfilledText.text = "Orders Fulfilled: " + ordersFulfilled.ToString();
        bestOrdersFulfilledText.text = "Best: " + PlayerPrefs.GetInt("OrderBest").ToString();
        refundsOrderedText.text = " Refunds Ordered: " + refundsOrdered.ToString();
        bestRefundsText.text = "Best: " + PlayerPrefs.GetInt("RefundsBest").ToString();

        StartCoroutine(EndDay());
    }

    IEnumerator EndDay()
    {
        paused = true;
        endDayAnim.Play();
        yield return new WaitForSeconds(endDayAnim.clip.length);
        Time.timeScale = 0f;
        endDayCg.blocksRaycasts = true;
        endDayCg.interactable = true;
        gm.state = GAMESTATE.CLOCKEDOUT;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quit the game in editor and build
    /// </summary>
    public void QuitGame()
    {
        //needs removed/commented out when making builds
        //UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
        //

        Application.Quit();

        //remove for official development
        //for testing purposes, reset the tutorial check
        PlayerPrefs.DeleteKey("tutorialDone");
        //
        PlayerPrefs.DeleteKey("InvertedMovement");
    }


    ////////////////FOR DEMO ONLY/////////////////////////
    public void Wishlist()
    {
        gm.state = GAMESTATE.PAUSED;
        wishlist.SetActive(true);
    }
}