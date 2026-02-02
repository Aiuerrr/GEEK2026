using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class TypingGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text textDisplay;
    public TMP_Text timerText;
    public TMP_Text feedbackText;
    public TMP_Text statsText;

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
    
    private Vector2 originalUiPos;
    private bool isCursorVisible = true;
    private float blinkTimer = 0f;

    void Start()
    {
        if (uiElementToShake != null)
            originalUiPos = uiElementToShake.anchoredPosition;

        isGameActive = false;
    }
    
    public void StartGame(string nowyTekst)
    {
        if (string.IsNullOrEmpty(nowyTekst)) return;

        textToType = nowyTekst;
        currentTime = initialTime;
        currentIndex = 0;
        mistakes = 0;
        isGameActive = true;

        UpdateTextDisplay();
        if (feedbackText) feedbackText.text = "";
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
        if (!isGameActive) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            StartCoroutine(EndGame(false)); // Czas się skończył
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
                    StartCoroutine(EndGame(true)); // Wygrana!
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
        if (timerText) timerText.text = $"Czas: {Mathf.CeilToInt(currentTime)}s";
    }

    IEnumerator EndGame(bool wasSuccessful)
    {
        isGameActive = false;
        isCursorVisible = false;
        UpdateTextDisplay();
        UpdateTimerUI();

        // 1. Chowanie kartki
        PaperController paper = FindFirstObjectByType<PaperController>();
        if (paper != null)
        {
            paper.SlideOut();
            yield return new WaitForSeconds(paper.duration);
        }

        // 2. Obliczanie wyniku (LOKALNIE, żeby nie psuć sceny)
        int moneyToPay = 0;

        if (!wasSuccessful)
        {
            moneyToPay = 0; // Przegrana (czas)
        }
        else
        {
            // Prosta matematyka: 
            // 0 błędów = 100 zł
            // 1-3 błędy = 50 zł
            // Więcej = 25 zł
            if (mistakes == 0) moneyToPay = 100;
            else if (mistakes <= 3) moneyToPay = 50;
            else moneyToPay = 25;
        }

        // 3. Przekazanie wyniku do ClientManagera
        ClientManager client = FindFirstObjectByType<ClientManager>();
        if (client != null)
        {
            client.JobFinished(moneyToPay);
        }
        else
        {
            Debug.LogError("CRITICAL: Nie znaleziono ClientManagera!");
        }
    }
}