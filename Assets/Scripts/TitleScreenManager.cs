using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("--- ELEMENTY MENU ---")]
    public RectTransform[] opcjeMenu; 
    
    [Header("--- KURSOR ---")]
    public RectTransform kursor;
    
    [Header("--- KOREKTY POZYCJI (Dla każdego osobno) ---")]
    // To jest tablica, która pozwoli nam ustawić inne przesunięcie dla PLAY, a inne dla QUIT
    // X = lewo/prawo, Y = góra/dół
    public Vector2[] korekty; 

    private int obecnyIndeks = 0;

    void Start()
    {
        // Zabezpieczenie: jeśli zapomnisz ustawić korekty w Unity, skrypt sam je stworzy (same zera)
        if (korekty.Length != opcjeMenu.Length)
        {
            korekty = new Vector2[opcjeMenu.Length];
        }

        UpdateCursorPosition();
    }

    void Update()
    {
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

        // 1. Bierzemy pozycję wybranej opcji
        Vector3 nowaPozycja = opcjeMenu[obecnyIndeks].position;
        
        // 2. Dodajemy unikalną korektę dla tego konkretnego elementu
        // (np. dla PLAY dodamy -50, a dla QUIT -80)
        float poprawkaX = korekty[obecnyIndeks].x;
        float poprawkaY = korekty[obecnyIndeks].y;

        kursor.position = new Vector3(nowaPozycja.x + poprawkaX, nowaPozycja.y + poprawkaY, nowaPozycja.z);
    }

    void WykonajAkcje()
    {
        switch (obecnyIndeks)
        {
            case 0: // PLAY
                SceneManager.LoadScene("MainMenu");
                break;
            case 1: // OPTIONS
                Debug.Log("Opcje...");
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