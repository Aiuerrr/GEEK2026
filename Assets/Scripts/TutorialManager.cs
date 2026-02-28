using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public Button gotItButton;

    private int currentStep = 0;
    private bool isTutorialActive = false;

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
                break;
        }
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

    // Odkomentuj to do testów, jeœli chcesz zresetowaæ samouczek
    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerPrefs.SetInt("TutorialDone", 0);
            StartTutorial();
        }
    }
    */
}