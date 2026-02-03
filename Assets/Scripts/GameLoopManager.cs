using UnityEngine;
using TMPro; 
using System.Collections;

public class GameLoopManager : MonoBehaviour
{
    [Header("--- ROUND CONFIG ---")]
    public int currentRound = 1;
    public int rentInterval = 3; 
    public int rentAmount = 100;
    public float rentMultiplier = 1.5f; 

    [Header("--- TIMING ---")]
    public float delayBetweenRounds = 2.0f; 

    [Header("--- UI ---")]
    public TextMeshProUGUI roundText; 
    
    // <--- ZMIANA: Zamiast paneli, mamy kontroler ekranu końcowego
    public EndScreenController endScreenController; 

    [Header("--- REFERENCES ---")]
    public ClientManager clientSystem; 

    private void Start()
    {
        StartNewRound();
    }

    public void StartNewRound()
    {
        Debug.Log($"🔵 STARTING ROUND: {currentRound}");
        if (roundText != null) roundText.text = "Round: " + currentRound;
        if (clientSystem != null) clientSystem.StartClientCycle(); 
    }

    public void EndRound()
    {
        if (currentRound % rentInterval == 0)
        {
            PayRent();
        }
        else
        {
            AdvanceRound();
        }
    }

    void PayRent()
    {
        int currentGold = ClientManager.TotalGold;

        if (currentGold >= rentAmount)
        {
            ClientManager.TotalGold -= rentAmount;
            
            // <--- OPCJONALNIE: Tu możemy pokazać ekran sukcesu co czynsz (jeśli chcesz)
            // Ale standardowo Endless Mode włącza się np. po 10 rundzie.
            // Na razie zróbmy prosto: zapłacił -> gra dalej.
            
            rentAmount = Mathf.RoundToInt(rentAmount * rentMultiplier);
            AdvanceRound();
        }
        else
        {
            // PRZEGRANA (Brak kasy)
            Debug.LogError("💀 GAME OVER - Brak kasy na czynsz!");
            
            if (endScreenController != null)
            {
                // false = przegrana
                endScreenController.ShowEndScreen(false, currentRound, currentGold);
            }
        }
    }

    void AdvanceRound()
    {
        currentRound++;
        
        // <--- LOGIKA WYGRANEJ (np. po 10 rundzie)
        // Jeśli chcesz, żeby po 10 rundzie wyskoczył ekran "Gratulacje" z opcją Endless:
        if (currentRound == 11) // Tu wpisz po której rundzie ma być "Win"
        {
             Debug.Log("🏆 WYGRANA! Ale możesz grać dalej.");
             if (endScreenController != null)
             {
                 // true = wygrana
                 endScreenController.ShowEndScreen(true, currentRound, ClientManager.TotalGold);
                 // Jak kliknie "Endless", panel zniknie i gra pójdzie dalej (kod poniżej WaitAndStart)
             }
             // Nie robimy return, bo jak zamknie panel, to coroutine i tak musi ruszyć
        }

        StartCoroutine(WaitAndStartNewRound());
    }

    IEnumerator WaitAndStartNewRound()
    {
        // Jeśli właśnie pokazaliśmy ekran końcowy, gra jest spauzowana (TimeScale = 0).
        // Musimy poczekać, aż gracz kliknie "Endless" i odblokuje czas.
        
        yield return new WaitForSeconds(delayBetweenRounds);
        StartNewRound();
    }
}