using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Upewnij się, że masz to, jeśli używasz TextMeshPro w ClientVisuals, jeśli nie - usuń.

public class ClientManager : MonoBehaviour
{
    // --- TWORZYMY TECZKĘ OSOBOWĄ ---
    [System.Serializable]
    public class CharacterProfile
    {
        public string nazwaPostaci;
        
        [Header("--- CZY TO STARY POTWÓR? ---")]
        public bool toStaryPotwor; 
        public Sprite cialoStaregoPotwora;

        [Header("--- GRAFIKI ---")]
        public Sprite neutralny;    
        public Sprite szczesliwy;   
        public Sprite zly;          
    }

    [Header("--- Client Settings ---")]
    public GameObject clientWhole;
    
    [Header("--- VISUALS ---")]
    public Image bodyImageRenderer; 
    public Image oldFloatingHeadImage; 
    public GameObject oldFloatingHeadObject; 

    [Header("--- LISTY POSTACI ---")]
    public List<CharacterProfile> ludzie;
    public List<CharacterProfile> potwory;

    [Header("--- DIALOGI (POWITANIA) ---")]
    public string[] humanDialogues = {
        "Dzień dobry!", "To pilne!", "Szef czeka...", "Tylko szybko.", 
        "Ładna pogoda.", "Poproszę to.", "Dużo tego?", "Kawa stygnie."
    };

    public string[] monsterDialogues = {
        "Grrr...", "Ubongo...", "Kustahili...", "Andika!", "Argh!", 
        "Kwa kwa?", "Habari", "..." 
    };

    [Header("--- HUMAN FEEDBACK (Ludzie) ---")]
    public string[] humanGood = { "Dzięki, świetna robota!", "O to chodziło.", "Zostawię napiwek.", "Bardzo profesjonalnie.", "Uratowałeś mi życie!" };
    public string[] humanMedium = { "Dzięki.", "Może być.", "Trochę długo...", "Następnym razem szybciej.", "Wezmę to." };
    public string[] humanBad = { "To jest skandal!", "Jedna gwiazdka w Google!", "Chcę rozmawiać z kierownikiem!", "To nie to, co chciałem!", "Nigdy więcej tu nie przyjdę." };

    [Header("--- MONSTER FEEDBACK (Potwory) ---")]
    public string[] monsterGood = { "Mmm... ladha...", "Nzuri!", "Hrr... Baridi", "Nafurahi!", "Shukrani!" };
    public string[] monsterMedium = { "Grrr...", "Ilyokunjamana...", "labda damu...", "haraka intaodoka.", "Hmpf." };
    public string[] monsterBad = { "Nina hasira ", "NITAKULA WEWE!!!", "NJA!!!", "MBAJA! MBAJA!"};

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
    private bool czyToPotwor = false;

    private string[] humanSentences = {
        "Dzien dobry poprosze o raport.", "Musze wyslac ten wazny list.", "Czy to biuro pisania?",
        "Prosze przepisac to sprawozdanie.", "Szybko szef na to czeka.", "Mam nadzieje ze macie atrament."
    };

