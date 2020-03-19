using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;

public class NarrativeDialogue : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    public TextAsset inkJSONAsset2;
    public Story story;

    public Canvas canvas;
    public TextMeshProUGUI textBox;
    public Button button;
    public Button choiceButton;

    public HorizontalLayoutGroup hlg;

    // Start is called before the first frame update
    void Start()
    {
        if (textBox == null)
        {
            textBox = GameObject.Find("TextTest").GetComponent<TextMeshProUGUI>();
            print(textBox);
        }

        //script for calling the .ink files located in the Dialogue Script folder
        story = new Story(inkJSONAsset.text);

        //start the story off with the first line of text
        RefreshView();
    }

    public Story NewStory()
    {
        story = new Story(inkJSONAsset2.text);
        print("new story");
        RefreshView();
        return story;
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    public void RefreshView()
    {

        // Read all the content until we can't continue any more
        if (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            CreateContentView(text);
        }
        else
        {
            GetComponent<DialogueMenuManager>().EndDialogue();
        }

        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];

                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate {OnClickChoiceButton(choice);});
            }

            button.interactable = true;
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            //Button choice = CreateChoiceView("MANAGER HAS HUNG UP");
            //choice.onClick.AddListener(delegate {
            //    Start();
            //});
        }
    }

    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
        GameObject[] go = GameObject.FindGameObjectsWithTag("Choices");
        foreach (GameObject o in go)
        {
            Destroy(o);
        }
        button.interactable = true;
    }

    // Creates a button showing the choice text
    void CreateContentView(string text)
    {
        textBox.text = text;
    }

    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate(choiceButton) as Button;
        choice.transform.SetParent(canvas.transform.Find("DialogueScreen").Find("Choices").transform, false);

        // Gets the text from the button prefab
        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
        choiceText.text = text ;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = true;

        return choice;
    }

    //bad, we don't want this. This destroys every child in the canvas
    // Destroys all the children of this gameobject (all the UI)
    void RemoveChildren()
    {
        int childCount = canvas.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(canvas.transform.GetChild(i).gameObject);
        }
    }
    /////////////////////////////////////////////////////////////////////////////
}
