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

    // Start is called before the first frame update
    void Start()
    {
        workdayStatusText.text = "Clocked IN!";
        textAnimation = workdayStatusText.GetComponent<Animation>();
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
}
