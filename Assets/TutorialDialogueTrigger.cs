using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogueTrigger : MonoBehaviour
{
    [SerializeField] TutorialManager tutorialManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Player" && tutorialManager.i == 19)
        {
            tutorialManager.ToggleDialogueOn();
            tutorialManager.i++;
            Time.timeScale = 0f;
        }
    }
}
