using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public RectTransform bubbleRect; 
    public TextMeshProUGUI tutorialText;
    public Button gotItButton;

    [Header("Animation & Positions")]
    public float animationDuration = 0.3f;
    public Vector2[] stepPositions;

    private int currentStep = 0;
    private bool isTutorialActive = false;
    private Coroutine animationCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)
        {
            StartTutorial();
        }
        else
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }
    }

    public void StartTutorial()
    {
        isTutorialActive = true;
        currentStep = 0;
        tutorialPanel.SetActive(true);
        ShowCurrentStep();
    }

    public void AdvanceTutorial(int expectedStep)
    {
        if (!isTutorialActive) return;

        if (currentStep == expectedStep)
        {
            currentStep++;
            ShowCurrentStep();
        }
    }

    void ShowCurrentStep()
    {
        gotItButton.gameObject.SetActive(false);

        if (stepPositions != null && currentStep < stepPositions.Length)
        {
            if (bubbleRect != null) bubbleRect.anchoredPosition = stepPositions[currentStep];
        }

        switch (currentStep)
        {
            case 0:
                tutorialText.text = "Witaj w drukarni!\n\nTwój pierwszy klient ju¿ czeka. Kliknij maszynê do pisania, aby zacz¹æ zlecenie.";
                break;
            case 1:
                tutorialText.text = "Przepisuj tekst najszybciej jak potrafisz. Im wy¿sza bezb³êdnoœæ i tempo, tym lepsza zap³ata!";
                break;
            case 2:
                tutorialText.text = "Dobra robota! Klient zostawi³ gotówkê na ladzie.\n\nKliknij w monety, aby odebraæ zap³atê.";
                break;
            case 3:
                tutorialText.text = "Do pisania zu¿ywasz papier i tusz. Gdy siê skoñcz¹, musisz je dokupiæ.\n\nKliknij na rega³ w tle, aby otworzyæ sklep.";
                break;
            case 4:
                tutorialText.text = "Ostatnia zasada: co 10 klientów musisz zap³aciæ 3000 z³ota d³ugu, inaczej przegrasz grê!\n\nPowodzenia!";
                gotItButton.gameObject.SetActive(true);
                break;
            default:
                EndTutorial();
                return;
        }

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        if (gameObject.activeInHierarchy) animationCoroutine = StartCoroutine(AnimateBubblePop());
    }

    IEnumerator AnimateBubblePop()
    {
        if (bubbleRect == null) yield break;

        float elapsed = 0f;
        bubbleRect.localScale = Vector3.zero;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;

            float scale = Mathf.Clamp01(t);
            scale = Mathf.Sin(scale * Mathf.PI * 0.5f) + Mathf.Sin(scale * Mathf.PI) * 0.15f;

            bubbleRect.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        bubbleRect.localScale = Vector3.one;
    }

    public void OnGotItClicked()
    {
        AdvanceTutorial(currentStep);
    }

    public void EndTutorial()
    {
        isTutorialActive = false;
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt("TutorialDone", 1);
    }
}