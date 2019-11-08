using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int packagesDelivered = 0;
    public int points = 0;

    public float timeInWorkday = 0f;
    public float workdayLength = 600f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Cancel") != 0 || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Reload scene...");
        }

        //Workday reset
        timeInWorkday += Time.deltaTime;
        if (timeInWorkday > workdayLength)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("New Workday! Reload scene...");
        }
    }
}
