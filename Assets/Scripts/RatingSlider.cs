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

    private void Start()
    {
        slider.maxValue = maxValue;
    }

    private void Update()
    {
        //make this an event, get this off update//////
        slideValue = GameManager.Instance.points;

        slider.value = slideValue;


        if (slideValue == maxValue)
        {
            Fill.GetComponent<Image>().sprite = filledImage;
        }
        ////////////////////////
    }
}
