using UnityEngine;
using UnityEngine.UI;

public class BlackDoorHingeDetechZone : MonoBehaviour
{

    //open from inside the room collider
    public Collider openFromInsideCollider;
    //open from outside the room collider
    public Collider openFromOutsideCollider;

    public BlackDoorHingeController doorHinge;        // Assign this to DoorPrefab (has DoorHinge)
    public GameObject pressFUI;        // Assign your UI "Press F" text object
    public GameObject lockedUI;
    private bool isPlayerNear = false;
    private bool isDoorLocked = true; // Flag to check if the door is locked
    //auto close
    public float autoCloseDelay = 3f; // Time in seconds before the door closes automatically
    private float autoCloseTimer = 0f;

    private bool isDialogueComplete = false;


    [Header("Win Condition")]
    public FPSController playerController; // Assign in inspector

    void Start()
    {
        if (pressFUI != null)
            pressFUI.SetActive(false);
        if (lockedUI != null)
            lockedUI.SetActive(false);

        DialogueManager.OnDialogueComplete += OnDialogueFinished;

        // Subscribe to question completion events
        // QuestionController.OnAllQuestionsAnswered += OnAllQuestionsAnswered;
        // QuestionController.OnPlayerDied += OnPlayerDied;

    }
    void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        DialogueManager.OnDialogueComplete -= OnDialogueFinished;

        QuestionController.OnAllQuestionsAnswered -= OnAllQuestionsAnswered;
        QuestionController.OnPlayerDied -= OnPlayerDied;
    }

    //   Add these event handlers
    void OnAllQuestionsAnswered()
    {
        UnlockDoor();
    }

    void OnPlayerDied()
    {
        LockDoor();
    }
    void OnDialogueFinished()
    {
        isDialogueComplete = true;
        Debug.Log("Door detection zone notified: Dialogue complete!");

        // If player is already near when dialogue ends, show appropriate UI
        if (isPlayerNear)
        {
            ShowAppropriateUI();
        }
    }
    void Update()
    {
        if (isPlayerNear && isDialogueComplete && Input.GetKeyDown(KeyCode.F))
        {
            if (isDoorLocked)
            {
                // Show locked message
                Debug.Log("Door is locked! Answer the questions first.");
                return;
            }
            doorHinge.ToggleDoor();
            if (pressFUI != null)
                pressFUI.SetActive(false);
        }

        // Handle auto close
        if (!isDoorLocked && doorHinge.isOpen)
        {
            autoCloseTimer += Time.deltaTime;
            if (autoCloseTimer >= autoCloseDelay)
            {
                doorHinge.ToggleDoor();
                autoCloseTimer = 0f;
            }
        }
    }

    void ShowAppropriateUI()
    {
        if (isDoorLocked)
        {
            if (lockedUI != null)
                lockedUI.SetActive(true);
            if (pressFUI != null)
                pressFUI.SetActive(false);
        }
        else
        {
            if (pressFUI != null)
                pressFUI.SetActive(true);
            if (lockedUI != null)
                lockedUI.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Something entered: " + other.name);
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Player entered trigger zone.");
            isPlayerNear = true;
            if (isDialogueComplete)
            {
                ShowAppropriateUI();
            }
        }

        Debug.Log("OnTriggerEnter: " + other.name);

        if (other.CompareTag("Player") && !isDoorLocked)
        {
            Debug.Log("Player entered door zone and door is unlocked - PLAYER WINS!");

            // Find player controller if not assigned
            if (playerController == null)
            {
                playerController = other.GetComponent<FPSController>();
            }

            // Trigger win condition
            if (playerController != null)
            {
                playerController.PlayerWins();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            // Hide both UIs when player leaves
            if (pressFUI != null)
                pressFUI.SetActive(false);
            if (lockedUI != null)
                lockedUI.SetActive(false);
        }
    }


    public void UnlockDoor()
    {
        isDoorLocked = false; // Set the door to unlocked
        if (isPlayerNear && isDialogueComplete)
        {
            ShowAppropriateUI();
        }
        Debug.Log("Door unlocked!");
    }

    public void LockDoor()
    {
        isDoorLocked = true; // Set the door to locked
        if (isPlayerNear && isDialogueComplete)
        {
            ShowAppropriateUI();
        }
        Debug.Log("Door locked!");
    }

    System.Collections.IEnumerator HideLockedMessage()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        if (lockedUI != null)
        {
            lockedUI.SetActive(false); // Hide the locked UI after the delay
        }
    }
}
