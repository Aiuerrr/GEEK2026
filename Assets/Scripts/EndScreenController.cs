using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndScreenController : MonoBehaviour
{
    [Header("--- UI ELEMENTS ---")]
    public GameObject screenPanel;      // Cały EndScreenPanel
    public RectTransform envelopeRect;  // <--- ZMIANA: RectTransform, żebyśmy mogli przesuwać kopertę
    public GameObject resultCardObj;    // Obiekt Game_Result
    
    [Header("--- CARD VISUALS ---")]
    public Image resultCardImage;       
    public Sprite goodEndingSprite;     
    public Sprite badEndingSprite;      

    [Header("--- TEXTS ---")]
    public TextMeshProUGUI titleText;   
    public TextMeshProUGUI messageText; 
    public TextMeshProUGUI statsText;   

    [Header("--- BUTTONS (Na Biurku) ---")]
    public GameObject endlessButton;    
    public GameObject menuButton;       

    [Header("--- ANIMATION SETTINGS ---")]
    public float startDelay = 2.0f;     // Czas patrzenia na biurko przed wejściem listu
    public float openDelay = 1.0f;      // Czas patrzenia na zamkniętą kopertę
    public float readingTime = 5.0f;    // Czas czytania wyniku (zanim zacznie znikać)
    public float closeDelay = 2.0f;     // Czas zamykania (chowania koperty)
    
    public float slideSpeed = 5000f;    // Szybkość wysuwania koperty (piksele na sekundę)
    public float offScreenY = -1500f;   // Pozycja Y, gdzie koperta jest schowana (pod ekranem)

    // Teksty
    [TextArea] public string winMessage = "Drogi graczu, gratulacje! \nUdalo ci sie utrzymac drukarnie!";
    [TextArea] public string loseMessage = "Drogi graczu, niestety... \nTwoja drukarnia zbankrutowala.";

    public void ShowEndScreen(bool isWin, int round, int gold)
    {
        screenPanel.SetActive(true);
        Time.timeScale = 0f;

        // Reset animacji
        envelopeRect.anchoredPosition = new Vector2(0, offScreenY);
        envelopeRect.gameObject.SetActive(true);
        resultCardObj.SetActive(false);

        // Ustawiamy statystyki (to masz podpięte, więc zadziała)
        statsText.text = $"Runda: {round}\nZłoto: {gold}";

        if (isWin)
        {
            resultCardImage.sprite = goodEndingSprite;
            titleText.text = "AWANS!";
            titleText.color = new Color(0, 0.5f, 0);
            
            // --- ZABEZPIECZENIE PRZED BRAKIEM TEKSTU ---
            // Kod wejdzie do środka TYLKO jeśli messageText jest podpięte
            if (messageText != null) 
            {
                messageText.text = winMessage;
            }
            
            endlessButton.SetActive(true);
        }
        else
        {
            resultCardImage.sprite = badEndingSprite;
            titleText.text = "ZWOLNIONY";
            titleText.color = new Color(0.6f, 0, 0);
            
            // --- ZABEZPIECZENIE PRZED BRAKIEM TEKSTU ---
            if (messageText != null)
            {
                messageText.text = loseMessage;
            }
            
            endlessButton.SetActive(false);
        }

        StartCoroutine(FullSequenceRoutine());
    }

    IEnumerator FullSequenceRoutine()
    {
        // FAZA 1: Czekamy przy biurku (same tło + przyciski)
        yield return new WaitForSecondsRealtime(startDelay);

        // FAZA 2: Koperta wjeżdża z dołu!
        // Uruchamiamy animację wjazdu i czekamy aż dojedzie
        yield return StartCoroutine(SlideEnvelope(0)); // 0 to środek ekranu

        // FAZA 3: Chwila napięcia przy zamkniętej kopercie
        yield return new WaitForSecondsRealtime(openDelay);

        // FAZA 4: Otwarcie listu (Koperta znika, Karta się pojawia)
        envelopeRect.gameObject.SetActive(false);
        resultCardObj.SetActive(true);

        // FAZA 5: Czas na przeczytanie wyroku
        yield return new WaitForSecondsRealtime(readingTime);

        // --- ROBIMY SYTUACJĘ ODWROTNĄ ---

        // FAZA 6: Zdejmujemy Game Result (Karta znika, Koperta wraca)
        resultCardObj.SetActive(false);
        envelopeRect.gameObject.SetActive(true);

        // FAZA 7: Chwila oddechu (widzimy znów kopertę)
        yield return new WaitForSecondsRealtime(closeDelay); // User chciał 2 sekundy tutaj? Można dostosować.

        // FAZA 8: Koperta zjeżdża w dół
        yield return StartCoroutine(SlideEnvelope(offScreenY));

        // FAZA 9: Koniec (Zostaje samo tło i przyciski)
        envelopeRect.gameObject.SetActive(false);
        
        // Teraz gracz widzi przyciski na biurku i może kliknąć
    }

    // Prosta funkcja do przesuwania UI (zamiast Animatora)
    IEnumerator SlideEnvelope(float targetY)
    {
        float currentY = envelopeRect.anchoredPosition.y;
        
        while (Mathf.Abs(currentY - targetY) > 10f)
        {
            // Przesuwamy w stronę celu niezależnie od Time.timeScale (bo gra jest zapauzowana)
            currentY = Mathf.MoveTowards(currentY, targetY, slideSpeed * Time.unscaledDeltaTime);
            envelopeRect.anchoredPosition = new Vector2(0, currentY);
            yield return null;
        }
        envelopeRect.anchoredPosition = new Vector2(0, targetY);
    }

    // --- BUTTONS ---
    public void OnEndlessModeClicked()
    {
        Time.timeScale = 1f;
        screenPanel.SetActive(false);
    }

    public void OnMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene"); 
    }
}