using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [HideInInspector]
    public bool paused = false;
    public GameObject pausePanel;
    public GameObject optionsPanel;
    //public GameObject map;
    CanvasGroup pauseCg;
    CanvasGroup optionsCg;

    public bool invertedMovement;
    //public bool mapToggled = true;
    public Toggle invertMoveToggle;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        pauseCg = pausePanel.GetComponent<CanvasGroup>();
        optionsCg = optionsPanel.GetComponent<CanvasGroup>();

        //get saved input options if any exist
        if (!PlayerPrefs.HasKey("InvertedMovement"))
        {
            PlayerPrefs.SetInt("InvertedMovement", 1);
            PlayerPrefs.Save();
            invertedMovement = true;
            print("movement set to default");
        }
        else if (PlayerPrefs.GetInt("InvertedMovement") == 0)
        {
            invertedMovement = false;
            print("should be non-invert");
        }
        else
        {
            invertedMovement = true;
            print("should be invert");
        }

        invertMoveToggle.SetIsOnWithoutNotify(invertedMovement);
    }

    // Update is called once per frame
    void Update()
    {
        //pause the game on Espcape press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
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
        print("invert movement being called: " + invertedMovement);
        print("player pref value: " + PlayerPrefs.GetInt("InvertedMovement"));

        if (invertedMovement)
        {
            invertedMovement = false;
            PlayerPrefs.SetInt("InvertedMovement", 0);
            PlayerPrefs.Save();
            print("movement has been set to non-invert");

            print("invertedMovement now = 0");
        }
        else
        {
            invertedMovement = true;
            PlayerPrefs.SetInt("InvertedMovement", 1);
            PlayerPrefs.Save();
            print("movement set to invert");

            print("invertedMovement now = 1");
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
        PlayerPrefs.SetInt("tutorialDone", 0);
        //
        //PlayerPrefs.DeleteKey("InvertedMovement");
    }
}
