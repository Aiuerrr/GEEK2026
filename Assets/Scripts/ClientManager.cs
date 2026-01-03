using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientManager : MonoBehaviour
{
    [Header("--- USTAWIENIA KLIENTA ---")]
    public GameObject klientCalosc;
    public Image obrazekGlowy;
    
    [Header("--- PIENIĄDZE (INTERAKCJA) ---")]
    public GameObject przyciskKasy; 
    public Image obrazekKasy;       
    
    [Header("--- GRAFIKI MONET ---")]
    public Sprite monetyMalo;   // 25
    public Sprite monetySrednio;// 50
    public Sprite monetyDuzo;   // 100

    [Header("--- GRAFIKI TWARZY ---")]
    public Sprite glowaSzczesliwa;
    public Sprite glowaSrednia;
    public Sprite glowaZla;
    
    [Header("--- CZAS ---")]
    public float czasDoPojawienia = 3.0f;

    // --- ZMIENNE GLOBALNE ---
    public static string TekstDoPrzepisania = ""; 
    public static int WygraneMonety = 0;
    public static int CalkowiteZloto = 0;
    public static bool WracamyZGry = false;

    [Header("--- ANIMACJA ---")]
    public float szybkoscWyskoku = 0.5f; // Użyjemy tej samej szybkości do chowania się
    private Vector3 pozycjaStartowa;
    private Vector3 pozycjaUkryta;

    IEnumerator Start()
    {
        // --- KONFIGURACJA POZYCJI ---
        if (klientCalosc != null)
        {
            pozycjaStartowa = klientCalosc.transform.localPosition;
            // Obliczamy pozycję pod ladą
            pozycjaUkryta = new Vector3(pozycjaStartowa.x, pozycjaStartowa.y - 800f, pozycjaStartowa.z);
        }

        // Ukrywamy kasę na start
        if (przyciskKasy != null) przyciskKasy.SetActive(false);

        if (WracamyZGry)
        {
            // --- SCENARIUSZ A: WRACAMY Z GRY (Klient czeka na wypłatę) ---
            if (klientCalosc != null)
            {
                // Upewniamy się, że stoi NA MIEJSCU
                klientCalosc.transform.localPosition = pozycjaStartowa;
                klientCalosc.SetActive(true);
            }
            
            UstawMinePoWyniku();

            if (WygraneMonety > 0)
            {
                PokazKaseNaLadzie();
                Debug.Log("💰 Klient położył kasę. Czekam aż podniesiesz...");
            }
            else
            {
                Debug.Log("😡 Klient nic nie płaci i wychodzi.");
                yield return new WaitForSeconds(2.0f); // Czeka chwilę zły
                KlientSobieIdzie(); // I chowa się pod ladę
            }
        }
        else
        {
            // --- SCENARIUSZ B: STARTUJEMY GRĘ OD ZERA ---
            if (klientCalosc != null)
            {
                // Chowamy go pod ladę fizycznie na start
                klientCalosc.transform.localPosition = pozycjaUkryta;
                klientCalosc.SetActive(false);
            }
            RozpocznijOdliczanieDoKlienta();
        }
    }

    void PokazKaseNaLadzie()
    {
        if (przyciskKasy == null) return;

        if (WygraneMonety >= 100)
        {
            obrazekKasy.sprite = monetyDuzo;
            przyciskKasy.transform.localScale = Vector3.one;
        }
        else if (WygraneMonety >= 50)
        {
            obrazekKasy.sprite = monetySrednio;
            przyciskKasy.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        }
        else
        {
            obrazekKasy.sprite = monetyMalo;
            przyciskKasy.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }
        przyciskKasy.SetActive(true); 
    }

    // FUNKCJA PODPIĘTA POD PRZYCISK KASY
    public void PodniesKase()
    {
        CalkowiteZloto += WygraneMonety;
        Debug.Log($"💵 Podniosłeś {WygraneMonety}. Razem masz: {CalkowiteZloto}");

        WygraneMonety = 0;
        WracamyZGry = false;

        przyciskKasy.SetActive(false);

        // Klient chowa się pod ladę
        KlientSobieIdzie();
    }

    // --- ZMIENIONA FUNKCJA ---
    void KlientSobieIdzie()
    {
        Debug.Log("👋 Klient schodzi pod ladę.");
        // Zamiast wyłączać, uruchamiamy animację wyjścia
        if (klientCalosc != null) StartCoroutine(AnimujWyjscie());
    }

    void RozpocznijOdliczanieDoKlienta()
    {
        StartCoroutine(CzekajNaNowego());
    }

    IEnumerator CzekajNaNowego()
    {
        Debug.Log($"⏳ Czekam {czasDoPojawienia}s na nowego...");
        yield return new WaitForSeconds(czasDoPojawienia);
        GenerujNowegoKlienta();
    }

    void UstawMinePoWyniku()
    {
        if (obrazekGlowy == null) return;
        if (WygraneMonety >= 100) obrazekGlowy.sprite = glowaSzczesliwa;
        else if (WygraneMonety >= 50) obrazekGlowy.sprite = glowaSrednia;
        else obrazekGlowy.sprite = glowaZla;
    }

    void GenerujNowegoKlienta()
    {
        // Uruchamiamy animację wejścia
        if (klientCalosc != null) StartCoroutine(AnimujWejscie());

        if (glowaSrednia != null) obrazekGlowy.sprite = glowaSrednia;
        TekstDoPrzepisania = GenerujBelkot(Random.Range(5, 10));
        Debug.Log("🔔 Nowy klient!");
    }
    
    string GenerujBelkot(int ilosc)
    {
        string belkot = "";
        string[] sylaby = { "glar", "bo", "zump", "fiko", "tra", "xi", "mu", "ka", "bazz", "qo" };
        for (int i = 0; i < ilosc; i++) belkot += sylaby[Random.Range(0, sylaby.Length)] + sylaby[Random.Range(0, sylaby.Length)] + " ";
        return belkot.Trim();
    }

    // --- ANIMACJA WEJŚCIA (Z dołu do góry) ---
    IEnumerator AnimujWejscie()
    {
        if (klientCalosc == null) yield break;

        float czas = 0f;
        klientCalosc.SetActive(true);

        while (czas < szybkoscWyskoku)
        {
            czas += Time.deltaTime;
            float procent = czas / szybkoscWyskoku;
            float t = Mathf.SmoothStep(0f, 1f, procent);
            
            klientCalosc.transform.localPosition = Vector3.Lerp(pozycjaUkryta, pozycjaStartowa, t);
            
            yield return null;
        }
        klientCalosc.transform.localPosition = pozycjaStartowa;
    }

    // --- NOWA ANIMACJA WYJŚCIA (Z góry na dół) ---
    IEnumerator AnimujWyjscie()
    {
        if (klientCalosc == null) yield break;

        float czas = 0f;
        // Nie musimy robić SetActive(true), bo klient już tu jest

        while (czas < szybkoscWyskoku)
        {
            czas += Time.deltaTime;
            float procent = czas / szybkoscWyskoku;
            float t = Mathf.SmoothStep(0f, 1f, procent);
            
            // Różnica: Lerpujemy OD pozycji startowej DO ukrytej
            klientCalosc.transform.localPosition = Vector3.Lerp(pozycjaStartowa, pozycjaUkryta, t);
            
            yield return null;
        }
        // Na koniec upewniamy się, że jest pod ladą i go wyłączamy
        klientCalosc.transform.localPosition = pozycjaUkryta;
        klientCalosc.SetActive(false);

        // Dopiero teraz, jak się schował, zaczynamy liczyć czas do następnego
        RozpocznijOdliczanieDoKlienta();
    }
}