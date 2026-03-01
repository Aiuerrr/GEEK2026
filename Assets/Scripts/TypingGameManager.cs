using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class TypingGameManager : MonoBehaviour
{
    public bool isGameActive;

    [Header("UI References")]
    public TMP_Text textDisplay;
    public TMP_Text timerText;

    [Header("Stats UI")]
    public TMP_Text wpmText;
    public TMP_Text moneyText;
    public TMP_Text paperText;
    public TMP_Text inkText;
    public TMP_Text clientNumberText;

    [Header("Game Settings")]
    [TextArea(3, 10)]
    public string textToType = "";
    public float initialTime = 60f;

    [Header("Mistake Settings")]
    public float mistakeCooldownDuration = 0.5f;

    [Header("Visual Effects")]
    public RectTransform uiElementToShake;
    public float shakeIntensity = 15f;
    public float shakeDuration = 0.2f;
    public float cursorBlinkSpeed = 0.5f;

    private float currentTime;
    private int currentIndex;
    private int mistakes;
    private int savedWPM = 0;
    private Vector2 originalUiPos;
    private bool isCursorVisible = true;
    private float blinkTimer = 0f;
    private float mistakeCooldownTimer = 0f;
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

    public void ResetTimerDisplay()
    {
        currentTime = initialTime;
        UpdateTimerUI();
    }

    public void StartGame(string newText)
    {
        if (string.IsNullOrEmpty(newText)) return;

        textToType = newText;
        currentTime = initialTime;
        currentIndex = 0;
        mistakes = 0;
        savedWPM = 0;
        mistakeCooldownTimer = 0f;
        isGameActive = true;

        UpdateTextDisplay();
        UpdateTimerUI();
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

        if (mistakeCooldownTimer > 0)
        {
            mistakeCooldownTimer -= Time.deltaTime;
        }

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
        if (Time.timeScale == 0f) return;
        if (!isGameActive) return;
        if (char.IsControl(c)) return;

        if (currentIndex < textToType.Length)
        {
            char expectedChar = textToType[currentIndex];

            if (c == expectedChar)
            {
                currentIndex++;

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
                if (mistakeCooldownTimer <= 0f)
                {
                    mistakes++;
                    mistakeCooldownTimer = mistakeCooldownDuration;

                    if (AudioManager.Instance != null && AudioManager.Instance.sfxMistake != null)
                    {
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxMistake);
                    }

                    StopAllCoroutines();
                    StartCoroutine(ShakeUI());
                }
            }
        }
    }

    void UpdateLiveStatsUI()
    {
        if (isGameActive)
        {
            float timeElapsed = initialTime - currentTime;
            if (timeElapsed > 0)
            {
                float minutesElapsed = timeElapsed / 60f;
                savedWPM = Mathf.RoundToInt((currentIndex / 5f) / minutesElapsed);
            }
        }

        if (wpmText != null) wpmText.text = $"{savedWPM}";
        if (moneyText != null) moneyText.text = $"{ClientManager.TotalGold}";
        if (paperText != null) paperText.text = $"{ClientManager.PaperCount}";
        if (inkText != null) inkText.text = $"{ClientManager.InkCount} ({ClientManager.CurrentInkUses}/2)";
        if (clientNumberText != null && gameLoopManager != null) clientNumberText.text = $"{gameLoopManager.currentRound}";
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

        textDisplay.text = $"<color=#000000>{correctPart}</color>{cursor}<color=#808080>{remainingPart}</color>";
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

        if (wasSuccessful)
        {
            if (mistakes <= 7) moneyToPay = 500;
            else if (mistakes <= 15) moneyToPay = 350;
            else moneyToPay = 150;
        }

        ClientManager client = FindFirstObjectByType<ClientManager>();
        if (client != null) client.JobFinished(moneyToPay);
        if (TutorialManager.Instance != null) TutorialManager.Instance.AdvanceTutorial(1);
    }

    public void PrepareTextToType(string newText)
    {
        if (string.IsNullOrEmpty(newText)) return;

        textToType = newText;
        currentIndex = 0;
        isCursorVisible = false;

        UpdateTextDisplay();
    }
}