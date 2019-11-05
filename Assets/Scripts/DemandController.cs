using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DemandController : MonoBehaviour
{

    public Image fillImage;

    public Color green;
    public Color yellow;
    public Color red;

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

        if (CurrentValue == 1.0)
        {
            fillImage.DOColor(yellow, 1);
        }

        if (Input.GetKeyDown("space"))
        {
            CurrentValue += 0.25f;
           // yield return new WaitForSeconds(1);
            CurrentValueCatchup += 0.25f;
        }
    }
}
