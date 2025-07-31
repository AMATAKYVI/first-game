using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSController : MonoBehaviour
{
    [Header("Death Effects")]
    //assign your fire effect prefab here
    public GameObject fireEffect;

    [Header("UI Door Settings")]
    public GameObject pressFUI;
    public GameObject lockedUI;

    [Header("UI Elements")]
    //loading logic
    public GameObject loadingUI;  // Panel to show during loading
    public Slider loadingBar;       // Progress bar (optional)
    public TextMeshProUGUI loadingText;

    //restart and quit logic
    public Button restartButton;
    public Button quitButton;
    public string homeSceneName = "MenuScene"; // Name of the home scene to load when quitting
    public TextMeshProUGUI progressText; // For TextMeshPro support

    public TextMeshProUGUI tipText;

    [Header("Winner UI")]
    public GameObject winnerUI;  // Add this - Winner screen panel
    public Button winnerRestartButton;  // Restart button on winner screen
    public Button winnerQuitButton;     // Quit button on winner screen

    public float walkSpeed = 4f;
    public float lookSpeed = 0.8f;

    public float jumpHeight = 4f;  // Jump height in Unity units
    public Transform playerCamera;  // Assign the Camera here (child of player)

    float verticalRotation = 0f;

    private bool isDead = false;


    public Rigidbody rb;  // Assign Rigidbody via inspector or GetComponent
    public GameObject gameOverUI;  // Assign your Game Over UI panel here
    public GameObject gameOverPanel;  // Assign your Game Over panel here

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask = 1;
    public GameObject questionPanel;
    public GameObject timerText;
    private bool canJump = false;
    private bool hasJumped = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCamera.localRotation = Quaternion.identity;

        DialogueManager.OnDialogueComplete += OnDialogueFinished;

        // Hide all UI at start
        HideAllUI();

        // Setup ground check
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = Vector3.down * 1f;
            groundCheck = groundCheckObj.transform;
        }

        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitToHome);
        }

        if (winnerRestartButton != null)
        {
            winnerRestartButton.onClick.AddListener(RestartGame);
        }

        if (winnerQuitButton != null)
        {
            winnerQuitButton.onClick.AddListener(QuitToHome);
        }
    }

    public void OnDialogueFinished()
    {
        canJump = true;
    }

    void Update()
    {
        if (isDead) return;
    }
    void FixedUpdate()
    {
        if (isDead) return;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        Vector3 newPos = rb.position + move * walkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);


    }


    void LateUpdate()
    {

        if (isDead) return;

        // Mouse look (same as before)
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    public void Die()
    {
        isDead = true;

        Debug.Log("Player has died.");


        rb.linearVelocity = Vector3.zero;  // Stop player movement
        rb.isKinematic = true;  // Disable physics interactions

        EnableFireEffect();

        StartCoroutine(HandleDeath());
    }

    private void EnableFireEffect()
    {
        // Just enable your existing fire prefab
        if (fireEffect != null)
        {
            fireEffect.SetActive(true);
            Debug.Log("Fire effect enabled!");
        }
    }
    private System.Collections.IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.5f);

        // Show game over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            questionPanel.SetActive(false);
            // If using UnityEngine.UI.Text
            var textComponent = timerText.GetComponent<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                textComponent.text = "You are dead!";
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Fallback: reload scene
            RestartGame();
        }
    }

    // Add restart method for UI buttons
    public void RestartGame()
    {


        StartCoroutine(RestartWithLoading());

    }
    public void QuitToHome()
    {
        Debug.Log("Quitting to home menu...");
        StartCoroutine(QuitWithLoading());


    }


    void OnDestroy()
    {
        // Unsubscribe from events
        DialogueManager.OnDialogueComplete -= OnDialogueFinished;
    }


    private System.Collections.IEnumerator RestartWithLoading()
    {
        // Hide all UI first
        HideAllUI();

        // Then show loading UI
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
        }

        if (loadingText != null)
        {
            loadingText.text = "Restarting...";
        }

        if (tipText != null)
        {
            string[] restartTips = {
                "💡 Tip: Listen carefully to all dialogue before the questions begin!",
                "💡 Tip: Use number keys 1-4 to quickly answer questions!",
                "💡 Tip: Wrong answers are fatal - think before you click!",
                "💡 Tip: Pay attention to the timer - time management is crucial!",
                "💡 Tip: Each playthrough has different randomized questions!",
                "💡 Tip: Focus and read each question carefully before answering!"
            };

            tipText.text = restartTips[Random.Range(0, restartTips.Length)];
        }

        // Simulate loading progress
        for (int i = 0; i <= 100; i += 10)
        {
            if (loadingBar != null)
            {
                loadingBar.value = i / 100f;
            }

            if (progressText != null)
            {
                progressText.text = $"{i}%";
            }

            yield return new WaitForSeconds(0.1f);
        }

        // Load scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private System.Collections.IEnumerator QuitWithLoading()
    {
        // Hide all UI first
        HideAllUI();

        // Then show loading UI
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
        }

        if (loadingText != null)
        {
            loadingText.text = "Returning to Menu...";
        }

        if (tipText != null)
        {
            string[] quitTips = {
                "💡 Thanks for playing! Try again for different questions!",
                "💡 Remember: Each game has 3 random questions from 10 total!",
                "💡 Pro tip: Practice makes perfect in this escape room!",
                "💡 Coming back? The questions will be randomized again!",
                "💡 Challenge yourself: Can you answer all questions correctly?",
                "💡 Good luck on your next escape attempt!"
            };

            tipText.text = quitTips[Random.Range(0, quitTips.Length)];
        }

        // Simulate loading progress
        for (int i = 0; i <= 100; i += 20)
        {
            if (loadingBar != null)
            {
                loadingBar.value = i / 100f;
            }

            if (progressText != null)
            {
                progressText.text = $"{i}%";
            }

            yield return new WaitForSeconds(0.1f);
        }

        // Load menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    // Add this method for when player wins
    public void PlayerWins()
    {
        isDead = true; // Stop player movement
        Debug.Log("Player wins!");

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        StartCoroutine(HandleWin());
    }

    private System.Collections.IEnumerator HandleWin()
    {
        // Hide all UI first
        HideAllUI();

        // Then show loading UI
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
        }

        if (loadingText != null)
        {
            loadingText.text = "Congratulations!";
        }

        if (tipText != null)
        {
            string[] winTips = {
                "🎉 Excellent! You answered all questions correctly!",
                "🏆 You successfully escaped! Well done!",
                "⭐ Perfect score! You're a true escape artist!",
                "🎯 Amazing! You beat the challenge!",
                "🥇 Victory! You've mastered this escape room!"
            };

            tipText.text = winTips[Random.Range(0, winTips.Length)];
        }

        // Simulate "processing victory" loading
        for (int i = 0; i <= 100; i += 20)
        {
            if (loadingBar != null)
            {
                loadingBar.value = i / 100f;
            }

            if (progressText != null)
            {
                progressText.text = $"{i}%";
            }

            yield return new WaitForSeconds(0.1f);
        }

        // Hide loading and show winner screen
        if (loadingUI != null)
        {
            loadingUI.SetActive(false);
        }

        if (winnerUI != null)
        {
            winnerUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HideAllUI()
    {
        if (loadingUI != null)
            loadingUI.SetActive(false);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winnerUI != null)
            winnerUI.SetActive(false);
        // Or find by name patterns
        if (pressFUI != null) pressFUI.SetActive(false);

        if (lockedUI != null) lockedUI.SetActive(false);
    }
}
