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
        SceneManager.LoadScene("MainGameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}