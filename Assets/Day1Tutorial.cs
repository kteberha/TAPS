using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Day1Tutorial : MonoBehaviour
{
    bool _started = false;
    bool _packageCollected = false;
    bool _packageThrown = false;
    bool _teleported = false;

    [SerializeField] string[] introLines, shuffleLines, throwLines, teleportLines, lastLines;//all the lines for the dialogue
    string[] dialogueReader;//array that will do all the reading
    [SerializeField] Text _text;
    int _textIndex = 0;
    CanvasGroup _cg;

    [SerializeField] Animator iconAnimator;
    CanvasGroup iconCg;

    private void Start()
    {
        _cg = GetComponent<CanvasGroup>();
        iconCg = iconAnimator.gameObject.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        MenuController.StartDay += StartTutorial;
        PlayerController.ToggleShuffleTutorial += StartShuffleTutorial;
        ThrowTutorialTrigger.ToggleTeleportTutorial += StartThrowTutorial;
        AsteroidHome.ToggleTeleportTutorial += StartTeleportTutorial;
    }

    private void OnDisable()
    {
        GameManager.WorkdayStarted -= StartTutorial;
        PlayerController.ToggleShuffleTutorial -= StartShuffleTutorial;
        ThrowTutorialTrigger.ToggleTeleportTutorial -= StartThrowTutorial;
        AsteroidHome.ToggleTeleportTutorial -= StartTeleportTutorial;
    }

    private void Update()
    {
        if(Input.anyKeyDown && _cg.alpha == 1)
        {
            print("progress tutorial");
            ProgressText();
        }

        if (GameManager.state != GAMESTATE.CLOCKEDIN)
            iconCg.alpha = 0;
        else
            iconCg.alpha = 1;
    }

    /// <summary>
    /// Starts the level off with the first tutorial active
    /// </summary>
    void StartTutorial()
    {
        print("started tutorial");
        iconAnimator.gameObject.SetActive(true);
        iconAnimator.SetTrigger("LeftClick");
        SetReader(introLines);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Sets the tutorial for teaching how to shuffle items
    /// </summary>
    void StartShuffleTutorial()
    {
        if (!_packageCollected)
        {
            iconAnimator.SetTrigger("Scroll");
            SetReader(shuffleLines);
            print("shuffle lines" + shuffleLines.Length);
            _packageCollected = true;
        }
    }

    /// <summary>
    /// sets the tutorial for teaching how to throw
    /// </summary>
    void StartThrowTutorial()
    {
        if (!_packageThrown)
        {
            iconAnimator.SetTrigger("RightClick");
            SetReader(throwLines);
            _packageThrown = true;
        }
    }

    /// <summary>
    /// sets the tutorial for teaching how to teleport
    /// </summary>
    void StartTeleportTutorial()
    {
        if (!_teleported)
        {
            iconAnimator.SetTrigger("Teleport");
            SetReader(teleportLines);
            _teleported = true;
            StartCoroutine(EndingLines());
        }
    }

    IEnumerator EndingLines()
    {
        yield return new WaitForSeconds(5);
        iconAnimator.gameObject.SetActive(false);
        SetReader(lastLines);
    }

    void SetReader(string[] _newDialogue)
    {
        dialogueReader = _newDialogue;
        _text.text = dialogueReader[0];
        _textIndex = 0;
        SetManagerActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Advances text. Checks if text should disappear or not
    /// </summary>
    void ProgressText()
    {
        //check that the next line of text exists
        if (_textIndex + 1 < dialogueReader.Length)
        {
            _textIndex++;//increment
            _text.text = dialogueReader[_textIndex];//set the new text
        }
        else
        {
            print(_textIndex + "= text index");
            ResumeGame();
        }
    }

    void ResumeGame()
    {
        SetManagerActive(false);
        Time.timeScale = 1;
    }

    void SetManagerActive(bool _active)
    {
        if (_active)
            _cg.alpha = 1;
        else
            _cg.alpha = 0;
    }

}
