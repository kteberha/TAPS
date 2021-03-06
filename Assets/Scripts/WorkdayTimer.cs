﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkdayTimer : MonoBehaviour
{
    private GameManager gm;

    //public Slider clockSlider;
    public Text clockText;
    //public Image fill;
    public Color startFillColor;
    public Color warningColor;
    public Color finalColor;

    public float warningColorPercent;
    public float finalColorSecValue;
    public float fadeTime;

    public bool countStarted = true;

    Color nextColor;
    Color currentColor;

    Animation fadeAnimation;
    Animation clockTextAnim;

    float countdownValue;
    float startTime;
    float minute;
    float second;
    string minText;
    string secText;
    bool oneTimeFadeStarted = false;
    bool fadeAnimationStarted = false;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        countdownValue = gm.workdayLength;
        //clockSlider.maxValue = countdownValue;
        //clockSlider.value = countdownValue;

        //fill.color = startFillColor;

        fadeAnimation = GetComponent<Animation>();
        clockTextAnim = GetComponent<Animation>();

        StartCoroutine(ClockFade());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.state == GAMESTATE.CLOCKEDIN)
        {
            ////a lot of this needs to be taken off update////
            if (countStarted && countdownValue >= 0f)//run countdown logic if there is time left AND if the counter is allowed to be active
            {
                countdownValue = Mathf.Clamp(countdownValue - Time.deltaTime, 0f, gm.workdayLength);//start the countdown


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

                /////////////////////////////////////////////This is all rough code that needs to be made nicer and more efficient
                //if (second % 60 == 1 || second % 60 == 31 && countdownValue > 32)
                //{
                    //if (!fadeAnimationStarted)
                    //{
                        //StartCoroutine(ClockFade());
                    //}
                //}

                //fade the clock in and don't let it fade out during the final push
               // if (Mathf.Ceil(countdownValue) == 31)
                //{
                    //fadeAnimation.Play("ClockFadeIn");
                //}

                //play the final fade out animation
                //if (countdownValue <= 0)
                //{
                    //if (!oneTimeFadeStarted)
                    //{
                        //fadeAnimation.Play("ClockFadeOut");
                        //oneTimeFadeStarted = true;
                    //}
                //}

                //start the red flashing warning that time is almost up
                if (countdownValue <= finalColorSecValue + 1)
                {
                    //fill.color = new Color(finalColor.r, finalColor.g, finalColor.b, Mathf.PingPong(Time.time * 1.2f, 1));
                    if (!oneTimeFadeStarted)
                    {
                        clockText.color = new Color(finalColor.r, finalColor.g, finalColor.b, Mathf.PingPong(Time.time * 1.2f, 1));
                    }

                }
                else if (countdownValue / gm.workdayLength <= warningColorPercent + 0.1f)
                {
                    //set the starting time for the fade to use
                    startTime = gm.workdayLength * warningColorPercent;
                    //fade from one color to the other smoothly
                    //fill.color = Color.Lerp(startFillColor, warningColor, Mathf.SmoothStep(0, 1, ((Time.time - startTime) / fadeTime)));
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

            //clockSlider.value = countdownValue;//adjust the slider fill value
            clockText.text = minText + ":" + secText;//format the timer text

        }
    }
    /// <summary>
    /// Handles playing the animation for the clock fading in and out
    /// </summary>
    /// <returns></returns>
    IEnumerator ClockFade()
    {
        fadeAnimationStarted = true;
        fadeAnimation.Play();

        yield return new WaitForSeconds(2);

        //fadeAnimation.clip = fadeAnimation.GetClip("ClockFadeOut");//change the clip to fade out
        //fadeAnimation.Play();//play the fade out

       // fadeAnimation.clip = fadeAnimation.GetClip("ClockFadeIn");//reset the animation clip to play correctly the next time coroutine is run
        //fadeAnimationStarted = false;//toggle bool back to false
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit");
    }
}