    IEnumerator Start()
    {
        // 1. Blokujemy przycisk na start
        if (machineButton != null) machineButton.interactable = false;

        if (clientWhole != null)
        {
            startPosition = clientWhole.transform.localPosition;
            hiddenPosition = new Vector3(startPosition.x, startPosition.y - 800f, startPosition.z);
            clientWhole.transform.localPosition = hiddenPosition;
            
            // 2. NAPRAWA "HEJ!": Wyłączamy dymek od razu na starcie gry
            ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
            if (visuals != null) visuals.HideBubble();

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
        UpdateClientReaction(earnedMoney);
        ShowCashOnCounter();
        UpdateFaceEmotion();
    }

    void GenerateNewClient()
    {
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
        czyToPotwor = !isHuman; // Zapamiętujemy kim jest postać

        if (lista.Count > 0)
        {
            currentProfile = lista[Random.Range(0, lista.Count)];

            if (currentProfile.toStaryPotwor)
            {
                if (oldFloatingHeadObject != null) oldFloatingHeadObject.SetActive(true);
                if (bodyImageRenderer != null && currentProfile.cialoStaregoPotwora != null)
                    bodyImageRenderer.sprite = currentProfile.cialoStaregoPotwora;
                if (oldFloatingHeadImage != null && currentProfile.neutralny != null)
                    oldFloatingHeadImage.sprite = currentProfile.neutralny;
            }
            else
            {
                if (oldFloatingHeadObject != null) oldFloatingHeadObject.SetActive(false);
                if (bodyImageRenderer != null && currentProfile.neutralny != null)
                    bodyImageRenderer.sprite = currentProfile.neutralny;
            }
        }

        if (isHuman) TextToCopy = humanSentences[Random.Range(0, humanSentences.Length)];
        else TextToCopy = GenerateGibberish(Random.Range(5, 10));
    }

    // --- LOGIKA ANIMACJI WEJŚCIA (Z POPRAWKAMI) ---
    IEnumerator AnimateEntry()
    {
        if (clientWhole == null) yield break;

        // 1. BLOKADA PRZYCISKU (Naprawa błędu resetowania gry przez kliknięcie za wcześnie)
        if (machineButton != null) machineButton.interactable = false;

        // 2. START: Pokazujemy postać, ale UKRYWAMY dymek (Naprawa błędu "Hej!")
        clientWhole.SetActive(true);
        ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
        if (visuals != null) visuals.HideBubble(); 

        // 3. ANIMACJA RUCHU
        float time = 0f;
        while (time < jumpSpeed)
        {
            time += Time.deltaTime;
            float percent = time / jumpSpeed;
            float t = Mathf.SmoothStep(0f, 1f, percent);
            clientWhole.transform.localPosition = Vector3.Lerp(hiddenPosition, startPosition, t);
            yield return null;
        }
        
        clientWhole.transform.localPosition = startPosition;

        // 4. CZEKAMY (1 sekunda ciszy - szybciej)
        yield return new WaitForSeconds(1.0f); 

        // 5. GENERUJEMY TEKST I POKAZUJEMY DYMEK
        if (visuals != null)
        {
            string randomText = "";
            if (czyToPotwor) 
                 randomText = monsterDialogues[Random.Range(0, monsterDialogues.Length)];
            else
                 randomText = humanDialogues[Random.Range(0, humanDialogues.Length)];

            visuals.ShowBubble(randomText);
        }

        // 6. START GRY
        TypingGameManager game = FindFirstObjectByType<TypingGameManager>();
        if (game != null) game.StartGame(TextToCopy);

        // 7. ODBLOKOWANIE MASZYNY - Dopiero TERAZ pozwalamy graczowi kliknąć!
        if (machineButton != null) machineButton.interactable = true; 
    }

    // --- LOGIKA ZBIERANIA KASY I WYJŚCIA ---
    public void PickUpCash()
    {
        StartCoroutine(PickUpCashSequence());
    }

    IEnumerator PickUpCashSequence()
    {
        TotalGold += WonCoins;
        WonCoins = 0;
        ReturningFromGame = false;
        cashButton.SetActive(false);

        // Chowamy dymek
        ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
        if (visuals != null) visuals.HideBubble();

        // Ręczna animacja wyjścia
        float time = 0f;
        Vector3 currentPos = clientWhole.transform.localPosition;
        
        while (time < jumpSpeed)
        {
            time += Time.deltaTime;
            float percent = time / jumpSpeed;
            float t = percent * percent * (3f - 2f * percent); 
            clientWhole.transform.localPosition = Vector3.Lerp(currentPos, hiddenPosition, t);
            yield return null;
        }

        clientWhole.transform.localPosition = hiddenPosition;
        clientWhole.SetActive(false);

        // Pauza gry (Koniec Rundy)
        GameLoopManager loop = FindFirstObjectByType<GameLoopManager>();
        if (loop != null) loop.EndRound();
    }

    // --- AUTOMATYCZNE WYJŚCIE (np. jak gracz nic nie zrobi) ---
    void ClientLeaves()
    {
        if (machineButton != null) machineButton.interactable = false;
        if (clientWhole != null) StartCoroutine(AnimateExit());
    }

    IEnumerator AnimateExit()
    {
        // Ukrywamy dymek
        ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
        if (visuals != null) visuals.HideBubble();

        if (clientWhole == null) yield break;
        
        // Animacja wyjścia (taka sama jak w PickUpCash, ale bez pauzowania gry na końcu)
        float time = 0f;
        Vector3 currentPos = clientWhole.transform.localPosition;

        while (time < jumpSpeed)
        {
            time += Time.deltaTime; 
            float percent = time / jumpSpeed;
            float t = Mathf.SmoothStep(0f, 1f, percent);
            clientWhole.transform.localPosition = Vector3.Lerp(currentPos, hiddenPosition, t);
            yield return null;
        }

        clientWhole.transform.localPosition = hiddenPosition;
        clientWhole.SetActive(false);
    }

    // --- HELPERY I UI ---
    void UpdateClientReaction(int money)
    {
        string reactionText = "";

        if (czyToPotwor)
        {
            if (money >= 100) reactionText = monsterGood[Random.Range(0, monsterGood.Length)];
            else if (money >= 50) reactionText = monsterMedium[Random.Range(0, monsterMedium.Length)];
            else reactionText = monsterBad[Random.Range(0, monsterBad.Length)];
        }
        else
        {
            if (money >= 100) reactionText = humanGood[Random.Range(0, humanGood.Length)];
            else if (money >= 50) reactionText = humanMedium[Random.Range(0, humanMedium.Length)];
            else reactionText = humanBad[Random.Range(0, humanBad.Length)];
        }

        if (clientWhole != null)
        {
            ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
            if (visuals != null) visuals.ShowBubble(reactionText);
        }
    }

    void UpdateFaceEmotion()
    {
        if (currentProfile == null) return;
        Sprite targetSprite = currentProfile.neutralny; 

        if (WonCoins >= 100) targetSprite = currentProfile.szczesliwy;
        else if (WonCoins >= 50) targetSprite = currentProfile.neutralny;
        else targetSprite = currentProfile.zly;

        if (targetSprite == null) return;

        if (currentProfile.toStaryPotwor)
        {
            if (oldFloatingHeadImage != null) oldFloatingHeadImage.sprite = targetSprite;
        }
        else
        {
            if (bodyImageRenderer != null) bodyImageRenderer.sprite = targetSprite;
        }
    }

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