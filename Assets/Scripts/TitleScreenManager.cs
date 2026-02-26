using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("--- PANELE ---")]
    public GameObject optionsPanel; 

    [Header("--- ELEMENTY MENU ---")]
    public RectTransform[] opcjeMenu; 
    
    [Header("--- KURSOR ---")]
    public RectTransform kursor;
    
    [Header("--- KOREKTY POZYCJI (Dla każdego osobno) ---")]
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
        // --- ZABEZPIECZENIE ---
        // Jeśli panel opcji jest włączony, zatrzymaj ten skrypt.
        // Dzięki temu kursor nie będzie jeździł w tle, jak jesteś w opcjach.
        if (optionsPanel != null && optionsPanel.activeSelf) return;

        // --- NAWIGACJA ---
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ZmienWybor(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ZmienWybor(1);
        }

        // --- ZATWIERDZANIE ---
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
            case 0: // PLAY
                SceneManager.LoadScene("MainGameScene");
                break;
            case 1: // OPTIONS
                // --- NOWE: Otwieramy panel zamiast pisać w logach ---
                if (optionsPanel != null) optionsPanel.SetActive(true);
                break;
            case 2: // CREDITS
                Debug.Log("Autorzy...");
                break;
            case 3: // QUIT
                Application.Quit();
                break;
        }
    }
}