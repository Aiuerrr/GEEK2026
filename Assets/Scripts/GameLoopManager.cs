using UnityEngine;
using TMPro; 
using System.Collections; // Potrzebne do IEnumerator (czekania)

public class GameLoopManager : MonoBehaviour
{
    [Header("--- ROUND CONFIG ---")]
    public int currentRound = 1;
    public int rentInterval = 3; 
    public int rentAmount = 100;
    public float rentMultiplier = 1.5f; 

    [Header("--- TIMING ---")]
    public float delayBetweenRounds = 2.0f; // <--- NOWE: Czas przerwy między klientami

    [Header("--- UI ---")]
    public TextMeshProUGUI roundText; 
    public GameObject gameOverPanel; 
    public GameObject winPanel;   

    [Header("--- REFERENCES ---")]
    public ClientManager clientSystem; 

    private void Start()
    {
        // Na start gry też dajmy chwilę oddechu, albo startujmy od razu
        StartNewRound();
    }

    public void StartNewRound()
    {
        Debug.Log($"🔵 STARTING ROUND: {currentRound}");
        
        if (roundText != null) roundText.text = "Round: " + currentRound;

        if (clientSystem != null)
        {
            clientSystem.StartClientCycle(); 
        }
    }

    public void EndRound()
    {
        Debug.Log("🏁 Round ended.");

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
        Debug.Log($"💸 RENT TIME! Need: {rentAmount}");

        int currentGold = ClientManager.TotalGold;

        if (currentGold >= rentAmount)
        {
            ClientManager.TotalGold -= rentAmount;
            Debug.Log($"✅ Rent paid. Remaining: {ClientManager.TotalGold}");
            
            rentAmount = Mathf.RoundToInt(rentAmount * rentMultiplier);
            
            AdvanceRound();
        }
        else
        {
            Debug.LogError("💀 GAME OVER - Can't pay rent!");
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }
    }

    void AdvanceRound()
    {
        currentRound++;
        
        if (currentRound > 10)
        {
             Debug.Log("🏆 WIN! You survived 10 rounds.");
             if (winPanel != null) winPanel.SetActive(true);
             return;
        }

        // ZAMIAST OD RAZU STARTOWAĆ - CZEKAMY!
        StartCoroutine(WaitAndStartNewRound());
    }

    // <--- NOWE: To jest ta poczekalnia
    IEnumerator WaitAndStartNewRound()
    {
        Debug.Log($"⏳ Waiting {delayBetweenRounds}s for the next client...");
        
        // Czekamy tyle sekund, ile ustawisz w Inspectorze
        yield return new WaitForSeconds(delayBetweenRounds);
        
        // Dopiero teraz wchodzi nowy
        StartNewRound();
    }
}