using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    // --- NASZA BEZPIECZNA FLAGA ---
    public static bool isCountingDown = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsMenuUI;
    [SerializeField] private InputActionReference pauseAction;

    [Header("--- ODLICZANIE (3, 2, 1) ---")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private Image countdownImage;
    [SerializeField] private Sprite[] countdownSprites;

    private Coroutine countdownRoutine;

    private void Awake()
    {
        isCountingDown = false; // Zabezpieczenie przy restarcie sceny
    }

    private void OnEnable()
    {
        pauseAction.action.performed += OnPausePerformed;
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePerformed;
        pauseAction.action.Disable();
        isCountingDown = false;
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (isCountingDown)
        {
            StopGame();
            return;
        }

        if (pauseMenuUI.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            StopGame();
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        if (optionsMenuUI != null) optionsMenuUI.SetActive(false);

        if (countdownRoutine != null) StopCoroutine(countdownRoutine);
        countdownRoutine = StartCoroutine(CountdownCoroutine());
    }

    public void StopGame()
    {
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }

        isCountingDown = false; // PRZERWANIE ODLICZANIA
        if (countdownPanel != null) countdownPanel.SetActive(false);

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    private IEnumerator CountdownCoroutine()
    {
        isCountingDown = true; // ZACZYNAMY ODLICZANIE
        if (countdownPanel != null) countdownPanel.SetActive(true);

        Time.timeScale = 0f;

        float popDuration = 0.25f;
        float holdDuration = 0.75f;
        float targetScale = 0.5f;

        for (int i = 0; i < countdownSprites.Length; i++)
        {
            if (countdownImage != null && countdownSprites[i] != null)
            {
                countdownImage.sprite = countdownSprites[i];
                countdownImage.SetNativeSize();
            }

            float elapsed = 0f;
            while (elapsed < popDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / popDuration;

                float scale = Mathf.SmoothStep(0f, targetScale, t);
                countdownImage.transform.localScale = new Vector3(scale, scale, 1f);

                yield return null;
            }

            countdownImage.transform.localScale = new Vector3(targetScale, targetScale, 1f);

            yield return new WaitForSecondsRealtime(holdDuration);
        }

        if (countdownPanel != null) countdownPanel.SetActive(false);
        countdownRoutine = null;

        isCountingDown = false; // KONIEC ODLICZANIA
        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        if (optionsMenuUI != null) optionsMenuUI.SetActive(true);
    }

    public void ReturnToMenu()
    {
        isCountingDown = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}