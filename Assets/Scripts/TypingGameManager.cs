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
    public string textToType = "Przykładowy tekst awaryjny."; // Zostanie nadpisany przez ClientManager
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

    // Dodana referencja do Sędziego
    private GameResultSystem sędzia;

    void Start()
    {
        // 1. Szukamy Sędziego na scenie (GameResultSystem)
        sędzia = FindFirstObjectByType<GameResultSystem>();
        if (sędzia == null) 
            Debug.LogError("⚠️ BRAK GameResultSystem NA SCENIE! Dodaj go do obiektu _SYSTEM!");

        // 2. Pobieramy tekst od Klienta (z Menu)
        // Sprawdzamy, czy w ogóle przyszedł jakiś tekst (czy nie jest pusty)
        if (!string.IsNullOrEmpty(ClientManager.TekstDoPrzepisania))
        {
            textToType = ClientManager.TekstDoPrzepisania;
            Debug.Log("📥 Odebrano tekst od klienta: " + textToType);
        }
        else
        {
            Debug.LogWarning("⚠️ Brak tekstu od klienta. Używam tekstu awaryjnego.");
        }

        // 3. Konfiguracja Shake Effect
        if (uiElementToShake == null)
            uiElementToShake = GetComponent<RectTransform>();

        if (uiElementToShake != null)
            originalUiPos = uiElementToShake.anchoredPosition;

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
            EndGame(false); // Czas się skończył (false = porażka czasowa)
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

                isCursorVisible = true;
                blinkTimer = 0f;

                if (currentIndex >= textToType.Length)
                {
                    UpdateTextDisplay();
                    EndGame(true); // Sukces (true = zdążył)
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

    // ZMIENIONE: Funkcja kończąca grę teraz woła Sędziego
    void EndGame(bool czyZdazyl)
    {
        isGameActive = false;
        isCursorVisible = false;
        UpdateTextDisplay();
        UpdateTimerUI();

        if (sędzia != null)
        {
            Debug.Log("🏁 Koniec gry! Wołam sędziego...");
            // Przekazujemy dane do GameResultSystem
            sędzia.ObliczWynikIKonczGre(currentTime, mistakes, textToType.Length, czyZdazyl);
        }
        else
        {
            Debug.LogError("‼️ Koniec gry, ale nie ma Sędziego (GameResultSystem)! Nie mogę wrócić do menu.");
        }
    }
}