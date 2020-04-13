using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RatingSlider : MonoBehaviour
{
    public Slider slider;
    public Sprite fillingImage;
    public Sprite filledImage;

    public GameObject Fill;

    public int maxStarValue;

    private void OnEnable()
    {
        AsteroidHome.UpdatePackagesDelivered += UpdateSliderValue;
    }

    /// <summary>
    /// Updates the slider's value based on given value
    /// </summary>
    /// <param name="_addedPoints"></param>
    void UpdateSliderValue(int _addedPoints)
    {

        //print("added points: " + _addedPoints);
        slider.value += _addedPoints;
        //print("New slider value: " + slider.value);
    }

    private void OnDisable()
    {
        AsteroidHome.UpdatePackagesDelivered -= UpdateSliderValue;
    }
}
