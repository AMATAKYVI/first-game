using UnityEngine;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource timerAudioSource;
    public AudioClip tickingClockSound;

    public TextMeshProUGUI timerText;
    private float timeLeft;
    private bool timerRunning = false;
    public GameObject panelTimer;
    public FPSController player;

    void Start()
    {
        // Find the AudioSource if not assigned in Inspector
        if (timerAudioSource == null)
        {
            GameObject audioObject = GameObject.Find("ClockTickingMusic"); // Replace with actual name
            if (audioObject != null)
            {
                timerAudioSource = audioObject.GetComponent<AudioSource>();
                Debug.Log("Found specific AudioSource: " + timerAudioSource.name);
            }
            else
            {
                Debug.LogError("AudioSource GameObject not found! Check the name.");
            }
        }

        if (timerText != null)
        {
            timerText.text = "0";
            timerText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Timer Text is not assigned in the GameManagerScript.");
        }

        if (panelTimer != null)
        {
            panelTimer.SetActive(false);
        }
        else
        {
            Debug.LogError("Panel Timer is not assigned in the GameManagerScript.");
        }
    }

    public void StartTimer(float duration = 30f)
    {
        if (timerRunning)
        {
            Debug.LogWarning("Timer is already running. Stopping the current timer before starting a new one.");
            StopTimer();
        }

        timeLeft = duration;
        timerRunning = true;
        timerText.gameObject.SetActive(true);
        panelTimer.SetActive(true);

        // Start ticking sound
        PlayTickingSound();
    }

    public void StopTimer()
    {
        timerRunning = false;
        timerText.gameObject.SetActive(false);
        panelTimer.SetActive(false);

        // Stop ticking sound
        StopTickingSound();
    }

    void Update()
    {
        if (timerRunning)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timeLeft).ToString();

            if (timeLeft <= 0)
            {
                timerRunning = false;
                timerText.text = "0";
                OnTimeOut();
            }
        }
    }

    public void RemoveTimer()
    {
        timerRunning = false;
        timerText.gameObject.SetActive(false);
        panelTimer.SetActive(false);
        StopTickingSound();
        Debug.Log("Timer removed.");
    }

    public void OnTimeOut()
    {
        Debug.Log("Time's up! Player loses.");
        StopTickingSound();
        killPlayer();
    }

    // Simple Audio Methods
    private void PlayTickingSound()
    {
        if (timerAudioSource != null)
        {
            // Check if AudioSource already has a clip assigned in hierarchy
            if (timerAudioSource.clip != null)
            {
                Debug.Log($"Using existing audio clip: {timerAudioSource.clip.name}");
                timerAudioSource.loop = true;
                timerAudioSource.volume = 0.5f;
                timerAudioSource.Play();
                Debug.Log("Ticking sound started");
            }
            else if (tickingClockSound != null)
            {
                // Fallback to assigned clip
                timerAudioSource.clip = tickingClockSound;
                timerAudioSource.loop = true;
                timerAudioSource.volume = 0.5f;
                timerAudioSource.Play();
                Debug.Log("Ticking sound started with assigned clip");
            }
            else
            {
                Debug.LogWarning("No audio clip found on AudioSource or in tickingClockSound field!");
            }
        }
        else
        {
            Debug.LogError("No AudioSource found! Please assign timerAudioSource.");
        }
    }

    private void StopTickingSound()
    {
        if (timerAudioSource != null && timerAudioSource.isPlaying)
        {
            timerAudioSource.Stop();
            Debug.Log("Ticking sound stopped");
        }
    }

    public void killPlayer()
    {
        if (player != null)
        {
            player.Die();
        }
        else
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                FPSController playerController = playerObj.GetComponent<FPSController>();
                if (playerController != null)
                {
                    playerController.Die();
                }
            }
        }
    }
}
