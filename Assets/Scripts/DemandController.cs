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

    private Color currentColor;

    public bool instant = true;

    // Trackers for min/max values
    protected float maxValue = 30.0f, minValue = 0f;

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



    // Start is called before the first frame update
    void Start()
    {
        CurrentValue = 300.0f;
        //print(CurrentValue);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentValue > 0)
            CurrentValue -= Time.deltaTime;

        Color nextColor = currentColor;

        if (fillImage.fillAmount > 0.5f)
        {
            nextColor = GoodColor;
            //fillImage.DOColor(GoodColor, 1);
        }

        if (fillImage.fillAmount < 0.5f)
        {
            nextColor = WarningColor;
            //fillImage.DOColor(WarningColor, 1);
        }

        if (fillImage.fillAmount < 0.25f)
        {
            nextColor = DangerColor;
            //fillImage.DOColor(DangerColor, 1);
        }

        if (fillImage.fillAmount == 0)
            fillImage.color = GoodColor;


        if (nextColor != currentColor)
        {
            currentColor = nextColor;
            fillImage.DOColor(nextColor, 1);
        }

        if (Input.GetKeyDown("space"))
        {
            if (instant == false)
            {
                AddtoTimer();
            }
            else
            {
                CurrentValue += 2.0f;
                DOTween.To(() => currentValue, x => CurrentValue = x, currentValue, 1.0f);
            }
        }
    }
}
