using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class NarrativeDialogue : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    private Story story;

    // Start is called before the first frame update
    void Start()
    {
        //script for calling the .ink files located in the Dialogue Script folder
        story = new Story(inkJSONAsset.text);
        Debug.Log(story.Continue());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
