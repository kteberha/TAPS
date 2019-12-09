﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkdayTimer : MonoBehaviour
{
    public Slider clockSlider;
    public Text clockText;
    public Image fill;
    public Color startFillColor;
    public Color warningColor;
    public Color finalColor;

    public float warningColorPercent;
    public float finalColorSecValue;
    public float fadeTime;

    public GameManager gm;

    public bool countStarted = true;
    public float countdownValue = 120f;

    Color nextColor;
    Color currentColor;

    float startTime;
    float minute;
    float second;
    string minText;
    string secText;

    private void Start()
    {
        countdownValue = gm.workdayLength;
        clockSlider.maxValue = countdownValue;
        clockSlider.value = countdownValue;

        fill.color = startFillColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (countStarted && countdownValue >= 0f)
        {
            //start the countdown
            countdownValue =Mathf.Clamp(countdownValue - Time.deltaTime, 0f, gm.workdayLength);

            //set the minute and second values
            minute = Mathf.Floor(countdownValue / 60f);
            second = Mathf.Floor(countdownValue) % 60f;

            //add 0's to the strings if they are less than 10
            if (minute < 10)
                minText = "0" + minute.ToString();
            else
                minText = minute.ToString();

            if (second < 10)
                secText = "0" + second.ToString();
            else
                secText = second.ToString();

            if(countdownValue <= finalColorSecValue + 1)
            {
                fill.color = new Color(finalColor.r, finalColor.g, finalColor.b, Mathf.PingPong(Time.time * 1.5f, 1));
                clockText.color = new Color(finalColor.r, finalColor.g, finalColor.b, Mathf.PingPong(Time.time * 1.5f, 1));
            }
            else if(countdownValue / gm.workdayLength <= warningColorPercent + 0.1f)
            {
                //set the starting time for the fade to use
                startTime = gm.workdayLength * warningColorPercent;
                //fade from one color to the other smoothly
                fill.color = Color.Lerp(startFillColor, warningColor, Mathf.SmoothStep(0,1,((Time.time - startTime) / fadeTime)));
            }         
        }
        else
        {
            //stop the counting
            countStarted = false;
            //ensure the timer values stay at 00
            if (countdownValue <= 0)
            {
                minText = "00";
                secText = "00";
            }
        }

        //adjust the slider fill value
        clockSlider.value = countdownValue;
        //format the timer text
        clockText.text = minText + ":" + secText;
    }
}