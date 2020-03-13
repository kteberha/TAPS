using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EyeController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button thisButton;

    public Image rightEyeFocus;
    public Image leftEyeFocus;
    public Image rightEyeCase;
    public Image leftEyeCase;

    public GameObject topEyeCaseBound;
    public GameObject midEyeCaseBound;
    public GameObject bottomEyeCaseBound;

    public float moveTime;
    public float scaleTime;
    public float scaleMax;
    public float scaleMin;
    

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        thisButton.transform.DOScaleX(scaleMax, scaleTime);
        thisButton.transform.DOScaleY(scaleMax, scaleTime);

        if (gameObject.name == "Start/Resume")
            {
            rightEyeFocus.transform.DOMoveY(thisButton.transform.position.y, moveTime);
            leftEyeFocus.transform.DOMoveY(thisButton.transform.position.y, moveTime);

            rightEyeCase.transform.DOMoveY(topEyeCaseBound.transform.position.y, moveTime);
            leftEyeCase.transform.DOMoveY(topEyeCaseBound.transform.position.y, moveTime);
            //Debug.Log(name);
        }
        if (gameObject.name == "Options")
        {
            rightEyeFocus.transform.DOMoveY(thisButton.transform.position.y, moveTime);
            leftEyeFocus.transform.DOMoveY(thisButton.transform.position.y, moveTime);

            rightEyeCase.transform.DOMoveY(midEyeCaseBound.transform.position.y, moveTime);
            leftEyeCase.transform.DOMoveY(midEyeCaseBound.transform.position.y, moveTime);
            //Debug.Log(name);
        }
        if (gameObject.name == "Credits")
        {
            rightEyeFocus.transform.DOMoveY(thisButton.transform.position.y, moveTime);
            leftEyeFocus.transform.DOMoveY(thisButton.transform.position.y, moveTime);

            rightEyeCase.transform.DOMoveY(midEyeCaseBound.transform.position.y, moveTime);
            leftEyeCase.transform.DOMoveY(midEyeCaseBound.transform.position.y, moveTime);
            //Debug.Log(name);
        }
        if (gameObject.name == "Quit")
        {
            rightEyeFocus.transform.DOMoveY(thisButton.transform.position.y, moveTime);
            leftEyeFocus.transform.DOMoveY(thisButton.transform.position.y, moveTime);

            rightEyeCase.transform.DOMoveY(bottomEyeCaseBound.transform.position.y, moveTime);
            leftEyeCase.transform.DOMoveY(bottomEyeCaseBound.transform.position.y, moveTime);
            //Debug.Log(name);
        }

        //Output to console the GameObject's name and the following message
        //Debug.Log("Cursor Entering " + name + " GameObject");
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        thisButton.transform.DOScaleX(scaleMin, scaleTime);
        thisButton.transform.DOScaleY(scaleMin, scaleTime);
        //Output the following message with the GameObject's name
        //Debug.Log("Cursor Exiting " + name + " GameObject");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
