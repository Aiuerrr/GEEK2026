using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

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

    [Header("Visual Effects")]
    // WAŻNE: Nie przypisuj tu Canvasa! Przypisz Text albo Panel wewnątrz Canvasa.
    public RectTransform uiElementToShake;
    public float shakeIntensity = 15f; // Zwiększyłem, żeby było lepiej widać
    public float shakeDuration = 0.2f;
    public float cursorBlinkSpeed = 0.5f;

    private float currentTime;
    private int currentIndex;
    private int mistakes;
    private bool isGameActive;
    private float startTime;

    private Vector2 originalUiPos;
    private bool isCursorVisible = true;
    private float blinkTimer = 0f;

    void Start()
    {
        // Auto-przypisanie, jeśli zapomniałeś, ale lepiej przypisz ręcznie w Inspectorze
        if (uiElementToShake == null)
        {
            // Próbuje znaleźć RectTransform na tym samym obiekcie
            uiElementToShake = GetComponent<RectTransform>();
        }

        if (uiElementToShake != null)
        {
            originalUiPos = uiElementToShake.anchoredPosition;
        }

        StartGame();
    }

    void OnEnable()
    {
        if (Keyboard.current != null) Keyboard.current.onTextInput += OnTextInput;
    }

    void OnDisable()
    {
        if (Keyboard.current != null) Keyboard.current.onTextInput -= OnTextInput;
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

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame();
        }

        UpdateTimerUI();

        // Miganie kursora
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= cursorBlinkSpeed)
        {
            blinkTimer = 0f;
            isCursorVisible = !isCursorVisible;
            UpdateTextDisplay();
        }
    }

    void OnTextInput(char c)
    {
        if (!isGameActive) return;
        if (char.IsControl(c)) return;

        if (currentIndex < textToType.Length)
        {
            char expectedChar = textToType[currentIndex];

            if (c == expectedChar)
            {
                // Poprawne
                currentIndex++;
                if (feedbackText) feedbackText.text = "";

                isCursorVisible = true; // Pokaż kursor przy pisaniu
                blinkTimer = 0f;

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
                // Błąd
                mistakes++;
                if (feedbackText) feedbackText.text = "BŁĄD!";

                // Shake
                StopAllCoroutines();
                StartCoroutine(ShakeUI());
            }
        }
    }

    IEnumerator ShakeUI()
    {
        if (uiElementToShake == null) yield break;

        float elapsed = 0.0f;

        // Upewniamy się, że startujemy z dobrej pozycji
        uiElementToShake.anchoredPosition = originalUiPos;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            uiElementToShake.anchoredPosition = originalUiPos + new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        uiElementToShake.anchoredPosition = originalUiPos;
    }

    void UpdateTextDisplay()
    {
        if (textDisplay == null) return;

        string correctPart = textToType.Substring(0, currentIndex);
        string remainingPart = textToType.Substring(currentIndex);

        // --- FIX SKAKANIA ---
        // Zamiast pustego stringa "", używamy <alpha=#00>|, co sprawia że znak jest, ale jest przezroczysty.
        // Dzięki temu szerokość tekstu się nie zmienia.
        string cursorTag = (isGameActive && isCursorVisible) ? "" : "<alpha=#00>";
        string cursor = $"<color=#000000>{cursorTag}|</color>";

        string formattedText = $"<color=#000000>{correctPart}</color>{cursor}<color=#808080>{remainingPart}</color>";
        textDisplay.text = formattedText;
    }

    void UpdateTimerUI()
    {
        if (timerText) timerText.text = $"Czas: {Mathf.CeilToInt(currentTime)}s";
    }

    void EndGame()
    {
        isGameActive = false;
        isCursorVisible = false;
        UpdateTextDisplay();
        UpdateTimerUI();

        float duration = Time.time - startTime;
        if (duration <= 0) duration = 0.1f;
        float wpm = (textToType.Length / 5f) / (duration / 60f);

        string result = $"Koniec!\nBłędy: {mistakes}\nPrędkość: {wpm:F0} WPM";
        if (statsText) statsText.text = result;
    }
}