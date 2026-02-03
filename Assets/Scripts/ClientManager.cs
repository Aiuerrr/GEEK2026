using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ClientManager : MonoBehaviour
{
    // --- TWORZYMY TECZKĘ OSOBOWĄ ---
    [System.Serializable]
    public class CharacterProfile
    {
        public string nazwaPostaci;
        
        [Header("--- CZY TO STARY POTWÓR? ---")]
        public bool toStaryPotwor; // <--- ZAZNACZ TO TYLKO DLA POTWORA Z TUTORIALA!
        public Sprite cialoStaregoPotwora; // <--- Tu wrzuć CIAŁO (tylko jeśli zaznaczyłeś powyżej)

        [Header("--- GRAFIKI (Dla nowych: Cała postać / Dla starego: Głowa) ---")]
        public Sprite neutralny;    
        public Sprite szczesliwy;   
        public Sprite zly;          
    }
    // --------------------------------

    [Header("--- Client Settings ---")]
    public GameObject clientWhole;
    
    [Header("--- VISUALS ---")]
    public Image bodyImageRenderer; // Główne miejsce wyświetlania (dla nowych: cała postać, dla starego: ciało)
    public Image oldFloatingHeadImage; // Obiekt "Head" wewnątrz clientWhole
    public GameObject oldFloatingHeadObject; // Ten sam obiekt, ale jako GameObject (żeby go włączać/wyłączać)

    [Header("--- LISTY POSTACI ---")]
    public List<CharacterProfile> ludzie;
    public List<CharacterProfile> potwory;

    [Header("--- INTERACTIONS ---")]
    public Button machineButton;
    public GameObject cashButton; 
    public Image cashImage;       
    
    [Header("--- COIN GRAPHICS ---")]
    public Sprite coinsLarge;
    public Sprite coinsMedium;
    public Sprite coinsSmall;

    [Header("--- ANIMATION ---")]
    public float jumpSpeed = 0.5f;
    private Vector3 startPosition;
    private Vector3 hiddenPosition;

    // --- GLOBAL VARIABLES ---
    public static string TextToCopy = ""; 
    public static int WonCoins = 0;
    public static int TotalGold = 0;
    public static bool ReturningFromGame = false;
    
    private CharacterProfile currentProfile; 

    private string[] humanSentences = {
        "Dzien dobry poprosze o raport.",
        "Musze wyslac ten wazny list.",
        "Czy to biuro pisania?",
        "Prosze przepisac to sprawozdanie.",
        "Szybko szef na to czeka.",
        "Mam nadzieje ze macie atrament."
    };

    IEnumerator Start()
    {
        if (machineButton != null) machineButton.interactable = false;

        if (clientWhole != null)
        {
            startPosition = clientWhole.transform.localPosition;
            hiddenPosition = new Vector3(startPosition.x, startPosition.y - 800f, startPosition.z);
            clientWhole.transform.localPosition = hiddenPosition;
            clientWhole.SetActive(false);
        }

        if (cashButton != null) cashButton.SetActive(false);
        yield return null;
    }

    public void StartClientCycle()
    {
        GenerateNewClient();
    }

    public void JobFinished(int earnedMoney)
    {
        Debug.Log($"🏁 Klient otrzymał zlecenie. Zarobek: {earnedMoney}");
        WonCoins = earnedMoney;
        ShowCashOnCounter();
        UpdateFaceEmotion();
    }

    void GenerateNewClient()
    {
        // Domyślnie wyłączamy starą głowę
        if (oldFloatingHeadObject != null) oldFloatingHeadObject.SetActive(false);

        int clientType = Random.Range(0, 2); 
        if (ludzie.Count == 0) clientType = 1;

        if (clientType == 0)
        {
            SetupCharacter(ludzie, true); 
            Debug.Log("🔔 New client! Type: HUMAN");
        }
        else
        {
            SetupCharacter(potwory, false); 
            Debug.Log("🔔 New client! Type: MONSTER");
        }

        if (clientWhole != null) StartCoroutine(AnimateEntry());
    }

    void SetupCharacter(List<CharacterProfile> lista, bool isHuman)
    {
        if (lista.Count > 0)
        {
            currentProfile = lista[Random.Range(0, lista.Count)];

            if (currentProfile.toStaryPotwor)
            {
                // --- LOGIKA DLA STAREGO POTWORA (HYBRYDA) ---
                // 1. Włączamy oddzielną głowę
                if (oldFloatingHeadObject != null) oldFloatingHeadObject.SetActive(true);
                
                // 2. Ciało ustawiamy na sztywno z pola "cialoStaregoPotwora"
                if (bodyImageRenderer != null && currentProfile.cialoStaregoPotwora != null)
                {
                    bodyImageRenderer.sprite = currentProfile.cialoStaregoPotwora;
                    //bodyImageRenderer.SetNativeSize();
                }

                // 3. Twarz ustawiamy na "HeadImage" (tą lewitującą)
                if (oldFloatingHeadImage != null && currentProfile.neutralny != null)
                {
                    oldFloatingHeadImage.sprite = currentProfile.neutralny;
                }
            }
            else
            {
                // --- LOGIKA DLA NOWYCH POSTACI (JEDEN OBRAZEK) ---
                // 1. Wyłączamy oddzielną głowę (już zrobione w GenerateNewClient)
                
                // 2. Ciało to po prostu "neutralny" sprite całej postaci
                if (bodyImageRenderer != null && currentProfile.neutralny != null)
                {
                    bodyImageRenderer.sprite = currentProfile.neutralny;
                    //bodyImageRenderer.SetNativeSize();
                }
            }
        }

        if (isHuman) TextToCopy = humanSentences[Random.Range(0, humanSentences.Length)];
        else TextToCopy = GenerateGibberish(Random.Range(5, 10));
    }

    void UpdateFaceEmotion()
    {
        if (currentProfile == null) return;

        Sprite targetSprite = currentProfile.neutralny; // Domyślnie neutralny

        if (WonCoins >= 100) targetSprite = currentProfile.szczesliwy;
        else if (WonCoins >= 50) targetSprite = currentProfile.neutralny;
        else targetSprite = currentProfile.zly;

        if (targetSprite == null) return;

        // Aplikujemy zmianę w odpowiednie miejsce
        if (currentProfile.toStaryPotwor)
        {
            // Stary potwór -> Zmieniamy sprite na GŁOWIE
            if (oldFloatingHeadImage != null) oldFloatingHeadImage.sprite = targetSprite;
        }
        else
        {
            // Nowa postać -> Zmieniamy CAŁE CIAŁO
            if (bodyImageRenderer != null) bodyImageRenderer.sprite = targetSprite;
        }
    }

    // --- RESZTA FUNKCJI BEZ ZMIAN ---
    void ShowCashOnCounter()
    {
        if (cashButton == null) return;
        if (WonCoins > 0)
        {
            if (WonCoins >= 100) { cashImage.sprite = coinsLarge; cashButton.transform.localScale = Vector3.one; }
            else if (WonCoins >= 50) { cashImage.sprite = coinsMedium; cashButton.transform.localScale = new Vector3(0.8f, 0.8f, 1f); }
            else { cashImage.sprite = coinsSmall; cashButton.transform.localScale = new Vector3(0.6f, 0.6f, 1f); }
            cashButton.SetActive(true);
        }
        else
        {
            Debug.Log("😡 Klient nic nie płaci (0 zł) i wychodzi.");
            StartCoroutine(WaitAndLeave());
        }
    }

    IEnumerator WaitAndLeave()
    {
        yield return new WaitForSeconds(2.0f);
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

        GameLoopManager loop = FindFirstObjectByType<GameLoopManager>();
        if (loop != null) loop.EndRound();
    }

    void ClientLeaves()
    {
        if (machineButton != null) machineButton.interactable = false;
        if (clientWhole != null) StartCoroutine(AnimateExit());
    }

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
        if (game != null) game.StartGame(TextToCopy);
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