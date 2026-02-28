using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject optionsPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel != null) optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    public void PlayGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxButtonClicked);
        SceneManager.LoadScene("MainGameScene");
    }

    public void ExitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxButtonClicked);
        Application.Quit();
    }
}