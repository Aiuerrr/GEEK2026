using UnityEngine;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("--- UI PANELS ---")]
    public GameObject shopPanel;
    public RectTransform shopPanelRect;
    public GameObject elementsToOff;

    [Header("--- ANIMATION (PRZEJAZD) ---")]
    public float transitionTime = 0.4f;
    public Vector2 onScreenPosition = new Vector2(0, 0);
    public Vector2 offScreenPosition = new Vector2(1920, 0);

    [Header("--- PRICES ---")]
    public int singlePaperPrice = 25;   // 1 papier
    public int bundlePaperPrice = 60;   // 3 papiery
    public int singleInkPrice = 75;     // 1 tusz
    public int bundleInkPrice = 200;    // 3 tusze

    [Header("--- UI TEXTS (W sklepie) ---")]
    public TextMeshProUGUI paperCountText;
    public TextMeshProUGUI inkCountText;
    public TextMeshProUGUI goldText;

    void Start()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        UpdateShopUI();
    }

    // --- OTWIERANIE SKLEPU ---
    // --- OTWIERANIE SKLEPU ---
    public void OpenShop()
    {
        // ZABEZPIECZENIE: Nie otwieraj sklepu, jeœli gra jest zapauzowana lub trwa odliczanie
        if (Time.timeScale == 0f) return;

        // Sprawdzamy, czy gracz w³aœnie nie przepisuje tekstu (to ju¿ mia³eœ zrobione wczeœniej)
        TypingGameManager typingManager = FindFirstObjectByType<TypingGameManager>();
        if (typingManager != null && typingManager.isGameActive)
        {
            Debug.Log("Nie mo¿na otworzyæ sklepu w trakcie przepisywania tekstu!");
            return;
        }

        if (shopPanel != null)
        {
            if (shopPanelRect != null) shopPanelRect.anchoredPosition = offScreenPosition;

            shopPanel.SetActive(true);
            UpdateShopUI();

            if (elementsToOff != null) elementsToOff.SetActive(false);

            Time.timeScale = 0f; // Zatrzymujemy grê dla sklepu

            StopAllCoroutines();
            StartCoroutine(SlideInShop());
        }
    }

    // --- ZAMYKANIE SKLEPU ---
    public void CloseShop()
    {
        if (shopPanel != null)
        {
            if (elementsToOff != null) elementsToOff.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(SlideOutShop());
        }
    }

    // --- ANIMACJE ---
    IEnumerator SlideInShop()
    {
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float percent = elapsed / transitionTime;
            float t = Mathf.SmoothStep(0f, 1f, percent);

            if (shopPanelRect != null)
            {
                shopPanelRect.anchoredPosition = Vector2.Lerp(offScreenPosition, onScreenPosition, t);
            }
            yield return null;
        }
        if (shopPanelRect != null) shopPanelRect.anchoredPosition = onScreenPosition;
    }

    IEnumerator SlideOutShop()
    {
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float percent = elapsed / transitionTime;
            float t = Mathf.SmoothStep(0f, 1f, percent);

            if (shopPanelRect != null)
            {
                shopPanelRect.anchoredPosition = Vector2.Lerp(onScreenPosition, offScreenPosition, t);
            }
            yield return null;
        }
        if (shopPanelRect != null) shopPanelRect.anchoredPosition = offScreenPosition;

        shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // ==========================================
    // --- ZAKUPY (Osobne funkcje pod przyciski) ---
    // ==========================================

    // 1 PAPIER za 25
    public void BuySinglePaper()
    {
        if (ClientManager.TotalGold >= singlePaperPrice)
        {
            ClientManager.TotalGold -= singlePaperPrice;
            ClientManager.PaperCount += 1;
            UpdateShopUI();
        }
        else Debug.Log("Brak kasy na 1 papier!");
    }

    // 3 PAPIERY za 60
    public void BuyBundlePaper()
    {
        if (ClientManager.TotalGold >= bundlePaperPrice)
        {
            ClientManager.TotalGold -= bundlePaperPrice;
            ClientManager.PaperCount += 3;
            UpdateShopUI();
        }
        else Debug.Log("Brak kasy na 3 papiery!");
    }

    // 1 TUSZ za 75
    public void BuySingleInk()
    {
        if (ClientManager.TotalGold >= singleInkPrice)
        {
            ClientManager.TotalGold -= singleInkPrice;
            ClientManager.InkCount += 1;
            UpdateShopUI();
        }
        else Debug.Log("Brak kasy na 1 tusz!");
    }

    // 3 TUSZE za 200
    public void BuyBundleInk()
    {
        if (ClientManager.TotalGold >= bundleInkPrice)
        {
            ClientManager.TotalGold -= bundleInkPrice;
            ClientManager.InkCount += 3;
            UpdateShopUI();
        }
        else Debug.Log("Brak kasy na 3 tusze!");
    }

    // --- AKTUALIZACJA UI ---
    public void UpdateShopUI()
    {
        if (paperCountText != null) paperCountText.text = ClientManager.PaperCount.ToString();
        if (inkCountText != null) inkCountText.text = ClientManager.InkCount.ToString();
        if (goldText != null) goldText.text = ClientManager.TotalGold.ToString();
    }
}