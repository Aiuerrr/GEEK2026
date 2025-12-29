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

    [Header("Effects")]
    public AudioSource audioSource;
    [Tooltip("Writing sounds")]
    public AudioClip[] typingSounds;
    [Tooltip("Error sound")]
    public AudioClip errorSound;
    [Tooltip("Particles and shake")]
    public ParticleSystem typeParticles;
    public Transform cameraTransform;
    public float shakeIntensity = 0.5f;
    public float shakeDuration = 0.2f;

    private float currentTime;
    private int currentIndex;
    private int mistakes;
    private bool isGameActive;
    private float startTime;
    private Vector3 originalCameraPos;

void Start()
    {
        if (cameraTransform == null && Camera.main != null) 
            cameraTransform = Camera.main.transform;
            
        originalCameraPos = cameraTransform.position;
        StartGame();
    }

    void OnEnable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput += OnTextInput;
    }

    void OnDisable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput -= OnTextInput;
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
                // --- POPRAWNE WPISANIE ---
                currentIndex++;
                if (feedbackText) feedbackText.text = "";
                
                // 1. Dźwięk z losowym pitch (żeby nie brzmiało jak robot)
                PlayTypingSound();

                // 2. Cząsteczki
                if(typeParticles) typeParticles.Play();

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
                // --- BŁĄD ---
                mistakes++;
                if (feedbackText) 
                {
                    feedbackText.text = "BŁĄD!";
                    feedbackText.color = Color.red; // Zmieniamy kolor na czerwony
                }

                // 1. Dźwięk błędu
                if (audioSource && errorSound) audioSource.PlayOneShot(errorSound);

                // 2. Trzęsienie kamerą
                StartCoroutine(ShakeCamera());
            }
        }
    }

    // NEW: Funkcja do odtwarzania przyjemnego dźwięku
    void PlayTypingSound()
    {
        if (audioSource && typingSounds.Length > 0)
        {
            // Losowy dźwięk z listy
            AudioClip clip = typingSounds[Random.Range(0, typingSounds.Length)];
            
            // Losowa wysokość dźwięku (0.95 do 1.05) - to daje efekt "naturalności"
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clip);
        }
    }

    // NEW: Funkcja Screen Shake
    IEnumerator ShakeCamera()
    {
        float elapsed = 0.0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            cameraTransform.position = originalCameraPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraTransform.position = originalCameraPos;
    }

    void UpdateTextDisplay()
    {
        if (textDisplay == null) return;

        string correctPart = textToType.Substring(0, currentIndex);
        string remainingPart = textToType.Substring(currentIndex);
        
        // NEW: Dodajemy kursor (znak pipe |) z mrugającym kolorem lub stałym
        // Dodatkowo: Podświetlony następny znak
        string cursor = "<color=#FFA500>|</color>"; // Pomarańczowy kursor

        // Formatowanie:
        // Poprawne: Białe/Jasne
        // Pozostałe: Półprzezroczyste
        string formattedText = $"<color=#FFFFFF>{correctPart}</color>{cursor}<color=#FFFFFF50>{remainingPart}</color>";
        
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
        if (duration <= 0) duration = 0.1f;

        float wpm = (textToType.Length / 5f) / (duration / 60f);

        string result = $"<size=150%>Koniec!</size>\n\nBłędy: <color=red>{mistakes}</color>\nPrędkość: <color=green>{wpm:F0} WPM</color>";
        if (statsText) statsText.text = result;
    }
}