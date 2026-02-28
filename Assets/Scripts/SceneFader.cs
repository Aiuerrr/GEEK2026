using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [Header("Ustawienia")]
    public Image fadeImage;
    public float fadeSpeed = 1.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    public void FadeToScene(string sceneName)
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeOut(sceneName));
        }
    }

    private IEnumerator FadeIn()
    {
        fadeImage.raycastTarget = false;
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}