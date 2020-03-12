using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DemandController : MonoBehaviour
{

    public Image fillImage;
    public Image arrow;

    public float targetDemand;

    public Color GoodColor;
    public Color WarningColor;
    public Color DangerColor;

    private Color currentColor;

    public bool instant = true;

    // Trackers for min/max values
    public float maxValue = 30.0f, minValue = 0f;

    // Create a property to handle the slider's value
    private float currentValue = 300.0f;
    public float CurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            // Ensure the passed value falls within min/max range
            currentValue = Mathf.Clamp(value, minValue, maxValue);

            // Calculate the current fill percentage and display it
            float fillPercentage = currentValue / maxValue;
            fillImage.fillAmount = fillPercentage;
        }

    }



    void AddtoTimer()
    {
        //CurrentValue += 2.0f;
        DOTween.To(()=> currentValue, x => CurrentValue = x, currentValue + 2.0f, 1.0f);
    }


    // Update is called once per frame
    void Update()
    {
        if(currentValue > 0)
            CurrentValue -= Time.deltaTime;

        Color nextColor = currentColor;//default: set the next color to the current color

        if (fillImage.fillAmount == 0)
        {
            fillImage.color = GoodColor;
            arrow.color = GoodColor;
        }
        else if (fillImage.fillAmount < 0.25f)
        {
            nextColor = DangerColor;
            //fillImage.DOColor(DangerColor, 1);
        }
        else if(fillImage.fillAmount < 0.5f)
        {
            nextColor = WarningColor;
            //fillImage.DOColor(WarningColor, 1);
        }
        else if(fillImage.fillAmount > 0.5f)//when fill amount is above 50%
        {
            nextColor = GoodColor;
            //fillImage.DOColor(GoodColor, 1);
        }
        else
        {
            print("something gone wrong");
        }


        if (nextColor != currentColor)
        {
            currentColor = nextColor;//assign the current color to the next assigned color
            fillImage.DOColor(nextColor, 1);//change the fill color
            arrow.DOColor(nextColor, 1);//change the arrow color
        }

        //if (Input.GetKeyDown("space"))
        //{
        //    if (instant == false)
        //    {
        //        AddtoTimer();
        //    }
        //    else
        //    {
        //        CurrentValue += 2.0f;
        //        DOTween.To(() => currentValue, x => CurrentValue = x, currentValue, 1.0f);
        //    }
        //}
    }
}
