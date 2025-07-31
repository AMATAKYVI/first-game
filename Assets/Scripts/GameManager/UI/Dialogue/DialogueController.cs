using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{

    // Event to notify when dialogue is complete
    public static event Action OnDialogueComplete;


    [Header("Dialogue Settings")]
    [Tooltip("Text component to display dialogue lines")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueUI;


    public QuestionController questionController; // Reference to QuestionController

    public GameManagerScript gameManager; // Reference to GameManager
    public string[] lines; // List of sentences
    public float textSpeed = 0.03f;
    public DialogueManager dialogue;

    private int index;
    private bool isTyping = false;

    public GameObject skipConfirmationPanel; // Panel to confirm skipping dialogue

    public Button skipButton; // Button to skip dialogue
    /// <summary>
    /// Start is called before the first frame update.
    /// It initializes the dialogue lines and starts the dialogue.
    /// </summary>
    void Start()
    {
        lines = new string[]
        {
        "Hello World",
        "You're trapped in this world. You have to find the exit to escape the world.",
        "This Game is simple, you have to answer questions to open the door.",
        "There will be 3 questions in total.",
        "You have to answer them correctly to open the door.",
        "If you answer incorrectly, the door will close and you will get killed.",
        "You have 30 seconds to answer each question.",
        "Good luck!"
        };
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipDialogue);
        }
        // Hide confirmation panel at start
        if (skipConfirmationPanel != null)
        {
            skipConfirmationPanel.SetActive(false);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipDialogue);
        }

        dialogueUI.SetActive(true);
        StartDialogue();
    }

    /// <summary>
    /// Update is called once per frame.
    /// It checks if the dialogue UI is active and if the space key is pressed.
    /// </summary>
    void Update()
    {
        if (dialogueUI.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Skip to full sentence
                StopAllCoroutines();
                dialogueText.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }

        // Add ESC key to skip dialogue
        if (dialogueUI.activeInHierarchy && Input.GetKeyDown(KeyCode.K))
        {
            SkipDialogue();
        }
    }
    // Add skip dialogue method
    public void SkipDialogue()
    {
        if (skipConfirmationPanel != null)
        {
            skipConfirmationPanel.SetActive(true);

            // Unlock cursor so player can click buttons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ConfirmSkip();
        }

    }
    public void ConfirmSkip()
    {
        Debug.Log("ConfirmSkip called!"); // Add this line

        if (skipConfirmationPanel != null)
        {
            skipConfirmationPanel.SetActive(false);

        }

        // Lock cursor back for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StopAllCoroutines();
        dialogueText.text = ""; // Clear the text
        OnDialogueEnd(); // End the dialogue
    }

    public void CancelSkip()
    {
        if (skipConfirmationPanel != null)
        {
            skipConfirmationPanel.SetActive(false);
        }

        // Lock cursor back for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    /// <summary>
    /// Starts the dialogue by initializing the index and clearing the text.
    /// It also starts the coroutine to type out the first line of dialogue.
    /// </summary>
    void StartDialogue()
    {
        index = 0;
        dialogueText.text = "";
        StartCoroutine(TypeLine());
    }

    /// <summary>
    /// Detects when the player enters the trigger collider
    /// and starts the dialogue.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogue.gameObject.SetActive(true);
            dialogue.StartDialogue();
        }
    }

    /// <summary>
    /// Coroutine to type out the current line of dialogue
    /// </summary>
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in lines[index].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    /// <summary>
    /// Proceed to the next line of dialogue
    /// If there are no more lines, end the dialogue
    /// </summary>
    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            OnDialogueEnd();
        }
    }

    /// <summary>
    /// Called when the dialogue ends
    /// Hides the dialogue UI and starts the game
    /// </summary>
    void OnDialogueEnd()
    {
        dialogueUI.SetActive(false);
        // You can call GameManager.StartGame() here
        Debug.Log("Dialogue ended. Starting game...");

        OnDialogueComplete?.Invoke();


        gameManager.StartTimer(); // Start the timer
        questionController.DialogueFinished(); // Show the first question

    }


}
