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
    [SerializeField] RatingSlider endDaySlider;

    private void OnEnable()
    {
        AsteroidHome.UpdatePackagesDelivered += UpdateSliderValue;
        AsteroidHome.UpdateRefundPackages += UpdateSliderValue;
    }

    private void Start()
    {
        //sets the end of day slider to match the gameday slider.
        if (endDaySlider != null)
        {
            endDaySlider.maxStarValue = maxStarValue;
            endDaySlider.slider.maxValue = maxStarValue;
        }

        slider.maxValue = maxStarValue;
    }

    /// <summary>
    /// Updates the slider's value based on given value
    /// </summary>
    /// <param name="_addedPoints"></param>
    void UpdateSliderValue(int _addedPoints)
    {
        print($"added {_addedPoints} to slider");
        //print("added points: " + _addedPoints);
        slider.value += _addedPoints;
        //print("New slider value: " + slider.value);
    }

    private void OnDisable()
    {
        AsteroidHome.UpdatePackagesDelivered -= UpdateSliderValue;
        AsteroidHome.UpdateRefundPackages -= UpdateSliderValue;
    }
}
