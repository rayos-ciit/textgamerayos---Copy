using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct DialogueData
{
    [TextArea] public string line;
    public Sprite image;
    public AudioClip soundCue;
    public DecisionOption[] decisionOptions; // New field for decision options
}

[System.Serializable]
public struct DecisionOption
{
    public string buttonText;
    public GameObject nextScene; // Reference to the next scene GameObject
}

public class Dialogue : MonoBehaviour
{
    public DialogueData[] dialogues;
    public float textSpeed;
    public TextMeshProUGUI textComponent;
    public Image imageComponent;
    public TextMeshProUGUI[] decisionButtons; 

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        SetDecisionButtonsActive(false); // Initially hide decision buttons
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == dialogues[index].line)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = dialogues[index].line;
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        SetDecisionButtonsActive(false); // Hide decision buttons at the start of each dialogue
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()

    {
        CheckTriggers();

        foreach (char c in dialogues[index].line.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        SetDecisionButtonsActive(true);

    }

    void CheckTriggers()
    {
        if (index < dialogues.Length)
        {
            // Check if there's an image to display for the current line
            if (dialogues[index].image != null)
            {
                ChangeImage(dialogues[index].image);
            }

            // Check if there's a sound cue to play for the current line
            if (dialogues[index].soundCue != null)
            {
                PlaySound(dialogues[index].soundCue);
            }

            // Populate decision buttons
            for (int i = 0; i < dialogues[index].decisionOptions.Length && i < decisionButtons.Length; i++)
            {
                decisionButtons[i].text = dialogues[index].decisionOptions[i].buttonText;
            }
        }
    }

    void ChangeImage(Sprite image)
    {
        if (imageComponent != null)
        {
            imageComponent.sprite = image;
        }
        else
        {
            Debug.LogError("Image component not found!");
        }
    }

    void PlaySound(AudioClip sound)
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.PlayOneShot(sound);
        }
        else
        {
            Debug.LogError("AudioSource component not found!");
        }
    }

    void NextLine()
    {
        if (index < dialogues.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            // Hide decision buttons at the end of the dialogue
            SetDecisionButtonsActive(false);
            gameObject.SetActive(false);
        }
    }

    void SetDecisionButtonsActive(bool active)
    {
        foreach (TextMeshProUGUI button in decisionButtons)
        {
            button.gameObject.SetActive(active);
        }
    }

    // Method to be called when a decision button is clicked
    public void OnDecisionButtonClicked(int buttonIndex)
    {
        if (index < dialogues.Length && buttonIndex < dialogues[index].decisionOptions.Length)
        {
            // Get the next scene GameObject from the decision option
            GameObject nextScene = dialogues[index].decisionOptions[buttonIndex].nextScene;

            // Disable the current dialogue GameObject
            gameObject.SetActive(false);

            // Enable the next scene GameObject to continue the story
            if (nextScene != null)
            {
                nextScene.SetActive(true);
            }
            else
            {
                Debug.LogError("Next scene not assigned to the decision option.");
            }
        }
    }
}
