using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    [HideInInspector]public bool paused = false;

    #region COMING IN FROM MAINMENU
    public AudioMixer masterMixer;
    public static float sfxVolume, musicVolume;//these are for saving to settings
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;
    #endregion

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
    [HideInInspector] public int packagesDelivered = 0;
    [HideInInspector] public int refundsOrdered = 0;

    public static bool invertedMovement = false;
    //public bool mapToggled = true;
    public Toggle invertMoveToggle;


    private void OnEnable()
    {
        //subscribe to gamemanager's pause and resume events
        GameManager.onPaused += Pause;
        GameManager.onResumed += Resume;
        GameManager.InitializeSettings += AssignSettings;
    }

    // Start is called before the first frame update
    void Start()
    {
        //get components
        pauseCg = pausePanel.GetComponent<CanvasGroup>();
        optionsCg = optionsPanel.GetComponent<CanvasGroup>();
        endDayCg = endDayPanel.GetComponent<CanvasGroup>();
        endDayAnim = endDayPanel.GetComponent<Animation>();

        #region COMING IN FROM MAIN MENU
        //get all of the resolutions available to display
        resolutions = Screen.resolutions;

        //Make sure the dropdown doesn't duplicate values
        resolutionDropdown.ClearOptions();

        //create a list to store all of the resolutions as strings (needs to have adjustable size)
        List<string> options = new List<string>();

        //current index variable for storing which value is the default
        int currentResolutionIndex = 0;

        //add all the resolutions into the list in a formatted string
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            //check to see if the resolution being added equals the one currently in use
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        //add the list of strings to the dropdown
        resolutionDropdown.AddOptions(options);
        //set the current dropdown value to the current resolution value
        resolutionDropdown.value = currentResolutionIndex;
        //Refresh the dropdown so it displays the correct value on start
        resolutionDropdown.RefreshShownValue();
        #endregion

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

    void AssignSettings()
    {

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
        GameManager.state = GAMESTATE.PAUSED;
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
        GameManager.state = GAMESTATE.CLOCKEDIN;
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

    public IEnumerator EndDay()
    {
        int bestOrdersFulfilled = GameManager.CheckOrderScore(GameManager.workDayIndex, ordersFulfilled);
        int bestPackagesDelivered = GameManager.CheckPackageScore(GameManager.workDayIndex, packagesDelivered);
        int bestRefundsOrdered = GameManager.CheckRefundScore(GameManager.workDayIndex, refundsOrdered);

        ordersFulfilledText.text = "Orders Fulfilled: " + ordersFulfilled.ToString();
        bestOrdersFulfilledText.text = "Best: " + PlayerPrefs.GetInt("OrderBest").ToString();
        refundsOrderedText.text = " Refunds Ordered: " + refundsOrdered.ToString();
        bestRefundsText.text = "Best: " + PlayerPrefs.GetInt("RefundsBest").ToString();

        GameManager.state = GAMESTATE.CLOCKEDOUTEND;
        //paused = true;
        endDayAnim.Play();
        yield return new WaitForSeconds(endDayAnim.clip.length);
        //Time.timeScale = 0f;
        endDayCg.blocksRaycasts = true;
        endDayCg.interactable = true;
    }

    public void ShowResultFace()
    {
        //GameManager.Instance.StartCoroutine(GameManager.Instance.ZipResults());
        endDayPanel.SetActive(false);
    }

    public void ReturnToMenu()
    {
        //GameManager.Instance.SaveAllPlayerData();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Quit the game in editor and build
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    #region CAME IN FROM MAIN MENU

    /// <summary>
    /// Adjusts the music master mixer
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("MasterMusicVolume", volume);
    }

    /// <summary>
    /// Adjusts the sound effect master mixer
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("MasterSFXVolume", volume);
    }

    /// <summary>
    /// Sets the game's resolution
    /// </summary>
    /// <param name="resolutionIndex"></param>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// Sets the graphics quality
    /// </summary>
    /// <param name="qualityIndex"></param>
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    /// <summary>
    /// Toggles full screen mode
    /// </summary>
    /// <param name="isFullscreen"></param>
    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    #endregion


    private void OnDisable()
    {
        //subscribe to gamemanager's pause and resume events
        GameManager.onPaused -= Pause;
        GameManager.onResumed -= Resume;
        GameManager.InitializeSettings -= AssignSettings;
    }


    ////////////////FOR DEMO ONLY/////////////////////////
    public void Wishlist()
    {
        GameManager.state = GAMESTATE.CLOCKEDOUTEND;
        wishlist.SetActive(true);
    }
}