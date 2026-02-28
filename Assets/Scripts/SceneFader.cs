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
<<<<<<< Updated upstream
        if (fadeImage != null)
        {
            StartCoroutine(FadeOut(sceneName));
        }
=======
        StartCoroutine(FadeOut(sceneName));
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

        fadeImage.gameObject.SetActive(false);
=======
>>>>>>> Stashed changes
    }

    private IEnumerator FadeOut(string sceneName)
    {
<<<<<<< Updated upstream
        fadeImage.gameObject.SetActive(true);
=======
>>>>>>> Stashed changes
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