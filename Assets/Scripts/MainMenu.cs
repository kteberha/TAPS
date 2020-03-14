using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Dropdown resolutionDropdown;
    public GameObject splash, mainMenu, optionsMenu, creditsPanel;
    public GameObject rightEye, leftEye;

    Resolution[] resolutions;

    private void Start()
    {
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
    }

    /// <summary>
    /// Returns to the main menu from options menu
    /// </summary>
    public void OptionsBack()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    /// <summary>
    /// Opens the options panel
    /// </summary>
    public void Options()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    /// <summary>
    /// returns to main menu from credits screen
    /// </summary>
    public void CreditsBack()
    {
        creditsPanel.SetActive(false);
        mainMenu.SetActive(true);
        rightEye.SetActive(true);
        leftEye.SetActive(true);
    }

    /// <summary>
    /// toggles the credits screen and plays the scrolling animation
    /// </summary>
    public void Credits()
    {
        mainMenu.SetActive(false);
        rightEye.SetActive(false);
        leftEye.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void SplashToMenu()
    {
        splash.SetActive(false);
        mainMenu.SetActive(true);
    }

    /// <summary>
    /// Starts the game and loads appropriate data
    /// </summary>
    public void StartGame()
    {
        //will need a section for loading saved
        //
        //

        SceneManager.LoadScene("TutorialScene");
    }

    /// <summary>
    /// Closes the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

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
        print(resolution);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// Sets the graphics quality
    /// </summary>
    /// <param name="qualityIndex"></param>
    public void SetQuality (int qualityIndex)
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
}
