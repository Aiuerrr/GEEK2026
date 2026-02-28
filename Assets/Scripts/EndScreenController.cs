using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndScreenController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject screenPanel;
    public RectTransform envelopeRect;
    public GameObject resultCardObj;

    [Header("Card Visuals")]
    public Image resultCardImage;
    public Sprite goodEndingSprite;
    public Sprite badEndingSprite;

    [Header("Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI triviaText;
    public string[] triviaFacts;

    [TextArea] public string winMessage;
    [TextArea] public string loseMessage;

    [Header("Buttons")]
    public GameObject endlessButton;
    public GameObject menuButton;

    [Header("Animation Settings")]
    public float startDelay = 2.0f;
    public float openDelay = 1.0f;
    public float readingTime = 5.0f;
    public float closeDelay = 2.0f;
    public float slideSpeed = 5000f;
    public float offScreenY = -1500f;

    public void ShowEndScreen(bool isWin, int round, int gold)
    {
        screenPanel.SetActive(true);
        Time.timeScale = 0f;

        envelopeRect.anchoredPosition = new Vector2(0, offScreenY);
        envelopeRect.gameObject.SetActive(true);
        resultCardObj.SetActive(false);

        statsText.text = $"Runda: {round}\nZłoto: {gold}";

        if (triviaText != null && triviaFacts != null && triviaFacts.Length > 0)
        {
            triviaText.text = triviaFacts[Random.Range(0, triviaFacts.Length)];
        }

        if (isWin)
        {
            resultCardImage.sprite = goodEndingSprite;
            titleText.text = "Drogi graczu, gratulacje!\nUdało ci się utrzymać drukarnię!";
            titleText.color = Color.black;
            if (messageText != null) messageText.text = winMessage;
            endlessButton.SetActive(true);
        }
        else
        {
            resultCardImage.sprite = badEndingSprite;
            titleText.text = "Drogi graczu, niestety...\nNie udało ci się utrzymać drukarni.";
            titleText.color = Color.black;
            if (messageText != null) messageText.text = loseMessage;
            endlessButton.SetActive(false);
        }

        StartCoroutine(FullSequenceRoutine());
    }

    IEnumerator FullSequenceRoutine()
    {
        yield return new WaitForSecondsRealtime(startDelay);
        yield return StartCoroutine(SlideEnvelope(0));
        yield return new WaitForSecondsRealtime(openDelay);

        envelopeRect.gameObject.SetActive(false);
        resultCardObj.SetActive(true);

        yield return new WaitForSecondsRealtime(readingTime);

        resultCardObj.SetActive(false);
        envelopeRect.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(closeDelay);
        yield return StartCoroutine(SlideEnvelope(offScreenY));

        envelopeRect.gameObject.SetActive(false);
    }

    IEnumerator SlideEnvelope(float targetY)
    {
        float currentY = envelopeRect.anchoredPosition.y;
        while (Mathf.Abs(currentY - targetY) > 10f)
        {
            currentY = Mathf.MoveTowards(currentY, targetY, slideSpeed * Time.unscaledDeltaTime);
            envelopeRect.anchoredPosition = new Vector2(0, currentY);
            yield return null;
        }
        envelopeRect.anchoredPosition = new Vector2(0, targetY);
    }

    public void OnEndlessModeClicked()
    {
        Time.timeScale = 1f;
        screenPanel.SetActive(false);
    }

    public void OnMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}