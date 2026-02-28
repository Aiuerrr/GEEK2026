using UnityEngine;
using System.Collections;

public class PaperController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform paperRect;

    [Header("Animation Settings")]
    public Vector2 hiddenPosition;
    public Vector2 visiblePosition;
    public float duration = 0.5f;

    private TypingGameManager typingManager;

    void Awake()
    {
        typingManager = FindFirstObjectByType<TypingGameManager>();

        if (paperRect != null)
            paperRect.anchoredPosition = hiddenPosition;
    }

    public void SlideIn(string textToType)
    {
        if (typingManager != null) typingManager.PrepareTextToType(textToType);

        StopAllCoroutines();
        StartCoroutine(AnimateSlideIn(textToType));
    }

    private IEnumerator AnimateSlideIn(string text)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            paperRect.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, percent);
            yield return null;
        }

        paperRect.anchoredPosition = visiblePosition;

        if (typingManager != null) typingManager.StartGame(text);
    }

    public void SlideOut()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateSlideOut());
    }

    private IEnumerator AnimateSlideOut()
    {
        float elapsed = 0f;
        Vector2 currentPos = paperRect.anchoredPosition;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            paperRect.anchoredPosition = Vector2.Lerp(currentPos, hiddenPosition, percent);
            yield return null;
        }
        paperRect.anchoredPosition = hiddenPosition;
    }
}