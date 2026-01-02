using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Przeciągnij tu obiekt PausePanel z Canvasa")]
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    void Update()
    {
        // Jeśli wciśniesz ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                WznowGre();
            }
            else
            {
                ZatrzymajGre();
            }
        }
    }

    public void WznowGre()
    {
        pauseMenuUI.SetActive(false); // Ukryj panel
        Time.timeScale = 1f;          // 1 = Normalny upływ czasu
        isPaused = false;
    }

    public void ZatrzymajGre()
    {
        pauseMenuUI.SetActive(true); // Pokaż panel
        Time.timeScale = 0f;         // 0 = Czas stoi w miejscu (ZAMRAŻANIE)
        isPaused = true;
    }

    public void WyjdzDoMenu()
    {
        // Bardzo ważne! Zanim wyjdziemy, musimy odmrozić czas.
        // Inaczej w Menu nic by się nie ruszało, a po powrocie do gry nadal by stała.
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
}