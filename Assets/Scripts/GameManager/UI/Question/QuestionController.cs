using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;

    public Question(string questionTextParam, string[] options, int correct)
    {
        questionText = questionTextParam;
        answers = options;
        correctAnswerIndex = correct;
    }
}

public class QuestionController : MonoBehaviour
{
    [Header("Question Database")]
    public List<Question> questionDatabase = new List<Question>();
    private List<Question> selectedQuestions = new List<Question>();

    // Events
    public static event Action OnAllQuestionsAnswered;
    public static event Action OnPlayerDied;

    [Header("UI References")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;  // Should be 4 buttons for 4 answers

    [Header("Door Detection Zones")]
    public DoorHingeDetechZone whiteDoorDetectionZone;
    public BlackDoorHingeDetechZone blackDoorDetectionZone;

    [Header("Game Manager")]
    public GameManagerScript gameManager;

    private int currentQuestionIndex = 0;

    void Start()
    {
        questionPanel.SetActive(false);
        InitializeQuestionDatabase();
        SelectRandomQuestions();
        SetupButtonListeners();
    }

    void InitializeQuestionDatabase()
    {
        questionDatabase.Clear();

        // 10 General Knowledge Questions with 3 choices each
        questionDatabase.Add(new Question(
            "What is the capital of France?",
            new string[] { "London", "Berlin", "Paris" },
            2
        ));

        questionDatabase.Add(new Question(
            "Which planet is known as the Red Planet?",
            new string[] { "Venus", "Mars", "Jupiter" },
            1
        ));

        questionDatabase.Add(new Question(
            "What is the largest ocean on Earth?",
            new string[] { "Atlantic", "Indian", "Pacific" },
            2
        ));

        questionDatabase.Add(new Question(
            "Who painted the Mona Lisa?",
            new string[] { "Vincent van Gogh", "Leonardo da Vinci", "Pablo Picasso" },
            1
        ));

        questionDatabase.Add(new Question(
            "What is the chemical symbol for gold?",
            new string[] { "Go", "Gd", "Au" },
            2
        ));

        questionDatabase.Add(new Question(
            "Which country invented pizza?",
            new string[] { "France", "Greece", "Italy" },
            2
        ));

        questionDatabase.Add(new Question(
            "What is the tallest mountain in the world?",
            new string[] { "K2", "Mount Everest", "Kangchenjunga" },
            1
        ));

        questionDatabase.Add(new Question(
            "How many continents are there?",
            new string[] { "5", "6", "7" },
            2
        ));

        questionDatabase.Add(new Question(
            "What is the smallest country in the world?",
            new string[] { "Monaco", "Vatican City", "Liechtenstein" },
            1
        ));

        questionDatabase.Add(new Question(
            "Which gas makes up most of Earth's atmosphere?",
            new string[] { "Oxygen", "Carbon Dioxide", "Nitrogen" },
            2
        ));

        Debug.Log($"Loaded {questionDatabase.Count} questions into database");
    }

    void SelectRandomQuestions()
    {
        selectedQuestions.Clear();
        List<Question> tempDatabase = new List<Question>(questionDatabase);

        for (int i = 0; i < 3 && tempDatabase.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempDatabase.Count);
            selectedQuestions.Add(tempDatabase[randomIndex]);
            tempDatabase.RemoveAt(randomIndex);
        }

        Debug.Log($"Selected {selectedQuestions.Count} random questions for this game");
    }

    void SetupButtonListeners()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Capture for closure
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => AnswerSelected(index));
        }
    }

    void Update()
    {
        // Debug key to show question
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShowQuestion();
        }

        // Keyboard shortcuts for answers
        if (Input.GetKeyDown(KeyCode.Alpha1)) AnswerSelected(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AnswerSelected(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) AnswerSelected(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) AnswerSelected(3);
    }

    public void DialogueFinished()
    {
        ShowQuestion();
    }

    public void ShowQuestion()
    {
        if (currentQuestionIndex < selectedQuestions.Count)
        {
            questionPanel.SetActive(true);
            Question currentQuestion = selectedQuestions[currentQuestionIndex];

            // Display question text
            if (questionText != null)
            {
                questionText.text = currentQuestion.questionText;
            }

            // Display answers on buttons with numbers
            for (int i = 0; i < answerButtons.Length && i < currentQuestion.answers.Length; i++)
            {
                if (answerButtons[i] != null)
                {
                    // Format: "1. Answer text" or "Press 1: Answer text"
                    string numberedAnswer = $"{i + 1}. {currentQuestion.answers[i]}";

                    // Try TextMeshPro first, then regular Text
                    TextMeshProUGUI tmpText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = numberedAnswer;
                    }
                    else
                    {
                        Text regularText = answerButtons[i].GetComponentInChildren<Text>();
                        if (regularText != null)
                        {
                            regularText.text = numberedAnswer;
                        }
                    }

                    answerButtons[i].gameObject.SetActive(true);
                }
            }

            // Hide unused buttons if there are fewer than 4 answers
            for (int i = currentQuestion.answers.Length; i < answerButtons.Length; i++)
            {
                if (answerButtons[i] != null)
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }

            Debug.Log($"Showing question {currentQuestionIndex + 1}: {currentQuestion.questionText}");
        }
    }

    void AnswerSelected(int selectedIndex)
    {
        if (currentQuestionIndex >= selectedQuestions.Count)
        {
            Debug.LogWarning("No more questions available");
            return;
        }

        Question currentQuestion = selectedQuestions[currentQuestionIndex];

        if (selectedIndex < 0 || selectedIndex >= currentQuestion.answers.Length)
        {
            Debug.LogError("Invalid answer index selected");
            return;
        }

        if (selectedIndex == currentQuestion.correctAnswerIndex)
        {
            Debug.Log("Correct answer!");
            currentQuestionIndex++;

            if (currentQuestionIndex >= selectedQuestions.Count)
            {
                // All questions answered correctly
                UnlockDoor();
            }
            else
            {
                // Show next question
                gameManager.StopTimer();
                ShowQuestion();
                gameManager.StartTimer();
            }
        }
        else
        {
            Debug.Log($"Wrong answer! Correct was: {currentQuestion.answers[currentQuestion.correctAnswerIndex]}");
            PlayerDies();
        }
    }

    void UnlockDoor()
    {
        Debug.Log("Door unlocked! Player can escape!");
        OnAllQuestionsAnswered?.Invoke();

        if (whiteDoorDetectionZone != null)
            whiteDoorDetectionZone.UnlockDoor();

        if (blackDoorDetectionZone != null)
            blackDoorDetectionZone.UnlockDoor();

        gameManager.StopTimer();
        questionPanel.SetActive(false);
    }

    void PlayerDies()
    {
        Debug.Log("Game Over! Player is dead.");
        OnPlayerDied?.Invoke();

        if (whiteDoorDetectionZone != null)
            whiteDoorDetectionZone.LockDoor();

        if (blackDoorDetectionZone != null)
            blackDoorDetectionZone.LockDoor();

        gameManager.OnTimeOut();
        gameManager.killPlayer();
        questionPanel.SetActive(false);
        currentQuestionIndex = 0; // Reset for next game
        gameManager.RemoveTimer();
    }

    // Reset questions for new game
    public void ResetQuestions()
    {
        currentQuestionIndex = 0;
        SelectRandomQuestions();
    }
}
