using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    [Header("Ustawienia Cutscenki")]
    public float waitTime = 15f;
    public string nextSceneName = "MainGameScene";

    private bool isSkipping = false;

    void Update()
    {
        if (isSkipping) return;

        waitTime -= Time.deltaTime;

        if (waitTime <= 0f || Input.GetMouseButtonDown(0))
        {
            isSkipping = true;

            if (SceneFader.Instance != null)
            {
                SceneFader.Instance.FadeToScene(nextSceneName);
            }
        }
    }
}