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

    private float lerpPerc = 0f;
    private float timeSinceOverlap = 0f;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        thisButton.transform.DOScaleX(scaleMax, scaleTime);
        thisButton.transform.DOScaleY(scaleMax, scaleTime);

        switch(gameObject.name)
        {
            case "Start/Resume":
                timeSinceOverlap = 0f;
                StartCoroutine(EyeTransition(topEyeCaseBound));
                break;

            case "Options":
                timeSinceOverlap = 0f;
                StartCoroutine(EyeTransition(midEyeCaseBound));
                break;
            case "Credits":
                timeSinceOverlap = 0f;
                StartCoroutine(EyeTransition(midEyeCaseBound));
                break;
            case "Quit":
                timeSinceOverlap = 0f;
                StartCoroutine(EyeTransition(bottomEyeCaseBound));
                break;
            default:
                print("No names matched");
                break;

        }

        /*
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
        */
    }


    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        thisButton.transform.DOScaleX(scaleMin, scaleTime);
        thisButton.transform.DOScaleY(scaleMin, scaleTime);
        //Output the following message with the GameObject's name
        //Debug.Log("Cursor Exiting " + name + " GameObject");
    }

    /// <summary>
    /// Handles the eye movement from button to button when they are selected.
    /// </summary>
    /// <param name="eyeCase"></param>
    /// <returns></returns>
    IEnumerator EyeTransition(GameObject eyeCase)
    {
        var rEyeCaseStartTrans = rightEyeCase.transform;
        var lEyeCaseStartTrans = leftEyeCase.transform;
        var rEyeFocusStartTrans = rightEyeFocus.transform;
        var lEyeFocusStartTrans = leftEyeFocus.transform;

        //start the lerp for the eye case at its starting position and set its new position
        rEyeCaseStartTrans.position = new Vector3(rEyeCaseStartTrans.position.x, Mathf.SmoothStep(rEyeCaseStartTrans.position.y, eyeCase.transform.position.y, lerpPerc), rEyeCaseStartTrans.position.z);
        lEyeCaseStartTrans.position = new Vector3(lEyeCaseStartTrans.position.x, Mathf.SmoothStep(lEyeCaseStartTrans.position.y, eyeCase.transform.position.y, lerpPerc), lEyeCaseStartTrans.position.z);

        //start the lerp for the eye pupil at its starting position and set its new position
        rEyeFocusStartTrans.position = new Vector3(rEyeFocusStartTrans.position.x, Mathf.SmoothStep(rEyeFocusStartTrans.position.y, thisButton.transform.position.y, lerpPerc), rEyeFocusStartTrans.position.z);
        lEyeFocusStartTrans.position = new Vector3(lEyeFocusStartTrans.position.x, Mathf.SmoothStep(lEyeFocusStartTrans.position.y, thisButton.transform.position.y, lerpPerc), lEyeFocusStartTrans.position.z);

        yield return new WaitForEndOfFrame();

        if(lerpPerc <= 1)
        {
            //print("need to keep lerping: " + lerpPerc);
            lerpPerc += timeSinceOverlap / moveTime;
            yield return StartCoroutine(EyeTransition(eyeCase));
        }
        else
        {
            //print("lerp done, percent restarted");
            lerpPerc = 0f;
        }
    }

    private void Update()
    {
        timeSinceOverlap += Time.deltaTime;
    }
}
