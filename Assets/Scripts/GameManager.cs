using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.GetAxis("Cancel"));
        if (Input.GetAxis("Cancel") != 0 || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Reload scene...");
        }
    }
}
