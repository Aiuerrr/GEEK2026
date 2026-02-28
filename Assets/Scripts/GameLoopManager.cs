using UnityEngine;
using TMPro; 
using System.Collections;

public class GameLoopManager : MonoBehaviour
{
    [Header("--- ROUND CONFIG ---")]
    public int currentRound = 1;
    public int rentInterval = 10; 
    public int rentAmount = 3000;
    public float rentMultiplier = 1.5f; 

    [Header("--- TIMING ---")]
    public float delayBetweenRounds = 2.0f; 

    [Header("--- UI ---")]
    public TextMeshProUGUI roundText; 
    public EndScreenController endScreenController; 

    [Header("--- REFERENCES ---")]
    public ClientManager clientSystem; 

    private void Start()
    {
        StartNewRound();
    }

    public void StartNewRound()
    {
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
            
            if (endScreenController != null)
            {
                endScreenController.ShowEndScreen(true, currentRound, ClientManager.TotalGold);
            }
            
            rentAmount = Mathf.RoundToInt(rentAmount * rentMultiplier);
            AdvanceRound();
        }
        else
        {
            if (endScreenController != null)
            {
                endScreenController.ShowEndScreen(false, currentRound, currentGold);
            }
        }
    }

    void AdvanceRound()
    {
        currentRound++;
        StartCoroutine(WaitAndStartNewRound());
    }

    IEnumerator WaitAndStartNewRound()
    {
        yield return new WaitForSeconds(delayBetweenRounds);
        StartNewRound();
    }
}