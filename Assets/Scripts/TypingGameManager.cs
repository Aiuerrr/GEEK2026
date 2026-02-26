using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class TypingGameManager : MonoBehaviour
{
    [Header("--- UI REFERENCES ---")]
    public TMP_Text textDisplay;
    public TMP_Text timerText;
    public TMP_Text feedbackText;

    [Header("--- STATYSTYKI UI (DO PODPIĘCIA) ---")]
    public TMP_Text wpmText;
    public TMP_Text moneyText;
    public TMP_Text paperText;
    public TMP_Text inkText;
    public TMP_Text clientNumberText;

    [Header("Game Settings")]
    [TextArea(3, 10)]
    public string textToType = "";
    public float initialTime = 60f;

    [Header("Visual Effects")]
    public RectTransform uiElementToShake;
    public float shakeIntensity = 15f;
    public float shakeDuration = 0.2f;
    public float cursorBlinkSpeed = 0.5f;

    private float currentTime;
    private int currentIndex;
    private int mistakes;
    private bool isGameActive;

    private int savedWPM = 0;

    private Vector2 originalUiPos;
    private bool isCursorVisible = true;
    private float blinkTimer = 0f;

    private GameLoopManager gameLoopManager;

    void Start()
    {
        if (uiElementToShake != null)
            originalUiPos = uiElementToShake.anchoredPosition;

        isGameActive = false;

        currentTime = initialTime;
        savedWPM = 0;
        UpdateTimerUI();

        gameLoopManager = FindFirstObjectByType<GameLoopManager>();
    }

    // --- NOWA FUNKCJA DO RESETOWANIA SAMEGO WYGLĄDU ZEGARA ---
    public void ResetTimerDisplay()
    {
        currentTime = initialTime;
        UpdateTimerUI();
    }

    public void StartGame(string nowyTekst)
    {
        if (string.IsNullOrEmpty(nowyTekst)) return;

        textToType = nowyTekst;
        currentTime = initialTime; // Dla pewności resetujemy też tutaj
        currentIndex = 0;
        mistakes = 0;
        savedWPM = 0;
        isGameActive = true;

        UpdateTextDisplay();
        UpdateTimerUI();
        if (feedbackText) feedbackText.text = "";

        UpdateLiveStatsUI();
    }

    void OnEnable()
    {
        if (Keyboard.current != null) Keyboard.current.onTextInput += OnTextInput;
    }

    void OnDisable()
    {
        if (Keyboard.current != null) Keyboard.current.onTextInput -= OnTextInput;
    }

    void Update()
    {
        UpdateLiveStatsUI();

        if (!isGameActive) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            StartCoroutine(EndGame(false));
        }

        UpdateTimerUI();

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
                currentIndex++;
                if (feedbackText) feedbackText.text = "";

                isCursorVisible = true;
                blinkTimer = 0f;

                if (currentIndex >= textToType.Length)
                {
                    UpdateTextDisplay();
                    UpdateLiveStatsUI();
                    StartCoroutine(EndGame(true));
                }
                else
                {
                    UpdateTextDisplay();
                }
            }
            else
            {
                mistakes++;
                if (feedbackText) feedbackText.text = "BŁĄD!";
                StopAllCoroutines();
                StartCoroutine(ShakeUI());
            }
        }
    }

    void UpdateLiveStatsUI()
    {
        if (isGameActive)
        {
            float timeElapsedInSeconds = initialTime - currentTime;
            if (timeElapsedInSeconds > 0)
            {
                float minutesElapsed = timeElapsedInSeconds / 60f;
                savedWPM = Mathf.RoundToInt((currentIndex / 5f) / minutesElapsed);
            }
        }

        if (wpmText != null) wpmText.text = $"{savedWPM}";
        if (moneyText != null) moneyText.text = $"{ClientManager.TotalGold}";
        if (paperText != null) paperText.text = $"{ClientManager.PaperCount}";
        if (inkText != null) inkText.text = $"{ClientManager.InkCount} ({ClientManager.CurrentInkUses}/2)";
        if (clientNumberText != null && gameLoopManager != null)
        {
            clientNumberText.text = $"{gameLoopManager.currentRound}";
        }
    }

    IEnumerator ShakeUI()
    {
        if (uiElementToShake == null) yield break;

        float elapsed = 0.0f;
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

        string cursorTag = (isGameActive && isCursorVisible) ? "" : "<alpha=#00>";
        string cursor = $"<color=#000000>{cursorTag}|</color>";

        string formattedText = $"<color=#000000>{correctPart}</color>{cursor}<color=#808080>{remainingPart}</color>";
        textDisplay.text = formattedText;
    }

    void UpdateTimerUI()
    {
        if (timerText) timerText.text = $"{Mathf.CeilToInt(currentTime)}";
    }

    IEnumerator EndGame(bool wasSuccessful)
    {
        isGameActive = false;
        isCursorVisible = false;
        UpdateTextDisplay();
        UpdateTimerUI();

        PaperController paper = FindFirstObjectByType<PaperController>();
        if (paper != null)
        {
            paper.SlideOut();
            yield return new WaitForSeconds(paper.duration);
        }

        int moneyToPay = 0;

        if (!wasSuccessful)
        {
            moneyToPay = 0;
        }
        else
        {
            if (mistakes == 0) moneyToPay = 100;
            else if (mistakes <= 3) moneyToPay = 50;
            else moneyToPay = 25;
        }

        ClientManager client = FindFirstObjectByType<ClientManager>();
        if (client != null)
        {
            client.JobFinished(moneyToPay);
        }
    }

    // --- NOWA FUNKCJA: Wpisuje tekst na kartkę ZANIM zacznie się gra ---
    public void PrepareTextToType(string nowyTekst)
    {
        if (string.IsNullOrEmpty(nowyTekst)) return;

        textToType = nowyTekst;
        currentIndex = 0;
        isCursorVisible = false; // Kursor nie miga, dopóki kartka jedzie

        UpdateTextDisplay();
    }
}