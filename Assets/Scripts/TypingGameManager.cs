using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TypingGameManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text element to display the target text.")]
    public TMP_Text textDisplay;
    [Tooltip("Text element to display the remaining time.")]
    public TMP_Text timerText;
    [Tooltip("Text element to display feedback (e.g., Error!).")]
    public TMP_Text feedbackText;
    [Tooltip("Text element to display final stats.")]
    public TMP_Text statsText;

    [Header("Game Settings")]
    [TextArea(3, 10)]
    public string textToType = "Przykładowy tekst do przepisania. Wpisz go poprawnie, aby wygrać grę.";
    public float initialTime = 60f;

    private float currentTime;
    private int currentIndex;
    private int mistakes;
    private bool isGameActive;
    private float startTime;

    void Start()
    {
        StartGame();
    }

    void OnEnable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput += OnTextInput;
        }
    }

    void OnDisable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput -= OnTextInput;
        }
    }

    void StartGame()
    {
        currentTime = initialTime;
        currentIndex = 0;
        mistakes = 0;
        isGameActive = true;
        startTime = Time.time;

        UpdateTextDisplay();
        if (feedbackText) feedbackText.text = "";
        if (statsText) statsText.text = "";
    }

    void Update()
    {
        if (!isGameActive) return;

        // Timer Logic
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame();
        }

        UpdateTimerUI();
    }

    void OnTextInput(char c)
    {
        if (!isGameActive) return;

        // Filter control characters (like Backspace, Enter, Esc)
        // If the game requires handling Enter as part of the text, remove '\r' check.
        if (char.IsControl(c)) return;

        if (currentIndex < textToType.Length)
        {
            char expectedChar = textToType[currentIndex];

            if (c == expectedChar)
            {
                // Correct input
                currentIndex++;
                if (feedbackText) feedbackText.text = "";

                // Check if finished
                if (currentIndex >= textToType.Length)
                {
                    UpdateTextDisplay();
                    EndGame();
                }
                else
                {
                    UpdateTextDisplay();
                }
            }
            else
            {
                // Wrong input
                mistakes++;
                if (feedbackText) feedbackText.text = "BŁĄD!";
                // Play sound effect here in the future
            }
        }
    }

    void UpdateTextDisplay()
    {
        if (textDisplay == null) return;

        string correctPart = textToType.Substring(0, currentIndex);
        string remainingPart = textToType.Substring(currentIndex);

        // Rich Text Formatting
        // Black (#000000) for typed/correct letters
        // Gray (#808080) for remaining letters (simulates transparency/fading)
        string formattedText = $"<color=#000000>{correctPart}</color><color=#808080>{remainingPart}</color>";
        textDisplay.text = formattedText;
    }

    void UpdateTimerUI()
    {
        if (timerText)
            timerText.text = $"Czas: {Mathf.CeilToInt(currentTime)}s";
    }

    void EndGame()
    {
        isGameActive = false;
        UpdateTimerUI();

        float duration = Time.time - startTime;
        if (duration <= 0) duration = 0.1f; // Safety check

        // WPM Calculation: (Characters / 5) / (Minutes)
        float wpm = (textToType.Length / 5f) / (duration / 60f);

        string result = $"Koniec!\nBłędy: {mistakes}\nPrędkość: {wpm:F0} WPM";
        if (statsText) statsText.text = result;
    }
}
