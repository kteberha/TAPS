using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DemandController : MonoBehaviour
{

    public Image fillImage;

    public float targetDemand;

    public Color GoodColor;
    public Color WarningColor;
    public Color DangerColor;

    // Trackers for min/max values
    protected float maxValue = 2f, minValue = 0f;

    // Create a property to handle the slider's value
    private float currentValue = 2.0f;
    public float CurrentValue;

    private float currentValueCatchup = 2.0f;
    public float CurrentValueCatchup

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
        CurrentValue += .25f;
        //CurrentValue.DOValue(.25, .3);
    }

    void AddtoTimerCatchup()
    {
        CurrentValueCatchup += 0.25f;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentValue = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentValue -= 0.0086f;
        CurrentValueCatchup -= 0.0086f;

        if (CurrentValue > 1.0)
        {
            fillImage.DOColor(GoodColor, 1);
        }

        if (CurrentValue < 1.0)
        {
            fillImage.DOColor(WarningColor, 1);
        }

        if (CurrentValue < 0.5)
        {
            fillImage.DOColor(DangerColor, 1);
        }

        if (Input.GetKeyDown("space"))
        {
            AddtoTimer();
            //yield return new WaitForSeconds(1);
            AddtoTimerCatchup();
        }
    }
}
