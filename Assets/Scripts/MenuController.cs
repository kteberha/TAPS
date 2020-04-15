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
    [SerializeField] GameObject goalsScreen;

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
    [SerializeField] TMPro.TextMeshProUGUI goalsScreenOneStar, goalsScreenTwoStar, goalsScreenThreeStar;

    Animation endDayAnim;
    [HideInInspector] public int ordersFulfilled = 0;
    [HideInInspector] public int packagesDeliveredActual = 0;
    [HideInInspector] public int packagesDeliveredStarRating = 0;
    [HideInInspector] public int refundsOrdered = 0;

    //public bool mapToggled = true;
    public Toggle invertMoveToggle;

    //Zip Transition Animations
    public GameObject zipFaceObject;
    public Animator zipAnimator;
    [SerializeField] Animator screenAnimator;

    //final points to judge
    public CanvasGroup zipAnimCG;
    [SerializeField] RatingSlider starRatingSlider;
    int _threeStarVal, _twoStarVal, _oneStarVal;

    [SerializeField] DialogueMenuManager dialogueManger;

    [SerializeField] Button continueButton;

    private void OnEnable()
    {
        GameManager.onPaused += Pause;
        GameManager.onResumed += Resume;
        GameManager.WorkdayStarted += LevelStart;
        GameManager.WorkdayEnded += EndDaySequence;
        GameManager.InitializeSettings += AssignSettings;
        AsteroidHome.UpdateScore += UpdateOrderScore;
        AsteroidHome.UpdatePackagesDelivered += UpdatePackageRatingScore;
        AsteroidHome.UpdatePackagesDelivered += UpdatePackageTotalScore;
        AsteroidHome.UpdateRefundPackages += UpdatePackageRatingScore;
        AsteroidHome.UpdateRefunds += UpdateRefundScore;
    }
    private void OnDisable()
    {
        GameManager.onPaused -= Pause;
        GameManager.onResumed -= Resume;
        GameManager.WorkdayStarted -= LevelStart;
        GameManager.WorkdayEnded -= EndDaySequence;
        GameManager.InitializeSettings -= AssignSettings;
        AsteroidHome.UpdateScore -= UpdateOrderScore;
        AsteroidHome.UpdatePackagesDelivered -= UpdatePackageRatingScore;
        AsteroidHome.UpdatePackagesDelivered += UpdatePackageTotalScore;
        AsteroidHome.UpdateRefundPackages -= UpdatePackageRatingScore;
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
        dialogueManger = GameObject.Find("Dialogue Menu Manager").GetComponent<DialogueMenuManager>();

        //establish the star rating values
        _threeStarVal = starRatingSlider.maxStarValue;
        _twoStarVal = Mathf.CeilToInt(_threeStarVal * 0.66f);
        _oneStarVal = Mathf.CeilToInt(_threeStarVal * 0.33f);

        //update the goals text values
        goalsScreenThreeStar.text = $"{_threeStarVal} Packages";
        goalsScreenTwoStar.text = $"{_twoStarVal} Packages";
        goalsScreenOneStar.text = $"{_oneStarVal} Packages";

        #region Resolution Setup
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

        #region NEEDSFIXED Check and set movement setting

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

    public void Continue()
    {
        StartCoroutine(ZipResults(true));
    }
    public void Restart()
    {
        StartCoroutine(ZipResults(false));
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

    #region Workday Methods
    public void LevelStart()
    {
        StartCoroutine(WakeUp());
    }

    public void WorkdayStart()
    {
        if (workdayStatusText != null)
        {
            workdayStatusText.text = "Clocked IN!";//set the animated text object
            textAnimation = workdayStatusText.GetComponent<Animation>();
            textAnimation.Play();
        }

        GameManager.state = GAMESTATE.CLOCKEDIN;

        goalsScreen.SetActive(false);
    }

    /// <summary>
    /// Sequence of events after the workday timer reaches 0;
    /// Coroutine nested in method for event call
    /// </summary>
    public void EndDaySequence()
    {
        StartCoroutine(EndDay());
    }

    public IEnumerator EndDay()
    {
        //play the clocked out text animation
        workdayStatusText.text = "Clocked OUT!";//set the animated text object
        textAnimation.Play();
        yield return new WaitForSeconds(textAnimation.clip.length);

        #region Checking For Highscores
        int bestOrdersFulfilled = GameManager.CheckOrderScore(GameManager.workDayIndex, ordersFulfilled);
        print($"best orders this day are {bestOrdersFulfilled}");

        int bestPackagesDelivered = GameManager.CheckPackageScore(GameManager.workDayIndex, packagesDeliveredStarRating);
        print($"best packages delivered this day are: {bestPackagesDelivered}");

        int bestRefundsOrdered = GameManager.CheckRefundScore(GameManager.workDayIndex, refundsOrdered);
        print($"best refunds this day are: {bestRefundsOrdered}");

        ordersFulfilledText.text = $"Orders Fulfilled: {ordersFulfilled}";
        bestOrdersFulfilledText.text = $"Best: {bestOrdersFulfilled}";

        packagesDeliveredText.text = $"Packages Delivered: {packagesDeliveredStarRating}";
        bestPackagesText.text = $"Best: {bestPackagesDelivered}";

        refundsOrderedText.text = $"Deliveries Missed: {refundsOrdered}";
        bestRefundsText.text = $"Best: {bestRefundsOrdered}";
        #endregion

        GameManager.SaveAllGameData();

        //check if player can continue
        if (packagesDeliveredStarRating < _oneStarVal)
        {
            continueButton.enabled = false;
            continueButton.GetComponentInChildren<Text>().text = "";
        }

        GameManager.state = GAMESTATE.CLOCKEDOUTEND;
        endDayAnim.Play();

        yield return new WaitForSeconds(endDayAnim.clip.length);

        endDayCg.blocksRaycasts = true;
        endDayCg.interactable = true;
    }

    #region NEEDS TO BE MOVED OR HEAVILY ALTERED
    IEnumerator WakeUp()
    {
        //Fade screen in
        screenAnimator.SetTrigger("FadeOut");
        zipFaceObject.SetActive(true);
        zipAnimCG.alpha = 1f;
        yield return new WaitForSeconds(Mathf.Ceil(GetAnimationClip(screenAnimator, "ScreenFadeOut").length));

        zipAnimator.SetTrigger("ClockingIn");
        yield return new WaitForSeconds(Mathf.Ceil(GetAnimationClip(zipAnimator, "WakeUpTransition").length));

        zipAnimator.SetTrigger("DonePlaying");
        zipAnimCG.alpha = 0f;

        //toggle the dialogue menu for starting dialogue
        dialogueManger.StartDialogue(GAMESTATE.CLOCKEDOUTSTART);
    }
    /// <summary>
    /// Helper method to get animation clips
    /// </summary>
    /// <param name="_animator"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    AnimationClip GetAnimationClip(Animator _animator, string _name)
    {
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        AnimationClip startClip = null;
        foreach (AnimationClip c in clips)
            if (c.name == _name)
            {
                //print($"Clip: {_name} found");
                startClip = c;
                break;
            }
        if (startClip.name == null)
            Debug.LogWarning($"no clip with name: {_name} exists");

        return startClip;
    }

    /// <summary>
    /// Plays the facial animations for zip based on the score earned.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ZipResults(bool _continue)
    {
        string clipName = "";
        endDayPanel.SetActive(false);
        zipAnimCG.alpha = 1f;
        if (packagesDeliveredStarRating >= _threeStarVal)
        {
            zipAnimator.SetTrigger("ExcitedResults");
            clipName = "ExcitedResultsFace";
            print($"{packagesDeliveredStarRating} >= {_threeStarVal}");
            print("show excited results");
        }
        else if (packagesDeliveredStarRating >= _twoStarVal)// && packagesDelivered < starRatingSlider.maxStarValue)
        {
            zipAnimator.SetTrigger("PleasedResults");
            clipName = "PleasedResultsFace";
            print($"{packagesDeliveredStarRating} >= {_twoStarVal}");
            print("show pleased results");
        }
        else if (packagesDeliveredStarRating >= _oneStarVal)// && packagesDelivered < Mathf.Ceil(starRatingSlider.maxStarValue * (2/3))
        {
            zipAnimator.SetTrigger("UghResults");
            clipName = "UghResultsFace";
            print($"{packagesDeliveredStarRating} >= {_oneStarVal}");
            print("show ugh results");
        }
        else
        {
            zipAnimator.SetTrigger("DisappointedResults");
            clipName = "DisappointedResultsFace";
            print("show disappointed results");
        }
        yield return new WaitForSeconds(Mathf.Ceil(GetAnimationClip(zipAnimator, clipName).length));

        //Fade out and load next level
        screenAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(Mathf.Ceil(GetAnimationClip(screenAnimator, "ScreenFadeIn").length));

        //check if restarting the scene or moving onto the next one.
        if (!_continue)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
        {
            //check if currently on last day
            if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(0);
            }
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    #endregion
    #endregion


    #region Settings Adjustment Methods

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
    public void UpdatePackageRatingScore(int _score)
    {
        packagesDeliveredStarRating += _score;
    }

    public void UpdatePackageTotalScore(int _score)
    {
        packagesDeliveredActual += _score;
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