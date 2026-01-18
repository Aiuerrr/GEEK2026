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
    
    [Header("--- TIME ---")]
    public float timeToAppear = 3.0f;

    // --- GLOBAL VARIABLES ---
    public static string TextToCopy = ""; 
    public static int WonCoins = 0;
    public static int TotalGold = 0;
    public static bool ReturningFromGame = false;

    [Header("--- ANIMATION ---")]
    public float jumpSpeed = 0.5f;
    private Vector3 startPosition;
    private Vector3 hiddenPosition;

    IEnumerator Start()
    {
        if (machineButton != null) machineButton.interactable = false;

        if (clientWhole != null)
        {
            startPosition = clientWhole.transform.localPosition;
            hiddenPosition = new Vector3(startPosition.x, startPosition.y - 800f, startPosition.z);
        }

        if (cashButton != null) cashButton.SetActive(false);

        if (ReturningFromGame)
        {
            if (clientWhole != null)
            {
                clientWhole.transform.localPosition = startPosition;
                clientWhole.SetActive(true);
            }
            
            SetFaceByResult();

            if (WonCoins > 0)
            {
                ShowCashOnCounter();
                Debug.Log("💰 Client placed cash.");
            }
            else
            {
                Debug.Log("😡 Client doesn't pay and leaves.");
                yield return new WaitForSeconds(2.0f);
                ClientLeaves();
            }
        }
        else
        {
            if (clientWhole != null)
            {
                clientWhole.transform.localPosition = hiddenPosition;
                clientWhole.SetActive(false);
            }
            StartCountdownToClient();
        }
    }

    void ShowCashOnCounter()
    {
        if (cashButton == null) return;

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

    public void PickUpCash()
    {
        TotalGold += WonCoins;
        Debug.Log($"💵 You picked up {WonCoins}. Total: {TotalGold}");

        WonCoins = 0;
        ReturningFromGame = false;

        cashButton.SetActive(false);
        ClientLeaves();
    }

    void ClientLeaves()
    {
        if (machineButton != null) machineButton.interactable = false;

        Debug.Log("👋 Client goes under the counter.");
        if (clientWhole != null) StartCoroutine(AnimateExit());
    }

    void StartCountdownToClient()
    {
        StartCoroutine(WaitForNewClient());
    }

    IEnumerator WaitForNewClient()
    {
        Debug.Log($"⏳ Waiting {timeToAppear}s for new client...");
        yield return new WaitForSeconds(timeToAppear);
        GenerateNewClient();
    }

    void SetFaceByResult()
    {
        if (headImage == null) return;
        if (WonCoins >= 100) headImage.sprite = headHappy;
        else if (WonCoins >= 50) headImage.sprite = headNeutral;
        else headImage.sprite = headAngry;
    }

    void GenerateNewClient()
    {
        if (clientWhole != null) StartCoroutine(AnimateEntry());

        if (headNeutral != null) headImage.sprite = headNeutral;
        TextToCopy = GenerateGibberish(Random.Range(5, 10));
        Debug.Log("🔔 New client!");
    }
    
    string GenerateGibberish(int count)
    {
        string gibberish = "";
        string[] syllables = { "glar", "bo", "zump", "fiko", "tra", "xi", "mu", "ka", "bazz", "qo" };
        for (int i = 0; i < count; i++) gibberish += syllables[Random.Range(0, syllables.Length)] + syllables[Random.Range(0, syllables.Length)] + " ";
        return gibberish.Trim();
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

        StartCountdownToClient();
    }

    public void OnMachineButtonClick()
{
    PaperController paper = FindFirstObjectByType<PaperController>();
    
    if (paper != null)
    {
        paper.SlideIn(TextToCopy);
    }
    
    if (machineButton != null) 
        machineButton.interactable = false;
}
}