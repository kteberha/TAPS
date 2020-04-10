using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RatingSlider : MonoBehaviour
{
    public Slider slider;
    public Sprite fillingImage;
    public Sprite filledImage;

    public float slideValue = 0f;
    public float maxValue = 1000f;

    public GameObject Fill;

    [SerializeField] int[] _maxStarValues = new int[10];//stores the max values of the star slider depending on workday

    private void OnEnable()
    {
        AsteroidHome.UpdatePackagesDelivered += UpdateSliderValue;
        GameManager.UpdateMaxStarValue += UpdateMaxValue;
    }

    void UpdateMaxValue(int _maxValue)
    {
        slider.maxValue = _maxStarValues[_maxValue];
    }

    /// <summary>
    /// Updates the slider's value based on given value
    /// </summary>
    /// <param name="_addedPoints"></param>
    void UpdateSliderValue(int _addedPoints)
    {
        slider.value += _addedPoints;
    }

    private void OnDisable()
    {
        AsteroidHome.UpdatePackagesDelivered -= UpdateSliderValue;
        GameManager.UpdateMaxStarValue -= UpdateMaxValue;
    }
}
