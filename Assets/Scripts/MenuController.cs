using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    protected static OptionsData optionsData;

    [HideInInspector]public bool paused = false;

    public AudioMixer masterMixer;
    public static bool invertedMovement = false;
    public static float sfxVolume, musicVolume;//these are for saving to settings
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;

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

    [SerializeField] Text workdayStatusText;
    [HideInInspector] public Animation textAnimation;

    public Text ordersFulfilledText, packagesDeliveredText, refundsOrderedText;
    public Text bestOrdersFulfilledText, bestPackagesText, bestRefundsText;

    Animation endDayAnim;
    [HideInInspector] public int ordersFulfilled = 0;
    [HideInInspector] public int packagesDelivered = 0;
    [HideInInspector] public int refundsOrdered = 0;

    //public bool mapToggled = true;
    public Toggle invertMoveToggle;

    //Zip Transition Animations
    public GameObject zipFaceObject;
    public Animator zipAnimator;

    //final points to judge
    public CanvasGroup zipAnimCG;
    [SerializeField] RatingSlider starRatingSlider;
    int _threeStarVal, _twoStarVal, _oneStarVal;


    private void OnEnable()
    {
        GameManager.onPaused += Pause;
        GameManager.onResumed += Resume;
        GameManager.WorkdayStarted += WorkdayStart;
        GameManager.WorkdayEnded += EndDaySequence;
        GameManager.InitializeSettings += AssignSettings;
        AsteroidHome.UpdateScore += UpdateOrderScore;
        AsteroidHome.UpdatePackagesDelivered += UpdatePackageScore;
        AsteroidHome.UpdateRefunds += UpdateRefundScore;
    }
    private void OnDisable()
    {
        GameManager.onPaused -= Pause;
        GameManager.onResumed -= Resume;
        GameManager.WorkdayStarted -= WorkdayStart;
        GameManager.WorkdayEnded -= EndDaySequence;
        GameManager.InitializeSettings -= AssignSettings;
        AsteroidHome.UpdateScore -= UpdateOrderScore;
        AsteroidHome.UpdatePackagesDelivered -= UpdatePackageScore;
        AsteroidHome.UpdateRefunds -= UpdateRefundScore;
    }

    // Start is called before the first frame update
    void Start()
    {
        //get components
        pauseCg = pausePanel.GetComponent<CanvasGroup>();
        optionsCg = optionsPanel.GetComponent<CanvasGroup>();
        endDayCg = endDayPanel.GetComponent<CanvasGroup>();
        endDayAnim = endDayPanel.GetComponent<Animation>();
        _threeStarVal = starRatingSlider.maxStarValue;
        _twoStarVal = Mathf.CeilToInt(starRatingSlider.maxStarValue * (2 / 3));
        _oneStarVal = Mathf.CeilToInt(starRatingSlider.maxStarValue * (1 / 3));

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

        #region NEEDSFIXED

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
        #endregion
        invertMoveToggle.SetIsOnWithoutNotify(invertedMovement);
    }

    void AssignSettings()
    {

    }

    public void WorkdayStart()
    {

        if (workdayStatusText != null)
        {
            workdayStatusText.text = "Clocked IN!";//set the animated text object
            textAnimation = workdayStatusText.GetComponent<Animation>();
        }
    }

    #region MenuMethods
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameManager.workDayActual);
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        //check if currently on last day
        if (++GameManager.workDayActual > SceneManager.sceneCount)
        {
            print("on last level, go back to main menu");
            SceneManager.LoadScene(0);
        }
        else
        {
            GameManager.workDayActual++;
            SceneManager.LoadScene(GameManager.workDayActual);
        }
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
            //PlayerPrefs.SetInt("InvertedMovement", 0);
            //PlayerPrefs.Save();
        }
        else
        {
            invertedMovement = true;
            //PlayerPrefs.SetInt("InvertedMovement", 1);
            //PlayerPrefs.Save();
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

        //need to save options settings here
        SaveAllSettings();
        ///
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
    #endregion

    /// <summary>
    /// Sequence of events to start the workday
    /// </summary>
    public void StartDaySequence()
    {
        StartCoroutine(WakeUp());
    }

    /// <summary>
    /// Sequence of events after the workday timer reaches 0;
    /// </summary>
    public void EndDaySequence()
    {
        StartCoroutine(EndDay());
    }

    public IEnumerator EndDay()
    {
        int bestOrdersFulfilled = GameManager.CheckOrderScore(GameManager.workDayIndex, ordersFulfilled);
        print($"best orders this day are {bestOrdersFulfilled}");

        int bestPackagesDelivered = GameManager.CheckPackageScore(GameManager.workDayIndex, packagesDelivered);
        print($"best packages delivered this day are: {bestPackagesDelivered}");

        int bestRefundsOrdered = GameManager.CheckRefundScore(GameManager.workDayIndex, refundsOrdered);
        print($"best refunds this day are: {bestRefundsOrdered}");

        ordersFulfilledText.text = $"Orders Fulfilled: {ordersFulfilled}";
        bestOrdersFulfilledText.text = $"Best: {bestOrdersFulfilled}";

        packagesDeliveredText.text = $"Packages Delivered: {packagesDelivered}";
        bestPackagesText.text = $"Best: {bestPackagesDelivered}";

        refundsOrderedText.text = $"Refunds Ordered: {refundsOrdered}";
        bestRefundsText.text = $"Best: {bestRefundsOrdered}";

        GameManager.SaveAllGameData();

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
        StartCoroutine(ZipResults());
        endDayPanel.SetActive(false);
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

    /// <summary>
    /// Plays the facial animations for zip based on the score earned.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ZipResults()
    {
        zipAnimCG.alpha = 1f;
        if (packagesDelivered >= _threeStarVal)
        {
            zipAnimator.SetTrigger("ExcitedResults");
            print(packagesDelivered);
            print("show excited results");
        }
        else if (packagesDelivered >= _twoStarVal)// && packagesDelivered < starRatingSlider.maxStarValue)
        {
            zipAnimator.SetTrigger("PleasedResults");
            print(packagesDelivered);
            print("show pleased results");
        }
        else if (packagesDelivered >= _oneStarVal)// && packagesDelivered < Mathf.Ceil(starRatingSlider.maxStarValue * (2/3))
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
    }
    #endregion


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

    #region Score Methods
    public void UpdateOrderScore(int _score)
    {
        ordersFulfilled += _score;
    }
    public void UpdatePackageScore(int _score)
    {
        packagesDelivered += _score;
    }
    public void UpdateRefundScore(int _score)
    {
        refundsOrdered += _score;
    }
    #endregion

    #region Save/Load Methods
    public void CreateFiles()
    {
        SaveSystem.CreateNewSettingsData();
    }

    public void SaveAllSettings()
    {
        optionsData.ChangeMovementSettings(invertedMovement);
        optionsData.ChangeMusicVolume(musicVolume);
        optionsData.ChangeSfxVolume(sfxVolume);
        SaveSystem.SaveOptionsData(optionsData);
        print("saved settings");
    }

    public OptionsData LoadAllSettings()
    {
        optionsData = SaveSystem.LoadOptionsData();
        if(optionsData != null)
        {
            print("found file to load");
            invertedMovement = optionsData.invertedMovement;
            musicVolume = optionsData.musicVolume;
            sfxVolume = optionsData.sfxVolume;
            return optionsData;
        }
        else
        {
            Debug.LogWarning("no options file to load");
            return null;
        }
    }
    #endregion




    ////////////////FOR DEMO ONLY/////////////////////////
    public void Wishlist()
    {
        GameManager.state = GAMESTATE.CLOCKEDOUTEND;
        wishlist.SetActive(true);
    }
}