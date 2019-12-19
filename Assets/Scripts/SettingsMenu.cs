using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Dropdown resolutionDropdown;

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

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
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
