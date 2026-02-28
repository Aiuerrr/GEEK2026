using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    public GameObject buttonsContainer;
    public GameObject cursorImage;

    public RectTransform[] opcjeMenu;
    public RectTransform kursor;
    public Vector2[] korekty;

    private int obecnyIndeks = 0;

    void Start()
    {
        if (korekty.Length != opcjeMenu.Length)
        {
            korekty = new Vector2[opcjeMenu.Length];
        }

        UpdateCursorPosition();
    }

    void Update()
    {
        if (optionsPanel != null && optionsPanel.activeSelf) return;

        if (creditsPanel != null && creditsPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseCredits();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ZmienWybor(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ZmienWybor(1);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            WykonajAkcje();
        }
    }

    void ZmienWybor(int kierunek)
    {
        obecnyIndeks += kierunek;

        if (obecnyIndeks < 0) obecnyIndeks = opcjeMenu.Length - 1;
        else if (obecnyIndeks >= opcjeMenu.Length) obecnyIndeks = 0;

        UpdateCursorPosition();
    }

    void UpdateCursorPosition()
    {
        if (opcjeMenu[obecnyIndeks] == null) return;

        Vector3 nowaPozycja = opcjeMenu[obecnyIndeks].position;
        
        float poprawkaX = korekty[obecnyIndeks].x;
        float poprawkaY = korekty[obecnyIndeks].y;

        kursor.position = new Vector3(nowaPozycja.x + poprawkaX, nowaPozycja.y + poprawkaY, nowaPozycja.z);
    }

    void WykonajAkcje()
    {
        switch (obecnyIndeks)
        {
            case 0:
                SceneManager.LoadScene("MainGameScene");
                break;
            case 1:
                if (optionsPanel != null) optionsPanel.SetActive(true);
                break;
            case 2:
                OpenCredits();
                break;
            case 3:
                Application.Quit();
                break;
        }
    }

    void OpenCredits()
    {
        if (buttonsContainer != null) buttonsContainer.SetActive(false);
        if (cursorImage != null) cursorImage.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (buttonsContainer != null) buttonsContainer.SetActive(true);
        if (cursorImage != null) cursorImage.SetActive(true);
    }
}