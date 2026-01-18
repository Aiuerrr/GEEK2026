using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EasterEggController : MonoBehaviour
{
    [Header("Ustawienia")]
    public Image easterEggImage;
    public Sprite neutralImage;
    public Sprite pressedImage;
    public float duration = 3.0f;

    private Coroutine timerCoroutine;
    public void ActivateEasterEgg()
    {
        easterEggImage.sprite = pressedImage;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        timerCoroutine = StartCoroutine(CountAndReset());
    }

    IEnumerator CountAndReset()
    {
        yield return new WaitForSeconds(duration);

        easterEggImage.sprite = neutralImage;
        
        timerCoroutine = null;
    }
}