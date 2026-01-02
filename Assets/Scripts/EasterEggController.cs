using UnityEngine;
using UnityEngine.UI; // Potrzebne, żeby widzieć komponent Image
using System.Collections; // Potrzebne do obsługi "czekania" (Korutyny)

public class EasterEggController : MonoBehaviour
{
    [Header("Ustawienia")]
    public Image obrazekEasterEgga; // Tu wrzucimy komponent Image tego przycisku
    public Sprite postacSpoczynek;  // Grafika "normalna"
    public Sprite postacNacisnieta; // Grafika "aktywna"
    public float czasTrwania = 3.0f; // Ile sekund ma trwać efekt (możesz zmienić w Inspectorze)

    private Coroutine licznikCzasu; // Zmienna pomocnicza do kontrolowania czasu

    // Tę funkcję podepniemy pod kliknięcie przycisku
    public void AktywujEasterEgg()
    {
        // 1. Natychmiast zmień obrazek na "naciśnięty"
        obrazekEasterEgga.sprite = postacNacisnieta;

        // 2. Jeśli jakiś licznik już odliczał (bo gracz klika jak szalony), zatrzymaj go
        if (licznikCzasu != null)
        {
            StopCoroutine(licznikCzasu);
        }

        // 3. Uruchom nowe odliczanie
        licznikCzasu = StartCoroutine(OdliczajICofnij());
    }

    // To jest specjalna funkcja, która potrafi "czekać" w czasie gry
    IEnumerator OdliczajICofnij()
    {
        // Czekaj tyle sekund, ile ustawiliśmy w zmiennej 'czasTrwania'
        yield return new WaitForSeconds(czasTrwania);

        // Po upływie czasu, przywróć normalny obrazek
        obrazekEasterEgga.sprite = postacSpoczynek;
        
        // Wyzeruj licznik
        licznikCzasu = null;
    }
}