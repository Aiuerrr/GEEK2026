using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientManager : MonoBehaviour
{
    [Header("--- Client Settings ---")]
    public GameObject clientWhole;
    public Image headImage;
    
    [Header("--- INTERACTIONS ---")]
    public Button machineButton;
    public GameObject cashButton; 
    public Image cashImage;       
    
    [Header("--- COIN GRAPHICS ---")]
    public Sprite coinsLarge;
    public Sprite coinsMedium;
    public Sprite coinsSmall;

    [Header("--- FACE GRAPHICS ---")]
    public Sprite headHappy;
    public Sprite headNeutral;
    public Sprite headAngry;
    
    [Header("--- ANIMATION ---")]
    public float jumpSpeed = 0.5f;
    private Vector3 startPosition;
    private Vector3 hiddenPosition;

    // --- GLOBAL VARIABLES ---
    public static string TextToCopy = ""; 
    public static int WonCoins = 0;
    public static int TotalGold = 0;
    public static bool ReturningFromGame = false;

    IEnumerator Start()
    {
        // TYLKO KONFIGURACJA POCZĄTKOWA
        if (machineButton != null) machineButton.interactable = false;

        if (clientWhole != null)
        {
            startPosition = clientWhole.transform.localPosition;
            hiddenPosition = new Vector3(startPosition.x, startPosition.y - 800f, startPosition.z);
            
            // Chowamy klienta pod ladę na start
            clientWhole.transform.localPosition = hiddenPosition;
            clientWhole.SetActive(false);
        }

        if (cashButton != null) cashButton.SetActive(false);
        
        yield return null;
    }

    // --- KOMUNIKACJA Z GAME LOOP MANAGEREM ---
    public void StartClientCycle()
    {
        GenerateNewClient();
    }

    // --- KOMUNIKACJA Z TYPING GAME MANAGEREM ---
    // <--- NOWE: Tę funkcję wywoła gra w pisanie, gdy skończysz zadanie
    public void JobFinished(int earnedMoney)
    {
        Debug.Log($"🏁 Klient otrzymał zlecenie. Zarobek: {earnedMoney}");
        
        WonCoins = earnedMoney;
        ShowCashOnCounter();
        SetFaceByResult();
    }

    void GenerateNewClient()
    {
        if (clientWhole != null) StartCoroutine(AnimateEntry());

        if (headNeutral != null) headImage.sprite = headNeutral;
        TextToCopy = GenerateGibberish(Random.Range(5, 10));
        Debug.Log("🔔 New client!");
    }

    // --- INTERAKCJA Z KASĄ ---

    void ShowCashOnCounter()
    {
        if (cashButton == null) return;

        // Pokazujemy kasę tylko, jeśli zarobiliśmy cokolwiek
        if (WonCoins > 0)
        {
            if (WonCoins >= 100)
            {
                cashImage.sprite = coinsLarge;
                cashButton.transform.localScale = Vector3.one;
            }
            else if (WonCoins >= 50)
            {
                cashImage.sprite = coinsMedium;
                cashButton.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
            else
            {
                cashImage.sprite = coinsSmall;
                cashButton.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            }
            cashButton.SetActive(true);
        }
        else
        {
            // Jeśli 0 zł, to od razu klient wychodzi (nie ma czego zbierać)
            Debug.Log("😡 Klient nic nie płaci (0 zł) i wychodzi.");
            StartCoroutine(WaitAndLeave());
        }
    }

    IEnumerator WaitAndLeave()
    {
        yield return new WaitForSeconds(2.0f);
        // Powiadamiamy Managera, że runda stracona (0 zł), ale skończona
        GameLoopManager loop = FindFirstObjectByType<GameLoopManager>();
        if (loop != null) loop.EndRound();
        
        ClientLeaves();
    }

    public void PickUpCash()
    {
        TotalGold += WonCoins;
        Debug.Log($"💵 You picked up {WonCoins}. Total: {TotalGold}");

        WonCoins = 0;
        ReturningFromGame = false;

        cashButton.SetActive(false);
        ClientLeaves();

        // Powiadom GameLoopManager, że runda skończona!
        GameLoopManager loop = FindFirstObjectByType<GameLoopManager>();
        if (loop != null)
        {
            loop.EndRound();
        }
        else
        {
            Debug.LogError("BRAK GameLoopManager na scenie!");
        }
    }

    void ClientLeaves()
    {
        if (machineButton != null) machineButton.interactable = false;
        Debug.Log("👋 Client leaves.");
        if (clientWhole != null) StartCoroutine(AnimateExit());
    }

    // --- ANIMACJE ---

    IEnumerator AnimateEntry()
    {
        if (clientWhole == null) yield break;

        float time = 0f;
        clientWhole.SetActive(true);
        while (time < jumpSpeed)
        {
            time += Time.deltaTime;
            float percent = time / jumpSpeed;
            float t = Mathf.SmoothStep(0f, 1f, percent);
            clientWhole.transform.localPosition = Vector3.Lerp(hiddenPosition, startPosition, t);
            yield return null;
        }
        clientWhole.transform.localPosition = startPosition;

        if (machineButton != null) machineButton.interactable = true;

        TypingGameManager game = FindFirstObjectByType<TypingGameManager>();
        if (game != null)
        {
            game.StartGame(TextToCopy); 
        }
    }

    IEnumerator AnimateExit()
    {
        if (clientWhole == null) yield break;

        float time = 0f;
        while (time < jumpSpeed)
        {
            time += Time.deltaTime;
            float percent = time / jumpSpeed;
            float t = Mathf.SmoothStep(0f, 1f, percent);
            clientWhole.transform.localPosition = Vector3.Lerp(startPosition, hiddenPosition, t);
            yield return null;
        }
        clientWhole.transform.localPosition = hiddenPosition;
        clientWhole.SetActive(false);
    }
    
    // --- UTILITIES ---

    public void SetFaceByResult()
    {
        if (headImage == null) return;
        if (WonCoins >= 100) headImage.sprite = headHappy;
        else if (WonCoins >= 50) headImage.sprite = headNeutral;
        else headImage.sprite = headAngry;
    }

    string GenerateGibberish(int count)
    {
        string gibberish = "";
        string[] syllables = { "glar", "bo", "zump", "fiko", "tra", "xi", "mu", "ka", "bazz", "qo" };
        for (int i = 0; i < count; i++) gibberish += syllables[Random.Range(0, syllables.Length)] + syllables[Random.Range(0, syllables.Length)] + " ";
        return gibberish.Trim();
    }

    public void OnMachineButtonClick()
    {
        PaperController paper = FindFirstObjectByType<PaperController>();
        if (paper != null) paper.SlideIn(TextToCopy);
        if (machineButton != null) machineButton.interactable = false;
    }
}