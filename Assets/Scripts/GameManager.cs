using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int packagesDelivered = 0;
    public int points = 0;

    public float timeInWorkday = 0f;
    public float workdayLength = 600f;

    public Text workdayStatusText;
    Animation textAnimation;

    bool paused = false;
    public Image pausePanel;
    public Image resumeButton;
    public Image optionsButton;
    public Image quitButton;
    Color panelColor;
    Color resumeButtonColor;
    Color optionsButtonColor;
    Color quitButtonColor;
    // Start is called before the first frame update
    void Start()
    {
        workdayStatusText.text = "Clocked IN!";
        textAnimation = workdayStatusText.GetComponent<Animation>();
        panelColor = pausePanel.color;
        resumeButtonColor = resumeButton.color;
        optionsButtonColor = optionsButton.color;
        quitButtonColor = quitButton.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Reload scene...");
        }

        //pause the game on Espcape press
        if(Input.GetAxis("Cancel") != 0 || Input.GetKeyDown(KeyCode.Escape))
        {
            //if (paused)
            //{
            //    print("resume");
            //    Resume();
            //}
            //else
            //    print("pause");
            //    Pause();

            //temporary until UI gets implemented successfully
            UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
            Application.Quit();
        }

        //Workday reset
        timeInWorkday += Time.deltaTime;
        if (timeInWorkday > workdayLength)
        {
            //have the workday over text appear and fade before loading the scene
            StartCoroutine(Clockout());

        }
    }

    public IEnumerator Clockout()
    {
        workdayStatusText.text = "Clocked OUT!";
        textAnimation.Play();

        yield return new WaitForSeconds(textAnimation["WorkdayStatusAnim"].length);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("New Workday! Reload scene...");
    }

    void Pause()
    {
        
        panelColor.a = 1f;
        resumeButtonColor.a = 1f;
        optionsButtonColor.a = 1f;
        quitButtonColor.a = 1f;
        Time.timeScale = 0f;
        paused = true;

    }

    void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        panelColor.a = 0f;
        resumeButtonColor.a = 0f;
        optionsButtonColor.a = 0f;
        quitButtonColor.a = 0f;
    }
}
