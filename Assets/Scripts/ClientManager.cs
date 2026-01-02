using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientManager : MonoBehaviour
{
    [Header("--- USTAWIENIA KLIENTA ---")]
    public GameObject klientCalosc;
    public Image obrazekGlowy;
    
    [Header("--- PIENIĄDZE (INTERAKCJA) ---")]
    public GameObject przyciskKasy; // Tu wrzuć obiekt Money_Pile
    public Image obrazekKasy;       // Tu wrzuć komponent Image z Money_Pile
    
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
    public static int WygraneMonety = 0; // Ile zarobiliśmy w tej rundzie
    public static int CalkowiteZloto = 0; // Ile mamy w portfelu ogólnie
    public static bool WracamyZGry = false;

    [Header("--- ANIMACJA ---")]
    public float szybkoscWyskoku = 0.5f; // Jak szybko wyskakuje (w sekundach)
    private Vector3 pozycjaStartowa;     // Gdzie klient stoi normalnie (zapamiętamy to)
    private Vector3 pozycjaUkryta;       // Gdzie klient chowa się pod ladą

    IEnumerator Start()
    {
        // --- KONFIGURACJA ANIMACJI ---
        if (klientCalosc != null)
        {
            pozycjaStartowa = klientCalosc.transform.localPosition;
            // Obliczamy pozycję pod ladą (np. 500 pikseli niżej)
            pozycjaUkryta = new Vector3(pozycjaStartowa.x, pozycjaStartowa.y - 800f, pozycjaStartowa.z);
            
            // Na dzień dobry chowamy go pod ladę fizycznie
            klientCalosc.transform.localPosition = pozycjaUkryta;
        }
        // Ukrywamy kasę na start (dla pewności)
        if (przyciskKasy != null) przyciskKasy.SetActive(false);

        if (WracamyZGry)
        {
            // 1. Klient jest i ocenia
            if (klientCalosc != null) klientCalosc.SetActive(true);
            UstawMinePoWyniku();

            // 2. Jeśli zarobiliśmy cokolwiek (>0), wykładamy kasę
            if (WygraneMonety > 0)
            {
                PokazKaseNaLadzie();
                Debug.Log("💰 Klient położył kasę. Czekam aż podniesiesz...");
                // Tutaj skrypt KOŃCZY działanie Start(). 
                // Dalsza akcja (zniknięcie klienta) wydarzy się dopiero jak klikniesz przycisk!
            }
            else
            {
                // Jeśli 0 monet (przegrana), klient jest zły i idzie sobie sam po chwili
                Debug.Log("😡 Klient nic nie płaci i wychodzi.");
                yield return new WaitForSeconds(2.0f);
                KlientSobieIdzie();
            }
        }
        else
        {
            // Start gry od zera
            if (klientCalosc != null) klientCalosc.SetActive(false);
            RozpocznijOdliczanieDoKlienta();
        }
    }

    void PokazKaseNaLadzie()
    {
        if (przyciskKasy == null) return;

        // 1. Wybieramy grafikę I USTAWIAMY SKALĘ
        // Zakładamy, że na scenie ustawiłeś przycisk tak, żeby pasował do DUŻEJ góry złota (Skala 1,1,1)
        
        if (WygraneMonety >= 100)
        {
            obrazekKasy.sprite = monetyDuzo;
            przyciskKasy.transform.localScale = Vector3.one; // Skala 1.0 (100%)
        }
        else if (WygraneMonety >= 50)
        {
            obrazekKasy.sprite = monetySrednio;
            przyciskKasy.transform.localScale = new Vector3(0.8f, 0.8f, 1f); // Skala 0.8 (80%)
        }
        else
        {
            obrazekKasy.sprite = monetyMalo;
            przyciskKasy.transform.localScale = new Vector3(0.6f, 0.6f, 1f); // Skala 0.6 (60%)
        }
        
        // 2. Włączamy obrazek kasy (bez SetNativeSize, bo robi giganty!)
        przyciskKasy.SetActive(true); 
    }

    // TĘ FUNKCJĘ PODEPNIEMY POD PRZYCISK "Money_Pile"
    public void PodniesKase()
    {
        // 1. Dodaj do portfela
        CalkowiteZloto += WygraneMonety;
        Debug.Log($"💵 Podniosłeś {WygraneMonety}. Razem masz: {CalkowiteZloto}");

        // 2. Zeruj tymczasowy zarobek
        WygraneMonety = 0;
        WracamyZGry = false;

        // 3. Ukryj kasę
        przyciskKasy.SetActive(false);

        // 4. Klient odchodzi
        KlientSobieIdzie();
    }

    void KlientSobieIdzie()
    {
        if (klientCalosc != null) klientCalosc.SetActive(false);
        Debug.Log("👋 Klient poszedł.");
        
        RozpocznijOdliczanieDoKlienta();
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
        // (Twoja stara funkcja od min - bez zmian)
        if (obrazekGlowy == null) return;
        if (WygraneMonety >= 100) obrazekGlowy.sprite = glowaSzczesliwa;
        else if (WygraneMonety >= 50) obrazekGlowy.sprite = glowaSrednia;
        else obrazekGlowy.sprite = glowaZla;
    }

    void GenerujNowegoKlienta()
    {
        // (Twoja stara funkcja generowania - bez zmian)
        if (klientCalosc != null) StartCoroutine(AnimujWejscie());
        if (glowaSrednia != null) obrazekGlowy.sprite = glowaSrednia;
        TekstDoPrzepisania = GenerujBelkot(Random.Range(5, 10));
        Debug.Log("🔔 Nowy klient!");
    }
    
    string GenerujBelkot(int ilosc)
    {
        // (Twoja funkcja belkotu - skopiuj ją sobie ze starego skryptu albo napisz, jeśli jej tu nie ma)
        string belkot = "";
        string[] sylaby = { "glar", "bo", "zump", "fiko", "tra", "xi", "mu", "ka", "bazz", "qo" };
        for (int i = 0; i < ilosc; i++) belkot += sylaby[Random.Range(0, sylaby.Length)] + sylaby[Random.Range(0, sylaby.Length)] + " ";
        return belkot.Trim();
    }
    IEnumerator AnimujWejscie()
    {
        if (klientCalosc == null) yield break;

        float czas = 0f;
        klientCalosc.SetActive(true); // Pokaż go (jest pod ladą)

        while (czas < szybkoscWyskoku)
        {
            czas += Time.deltaTime;
            float procent = czas / szybkoscWyskoku;
            
            // "Lerp" to magiczna funkcja do płynnego ruchu od A do B
            // Dodajemy "SmoothStep", żeby ruch był bardziej naturalny (miękki start i stop)
            float t = Mathf.SmoothStep(0f, 1f, procent);
            
            klientCalosc.transform.localPosition = Vector3.Lerp(pozycjaUkryta, pozycjaStartowa, t);
            
            yield return null; // Czekaj jedną klatkę
        }

        // Dla pewności na koniec ustawiamy idealnie na miejscu
        klientCalosc.transform.localPosition = pozycjaStartowa;
    }
}