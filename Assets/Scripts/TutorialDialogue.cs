using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogue : MonoBehaviour
{
    [SerializeField]NarrativeDialogue narrativeDialogue;
    [SerializeField]DialogueMenuManager dialogueMenuManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveAndEnabled == true && Input.GetKeyDown(KeyCode.Space))
        {
            narrativeDialogue.RefreshView();
        }
    }
}
