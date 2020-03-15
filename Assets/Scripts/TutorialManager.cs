using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] AsteroidHome home;
    [SerializeField] OrderManager orderManager;

    [SerializeField] NarrativeDialogue narrativeDialogue;
    public int i = 0;//keep track of how many times the dialogue has progressed.
    int[] keyToggles; // when the dialogue should get toggled

    [SerializeField] DialogueMenuManager dialogueMenuManager;

    public GameObject player;
    PlayerController playerController;

    public Animator controlIconAnimator;
    public CanvasGroup controlsCG;

    public Animation animation;
    [SerializeField] AnimationClip fadein;
    [SerializeField] AnimationClip fadeout;

    bool movementCue = false;
    bool throwCue = false;
    bool teleportCue = false;
    bool teleported = false;
    bool readyForMainScene = false;


    // Start is called before the first frame update
    void Start()
    {
        gameManager.state = GAMESTATE.PAUSED;
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space) && dialogueMenuManager.dialogueScreen.activeInHierarchy)
        {
            if (!narrativeDialogue.story.canContinue)
            {
                print("she's ready");
                readyForMainScene = true;
            }

            narrativeDialogue.RefreshView();
            i++;
        }

        #region
        if (i == 3 && Input.GetKeyDown(KeyCode.Space))
        {
            print(i);
            i++;
            ToggleDialogueOff();
            controlIconAnimator.SetBool("LeftClick", true);
            controlsCG.alpha = 1f;

        }
        else if (i == 4 && Input.GetMouseButtonDown(0))
        {
            print(i);
            i++;
            ToggleDialogueOn();
            PauseObjects();
        }
        else if (i == 8 && Input.GetKeyDown(KeyCode.Space))
        {
            print(i);
            i++;
            ToggleDialogueOff();
            ResumeObjects();
            controlIconAnimator.SetTrigger("LeftClick");
        }
        else if (i == 9 && Input.GetKeyDown(KeyCode.Space))
        {
            print(i);
            i++;
            ToggleDialogueOn();

            PauseObjects();
        }
        else if (i == 11 && Input.GetKeyDown(KeyCode.Space))
        {
            print(i + " go collect multiple packages");
            i++;
            ToggleDialogueOff();

            Time.timeScale = 1f;
        }
        else if (i == 13)
        {
            print(i);
            i++;
            ToggleDialogueOn();
            PauseObjects();
        }
        else if (i == 14 && Input.GetKeyDown(KeyCode.Space))
        {
            print(i);
            i++;
            ToggleDialogueOff();
            Time.timeScale = 1f;
        }
        else if (i == 14)
        {
            print(i);
            i++;
            ToggleDialogueOn();
            orderManager.OrderPackage();
            Time.timeScale = 0f;
        }
        else if (i == 18 && Input.GetKeyDown(KeyCode.Space))
        {
            print(i + " now go deliver a package");
            i++;
            ToggleDialogueOff();
            Time.timeScale = 1f;
        }
        else if (i == 23 && Input.GetKeyDown(KeyCode.Space))
        {
            print(i + "chuck away");
            i++;
            ToggleDialogueOff();
            controlIconAnimator.SetTrigger("RightClick");
            Time.timeScale = 1f;
        }
        else if(i == 27 && !teleportCue)
        {
            teleportCue = true;
            controlIconAnimator.SetTrigger("Teleport");
        }
        else if(i == 28 && Input.GetKeyDown(KeyCode.Space))
        {
            print(i);
            ToggleDialogueOff();
        }
        else if(i == 28 && (Input.GetKeyDown(KeyCode.T) || Input.GetMouseButtonDown(2)))
        {
            print(i + " should reappear");
            i++;
            ToggleDialogueOn();
            narrativeDialogue.RefreshView();
        }

        #endregion
        if (readyForMainScene && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(EndFade());
        }



        if (Input.GetKeyDown(KeyCode.I))
        {
            print(i);
        }
    }

    public void ToggleDialogueOn()
    {
        print("toggle on " + i);
        dialogueMenuManager.dialogueScreen.SetActive(true);
        dialogueMenuManager.enabled = false;

    }

    public void ToggleDialogueOff()
    {
        print("toggle off: " + i);
        dialogueMenuManager.dialogueScreen.SetActive(false);
        dialogueMenuManager.enabled = true;
        gameManager.state = GAMESTATE.CLOCKEDIN;
    }

    public void PauseObjects()
    {
        player.GetComponent<Rigidbody2D>().Sleep();
        foreach(GameObject p in playerController.heldPackages)
        {
            p.GetComponent<Rigidbody2D>().Sleep();
        }
    }

    public void ResumeObjects()
    {
        player.GetComponent<Rigidbody2D>().WakeUp();
        foreach(GameObject p in playerController.heldPackages)
        {
            p.GetComponent<Rigidbody2D>().WakeUp();
        }
    }

    IEnumerator EndFade()
    {
        animation.clip = fadein;
        animation.Play();
        yield return new WaitForSeconds(animation.clip.length);
        SceneManager.LoadScene("MainScene");
    }
}
