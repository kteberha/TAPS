using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] AsteroidHome home;
    [SerializeField] OrderManager orderManager;
    [SerializeField] TutorialDialogue tutorialDialogue;

    [SerializeField] NarrativeDialogue narrativeDialogue;
    [SerializeField] DialogueMenuManager dialogueMenuManager;

    Package package;

    GameObject player;
    PlayerController playerController;

    bool startDialogueEnded = false;
    bool packageCollected = false;
    bool firstDeliveryMade = false;
    bool teleported = false;
    bool readyForMainScene = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!narrativeDialogue.story.canContinue)
        {
            print("she's ready");
            readyForMainScene = true;
        }

        if (readyForMainScene && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
