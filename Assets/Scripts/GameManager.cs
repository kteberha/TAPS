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
    
    [HideInInspector]
    public bool paused = false;
    public GameObject pausePanel;
    CanvasGroup cg;
    
    // Start is called before the first frame update
    void Start()
    {
        cg = pausePanel.GetComponent<CanvasGroup>();
        workdayStatusText.text = "Clocked IN!";
        textAnimation = workdayStatusText.GetComponent<Animation>();
        
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
                Resume();
            else
                Pause();
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

    public void Pause()
    {
        cg.alpha = 1f;
        cg.interactable = true;
        Time.timeScale = 0f;
        paused = true;

    }

    /// <summary>
    /// resume the game when the button is pressed
    /// </summary>
    public void Resume()
    {
        cg.interactable = false;
        cg.alpha = 0f;
        paused = false;
        Time.timeScale = 1f;

    }
    
    /// <summary>
    /// option the options menu
    /// </summary>
    public void Options()
    {

    }

    /// <summary>
    /// Quit the game in editor and build
    /// </summary>
    public void QuitGame()
    {
        UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
        Application.Quit();
    }
}
