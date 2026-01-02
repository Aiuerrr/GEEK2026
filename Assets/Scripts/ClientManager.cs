using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientManager : MonoBehaviour
{
    [Header("--- USTAWIENIA KLIENTA ---")]
    public GameObject klientCalosc;
    public Image obrazekGlowy;
    
    [Header("--- EMOCJE (GRAFIKI) ---")]
    public Sprite glowaSzczesliwa; // 100 monet
    public Sprite glowaSrednia;    // 50 monet
    public Sprite glowaZla;        // 25 lub 0 monet
    
    [Header("--- CZAS ---")]
    public float czasDoPojawienia = 3.0f; // Czas na przyjście nowego
    public float czasNaPozegnanie = 3.0f; // Ile czasu klient stoi po oddaniu zlecenia

    // --- ZMIENNE GLOBALNE ---
    public static string TekstDoPrzepisania = ""; 
    public static int WygraneMonety = 0;
    public static bool WracamyZGry = false; // Czy właśnie skończyliśmy pisać?

    IEnumerator Start()
    {
        // SCENARIUSZ A: Wracamy z gry (Klient ocenia pracę)
        if (WracamyZGry)
        {
            // 1. Upewniamy się, że klient jest widoczny
            if (klientCalosc != null) klientCalosc.SetActive(true);

            // 2. Ustawiamy minę w zależności od wyniku
            UstawMinePoWyniku();

            Debug.Log($"💰 Odbierasz wypłatę: {WygraneMonety}. Klient ocenia...");

            // 3. Czekamy, aż gracz zobaczy reakcję (np. 3 sekundy)
            yield return new WaitForSeconds(czasNaPozegnanie);

            // 4. Resetujemy flagi i chowamy klienta
            WygraneMonety = 0;
            WracamyZGry = false;
            
            if (klientCalosc != null) klientCalosc.SetActive(false);
            Debug.Log("👋 Klient sobie poszedł.");
        }
        else
        {
            // SCENARIUSZ B: Dopiero włączyliśmy grę
            if (klientCalosc != null) klientCalosc.SetActive(false);
        }

        // --- PĘTLA ROZGRYWKI (Czekamy na nowego) ---
        
        Debug.Log($"⏳ Czekam {czasDoPojawienia} sekund na nowego klienta...");
        yield return new WaitForSeconds(czasDoPojawienia);

        GenerujNowegoKlienta();
    }

    void UstawMinePoWyniku()
    {
        if (obrazekGlowy == null) return;

        if (WygraneMonety >= 100)
        {
            if(glowaSzczesliwa) obrazekGlowy.sprite = glowaSzczesliwa; // Super!
        }
        else if (WygraneMonety == 50)
        {
            if(glowaSrednia) obrazekGlowy.sprite = glowaSrednia; // Meh.
        }
        else
        {
            if(glowaZla) obrazekGlowy.sprite = glowaZla; // Tragedia.
        }
        
        // ewentualnie: obrazekGlowy.SetNativeSize();
    }

    void GenerujNowegoKlienta()
    {
        if (klientCalosc != null) klientCalosc.SetActive(true);

        // Tutaj na start dajemy zawsze neutralną, albo losujemy (jak wolisz)
        // Na razie damy średnią jako "domyślną" na wejście
        if (glowaSrednia != null && obrazekGlowy != null) 
            obrazekGlowy.sprite = glowaSrednia;

        TekstDoPrzepisania = GenerujBelkot(Random.Range(5, 10));
        Debug.Log("🔔 DING! Nowy klient! Zadanie: " + TekstDoPrzepisania);
    }

    string GenerujBelkot(int iloscSlow)
    {
        string belkot = "";
        string[] sylaby = { "glar", "bo", "zump", "fiko", "tra", "xi", "mu", "ka", "bazz", "qo", "vip", "dox" };
        for (int i = 0; i < iloscSlow; i++)
        {
            string slowo = sylaby[Random.Range(0, sylaby.Length)] + sylaby[Random.Range(0, sylaby.Length)];
            belkot += slowo + " ";
        }
        return belkot.Trim();
    }
}