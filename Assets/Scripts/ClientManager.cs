using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ClientManager : MonoBehaviour
{
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
    public string[] humanDialogues;
    public string[] monsterDialogues;

    [Header("--- HUMAN FEEDBACK (Ludzie) ---")]
    public string[] humanGood;
    public string[] humanMedium;
    public string[] humanBad;

    [Header("--- MONSTER FEEDBACK (Potwory) ---")]
    public string[] monsterGood;
    public string[] monsterMedium;
    public string[] monsterBad;

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

    public static string TextToCopy = ""; 
    public static int WonCoins = 0;
    public static int TotalGold = 0;
    public static bool ReturningFromGame = false;
    public static int PaperCount = 3;
    public static int InkCount = 3;
    public static int CurrentInkUses = 2; 

    private CharacterProfile currentProfile; 
    private bool czyToPotwor = false;

    private string[] humanSentences = {
        "Jest",
        "czy",
        // "Mikołaj Kopernik udowodnił, że to Ziemia krąży wokół Słońca. To odkrycie zburzyło dawne wyobrażenie o miejscu człowieka we wszechświecie. Wcześniej wierzono, że nasza planeta stanowi nieruchome centrum kosmosu. Dzięki badaniom astronoma narodziła się nowoczesna nauka oparta na obserwacji. Ta rewolucja zmieniła nie tylko astronomię, ale i całą filozofię. Dziś wiemy, że jesteśmy częścią ogromnego układu planetarnego.",
        // "Maria Skłodowska-Curie odkryła nowe pierwiastki i zbadała niewidzialne promieniowanie. Jej prace naukowe otworzyły drzwi do wnętrza atomu. Dzięki tej wiedzy rozwinęła się nowoczesna diagnostyka oraz leczenie nowotworów. Uczona udowodniła, że atomy nie są niepodzielne, co zrewolucjonizowało chemię. Maria pozostaje jedyną kobietą z dwiema Nagrodami Nobla w różnych dziedzinach. Jej odkrycie zmieniło oblicze współczesnej medycyny i energetyki.",
        // "Charles Darwin odkrył, że wszystkie gatunki pochodzą od wspólnych przodków. Proces doboru naturalnego sprawia, że przeżywają osobniki najlepiej przystosowane. To odkrycie na zawsze zmieniło sposób, w jaki patrzymy na przyrodę. Ludzie zrozumieli, że życie na planecie zmienia się od milionów lat. Teoria ta wyjaśniła niesamowitą różnorodność zwierząt oraz roślin. Do dziś stanowi ona najważniejszy filar współczesnej biologii.",
        // "Isaac Newton sformułował prawo wyjaśniające, dlaczego przedmioty spadają na ziemię. Zrozumiał, że ta sama siła steruje ruchem jabłka oraz planety. Był to pierwszy raz, gdy matematycznie opisano uniwersalne reguły natury. Dzięki grawitacji naukowcy mogli zacząć planować loty w kosmos. Odkrycie to stało się fundamentem fizyki na kolejne stulecia. Newton pokazał nam, że wszechświat działa niczym precyzyjny mechanizm.",
        // "Louis Pasteur odkrył, że za choroby odpowiadają niewidoczne gołym okiem drobnoustroje. Zrozumienie roli bakterii pozwoliło na wprowadzenie zasad higieny. Przed tym odkryciem lekarze często nieświadomie przenosili groźne infekcje. Dzięki dowodom naukowym opracowano pierwsze szczepionki oraz pasteryzację żywności. Śmiertelność ludzi spadła, a medycyna stała się prawdziwą nauką. To odkrycie uratowało miliardy istnień ludzkich w historii świata.",
        // "Naukowcy odkryli, że nasz wszechświat ma swój początek i stale się rozszerza. Teoria Wielkiego Wybuchu opisuje moment powstania materii oraz czasu. Dowodem na to było odkrycie promieniowania, które wypełnia cały kosmos. Dzięki temu wiemy, że miliardy lat temu wszystko było skupione w jednym punkcie. Astronomia przestała być tylko opisem gwiazd, a stała się badaniem historii bytu. To odkrycie pozwala nam oszacować wiek całego świata.",
        // "William Harvey jako pierwszy opisał naukowo, jak krew krąży w ciele. Odkrył on, że serce pełni funkcję pompy tłoczącej płyn przez naczynia. Wcześniej sądzono, że krew jest stale zużywana przez ludzki organizm. To odkrycie pozwoliło zrozumieć, jak tlen dociera do wszystkich tkanek. Stało się ono fundamentem dla nowoczesnej fizjologii oraz chirurgii. Bez tej wiedzy niemożliwe byłyby żadne operacje czy ratujące życie transfuzje.",
        // "Grzegorz Mendel odkrył reguły przekazywania cech z pokolenia na pokolenie. Badając rośliny grochu, zauważył powtarzalne schematy w ich wyglądzie. Choć za życia nie został doceniony, jego prace stworzyły podstawy genetyki. Zrozumieliśmy, że dziedziczenie nie jest dziełem przypadku, lecz praw natury. Dzięki temu odkryciu nauka może dziś badać geny i leczyć wady wrodzone. Mendel pokazał, że klucz do tajemnicy życia kryje się w matematyce.",
        // "Michael Faraday odkrył zjawisko indukcji, badając ruch magnesu w cewce. To spostrzeżenie pozwoliło zamienić ruch mechaniczny w energię elektryczną. Bez tego odkrycia nigdy nie powstałyby prądnice zasilające nasze domy. Uczony udowodnił, że magnetyzm i elektryczność to dwie strony tej samej siły. Jego badania położyły fundamenty pod całą nowoczesną fizykę pola. Faraday udowodnił, że czysta nauka może całkowicie odmienić cywilizację.",
        // "Odkrycie struktury podwójnej helisy pozwoliło nam zrozumieć kod życia. Naukowcy wyjaśnili w ten sposób mechanizm dziedziczenia cech po przodkach. Dzięki temu wiemy, dlaczego mamy określony kolor oczu lub talent. Ta wiedza zrewolucjonizowała biologię, kryminalistykę oraz medycynę. Możemy teraz diagnozować choroby genetyczne i szukać na nie leków. Badanie DNA pozwala nam również lepiej poznać historię całej ludzkości.",
        // "Antoine Lavoisier odkrył, że powietrze nie jest jednolitym gazem. Zrozumiał on, że za spalanie i nasze oddychanie odpowiada tlen. To odkrycie położyło kres dawnym mitom na temat natury ognia. Dzięki niemu chemia stała się nowoczesną nauką opartą na faktach. Uczony udowodnił, że materia w przyrodzie nigdy nie ginie. To absolutny fundament wiedzy o tym, jak funkcjonuje życie.",
        // "Isaac Newton odkrył uniwersalne zasady, które rządzą ruchem wszystkich ciał. Opisał on matematycznie, jak siła wpływa na rozpędzanie i hamowanie przedmiotów. Dzięki tym odkryciom wiemy, jak bezpiecznie budować mosty i szybkie pojazdy. Był to pierwszy raz, gdy człowiek zrozumiał mechaniczne zasady wszechświata. Te proste równania pozwoliły nam później wysłać ludzi na Księżyc. Każdy Twój ruch podlega tym samym, niezmiennym prawom fizyki.",
        // "Naukowcy odkryli, że rośliny potrafią zamieniać światło słoneczne w energię. To niesamowity proces, który pozwala zielonym liściom produkować tlen i pokarm. Dzięki temu odkryciu zrozumieliśmy, że całe życie na Ziemi zależy od słońca. Rośliny są wielkimi fabrykami, które oczyszczają naszą atmosferę z węgla. Bez fotosyntezy nasza planeta byłaby tylko jałową, martwą pustynią. To najważniejsza reakcja chemiczna, jaka zachodzi w przyrodzie.",
        // "Odkrycie, że światło porusza się ze stałą prędkością, zszokowało świat. Okazało się, że patrząc w gwiazdy, patrzymy w daleką przeszłość. Światło potrzebuje czasu, aby pokonać ogromne odległości w kosmosie. To odkrycie pozwoliło nam zmierzyć wielkość wszechświata i zrozumieć jego skalę. Jest to najwyższa możliwa prędkość, jakiej nic nie może przekroczyć. Dzięki tej wiedzy rozwinęła się nowoczesna optyka oraz radioastronomia.",
        // "Odkrycie gazów, które pozwalają wyłączyć ból podczas operacji, było przełomem. Wcześniej pacjenci musieli cierpieć wielkie katusze w rękach chirurgów. Dzięki badaniom nad eterem i chloroformem medycyna stała się bardziej ludzka. Lekarze mogli wreszcie przeprowadzać długie i bardzo precyzyjne zabiegi. To odkrycie naukowe uratowało miliony ludzi przed traumą i strachem. Dziś nikt nie wyobraża sobie wizyty u dentysty bez znieczulenia.",
        // "Naukowcy odkryli, że wszystko wokół nas składa się z malutkich atomów. Zrozumienie, że w środku każdego z nich jest jądro, zmieniło fizykę. Okazało się, że materia jest w większości pustą przestrzenią pełną energii. To odkrycie doprowadziło do powstania nowoczesnej elektroniki i komputerów. Wiedza o atomach pozwala nam tworzyć nowe materiały o niesamowitych cechach. To najmniejszy klocek, z którego zbudowany jest cały nasz świat.",
        // "Odkrycie istnienia niewidzialnych drobnoustrojów zmieniło zasady higieny. Naukowcy udowodnili, że to małe organizmy powodują groźne choroby. Dzięki temu wiemy, dlaczego mycie rąk jest tak bardzo ważne dla zdrowia. To odkrycie pozwoliło stworzyć szczepionki, które chronią nas wszystkich. Zrozumieliśmy, że wokół nas tętni niewidoczne życie, które ma na nas wpływ. Medycyna stała się dzięki temu precyzyjną walką z konkretnym wrogiem.",
        // "Odkrycie, że nasza planeta zachowuje się jak wielki magnes, było kluczowe. To niewidzialne pole chroni nas przed promieniowaniem z kosmosu. Dzięki niemu działają kompasy, które od wieków prowadziły podróżników do celu. Naukowcy zrozumieli, że płynne żelazo wewnątrz Ziemi tworzy tę tarczę. Bez pola magnetycznego atmosfera naszej planety zostałaby szybko zniszczona. To ciche i niewidoczne odkrycie pozwala nam bezpiecznie żyć.",
        // "Edwin Hubble odkrył, że wszystkie galaktyki uciekają od siebie z dużą prędkością. Oznacza to, że wszechświat nie jest stały, lecz stale rośnie. To odkrycie doprowadziło naukowców do teorii o samym początku świata. Dowiedzieliśmy się, że kosmos ma swoją fascynującą i długą historię. Ta wiedza całkowicie zmieniła nasze miejsce w ogromnej przestrzeni. Każdej sekundy granice wszechświata przesuwają się coraz dalej od nas.",
        // "Odkrycie, że wszystkie żywe istoty są zbudowane z komórek, zjednoczyło biologię. Zrozumieliśmy, że komórka to podstawowa jednostka życia, która potrafi się dzielić. Każda roślina, zwierzę i człowiek zaczynają się od jednej, małej cząstki. To odkrycie pozwoliło nam badać, jak rośniemy i jak regeneruje się ciało. Dzięki mikroskopom poznaliśmy tajemnice ukryte wcześniej przed naszym okiem. Komórka to miniaturowy świat, który nieustannie tętni życiem.",
        // "Albert Einstein odkrył, że czas i przestrzeń są ze sobą ściśle połączone. Udowodnił, że czas może płynąć wolniej, jeśli poruszamy się bardzo szybko. To odkrycie zmieniło nasze myślenie o grawitacji i o całym wszechświecie. Dzięki jego równaniom możemy dziś korzystać z bardzo precyzyjnej nawigacji. Choć teoria ta brzmi skomplikowanie, jej skutki widzimy w codziennej technologii. Einstein pokazał nam, że rzeczywistość jest o wiele ciekawsza, niż sądziliśmy.",
        // "Naukowcy odkryli, że kontynenty na Ziemi nie stoją w miejscu, lecz powoli płyną. Zrozumienie ruchu płyt tektonicznych wyjaśniło powstawanie gór oraz trzęsień ziemi. Dowiedzieliśmy się, że miliony lat temu istniał tylko jeden wielki ląd. To odkrycie pozwoliło nam zrozumieć, jak zmieniał się klimat i życie na planecie. Ziemia pod naszymi stopami jest dynamiczna i stale się przekształca. Dzięki tej wiedzy lepiej rozumiemy potęgę sił drzemiących wewnątrz globusa.",
        // "Odkrycie praw rządzących ruchem powietrza pozwoliło nam przewidywać pogodę. Naukowcy zrozumieli, jak różnice temperatur tworzą wiatry i potężne cyklony. Dzięki tej wiedzy żeglarze mogli bezpieczniej przemierzać oceany świata. Zrozumienie atmosfery jest kluczowe dla ochrony klimatu i naszego rolnictwa. To odkrycie pozwala nam przygotować się na nadchodzące burze i upały. Powietrze nad naszymi głowami to skomplikowany system, który daje nam życie.",
        // "Odkrycie fizycznych właściwości krzemu pozwoliło na stworzenie nowoczesnej elektroniki. Naukowcy zrozumieli, jak kontrolować przepływ prądu wewnątrz bardzo małych struktur. Dzięki temu odkryciu powstały procesory, które napędzają Twojego laptopa i telefon. Bez półprzewodników nie byłoby Internetu ani żadnych inteligentnych maszyn. To naukowe odkrycie stało się fundamentem obecnej ery cyfrowej. Każdy mikroskopijny układ scalony korzysta z tych samych praw fizyki.",
        // "Odkrycie echa Wielkiego Wybuchu potwierdziło teorię o narodzinach wszechświata. Naukowcy zarejestrowali słaby szum, który dociera do nas z każdego kierunku nieba. Jest to najstarsze światło, jakie możemy zaobserwować w całym kosmosie. To odkrycie pozwoliło nam zajrzeć w przeszłość o miliardy lat wstecz. Dzięki niemu wiemy, że wszechświat narodził się w jednej, gorącej chwili. To ostateczny dowód na to, jak niezwykła jest historia naszego bytu."
    };

    IEnumerator Start()
    {
        if (machineButton != null) machineButton.interactable = false;

        if (clientWhole != null)
        {
            startPosition = clientWhole.transform.localPosition;
            hiddenPosition = new Vector3(startPosition.x, startPosition.y - 800f, startPosition.z);
            clientWhole.transform.localPosition = hiddenPosition;
            
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
        }
        else
        {
            SetupCharacter(potwory, false);
        }

        if (clientWhole != null) StartCoroutine(AnimateEntry());
    }

    void SetupCharacter(List<CharacterProfile> lista, bool isHuman)
    {
        czyToPotwor = !isHuman; 

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

    IEnumerator AnimateEntry()
    {
        if (clientWhole == null) yield break;

        if (machineButton != null) machineButton.interactable = false;

        TypingGameManager game = FindFirstObjectByType<TypingGameManager>();
        if (game != null) game.ResetTimerDisplay();

        clientWhole.SetActive(true);
        ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
        if (visuals != null) visuals.HideBubble();

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

        yield return new WaitForSeconds(1.0f);

        if (visuals != null)
        {
            string randomText = "";
            
            if (czyToPotwor)
            {
                if (monsterDialogues != null && monsterDialogues.Length > 0)
                    randomText = monsterDialogues[Random.Range(0, monsterDialogues.Length)];
            }
            else
            {
                if (humanDialogues != null && humanDialogues.Length > 0)
                    randomText = humanDialogues[Random.Range(0, humanDialogues.Length)];
            }

            visuals.ShowBubble(randomText);
        }

        if (machineButton != null) machineButton.interactable = true;
    }

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

        ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
        if (visuals != null) visuals.HideBubble();

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

        GameLoopManager loop = FindFirstObjectByType<GameLoopManager>();
        if (loop != null) loop.EndRound();
    }

    void ClientLeaves()
    {
        if (machineButton != null) machineButton.interactable = false;
        if (clientWhole != null) StartCoroutine(AnimateExit());
    }

    IEnumerator AnimateExit()
    {
        ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
        if (visuals != null) visuals.HideBubble();

        if (clientWhole == null) yield break;
        
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

    void UpdateClientReaction(int money)
    {
        string reactionText = "";

        if (czyToPotwor)
        {
            if (money >= 500)
            {
                if (monsterGood != null && monsterGood.Length > 0) reactionText = monsterGood[Random.Range(0, monsterGood.Length)];
            }
            else if (money >= 350)
            {
                if (monsterMedium != null && monsterMedium.Length > 0) reactionText = monsterMedium[Random.Range(0, monsterMedium.Length)];
            }
            else
            {
                if (monsterBad != null && monsterBad.Length > 0) reactionText = monsterBad[Random.Range(0, monsterBad.Length)];
            }
        }
        else
        {
            if (money >= 500)
            {
                if (humanGood != null && humanGood.Length > 0) reactionText = humanGood[Random.Range(0, humanGood.Length)];
            }
            else if (money >= 350)
            {
                if (humanMedium != null && humanMedium.Length > 0) reactionText = humanMedium[Random.Range(0, humanMedium.Length)];
            }
            else
            {
                if (humanBad != null && humanBad.Length > 0) reactionText = humanBad[Random.Range(0, humanBad.Length)];
            }
        }

        if (clientWhole != null)
        {
            ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
            if (visuals != null && !string.IsNullOrEmpty(reactionText)) visuals.ShowBubble(reactionText);
        }
    }

    void UpdateFaceEmotion()
    {
        if (currentProfile == null) return;
        Sprite targetSprite = currentProfile.neutralny; 

        if (WonCoins >= 500) targetSprite = currentProfile.szczesliwy;
        else if (WonCoins >= 350) targetSprite = currentProfile.neutralny;
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
            if (WonCoins >= 500) { cashImage.sprite = coinsLarge; cashButton.transform.localScale = Vector3.one; }
            else if (WonCoins >= 350) { cashImage.sprite = coinsMedium; cashButton.transform.localScale = new Vector3(0.8f, 0.8f, 1f); }
            else { cashImage.sprite = coinsSmall; cashButton.transform.localScale = new Vector3(0.6f, 0.6f, 1f); }
            cashButton.SetActive(true);
        }
        else
        {
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
        if (CurrentInkUses <= 0 && InkCount > 0)
        {
            InkCount--;
            CurrentInkUses = 2; 
        }

        if (PaperCount > 0 && CurrentInkUses > 0)
        {
            PaperCount--;
            CurrentInkUses--;

            PaperController paper = FindFirstObjectByType<PaperController>();
            if (paper != null) paper.SlideIn(TextToCopy);

            if (machineButton != null) machineButton.interactable = false;

            ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
            if (visuals != null) visuals.HideBubble();
        }
        else
        {
            ClientVisuals visuals = clientWhole.GetComponent<ClientVisuals>();
            if (visuals != null) visuals.ShowBubble("Brak papieru lub tuszu!\nOdwiedź sklep.");
        }
    }
}