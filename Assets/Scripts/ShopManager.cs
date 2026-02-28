using UnityEngine;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject shopPanel;
    public RectTransform shopPanelRect;
    public GameObject elementsToOff;

    [Header("Animation")]
    public float transitionTime = 0.4f;
    public Vector2 onScreenPosition = new Vector2(0, 0);
    public Vector2 offScreenPosition = new Vector2(1920, 0);

    [Header("Prices")]
    public int singlePaperPrice = 25;
    public int bundlePaperPrice = 60;
    public int singleInkPrice = 75;
    public int bundleInkPrice = 200;

    [Header("Texts")]
    public TextMeshProUGUI paperCountText;
    public TextMeshProUGUI inkCountText;
    public TextMeshProUGUI goldText;

    void Start()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        UpdateShopUI();
    }

    public void OpenShop()
    {
        if (PauseMenu.isCountingDown) return;

        TypingGameManager typingManager = FindFirstObjectByType<TypingGameManager>();
        if (typingManager != null && typingManager.isGameActive) return;

        if (TutorialManager.Instance != null) TutorialManager.Instance.AdvanceTutorial(3);

        if (AudioManager.Instance != null && AudioManager.Instance.sfxEasterEgg != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxButtonClicked);
        }

        if (shopPanel != null)
        {
            if (shopPanelRect != null) shopPanelRect.anchoredPosition = offScreenPosition;

            shopPanel.SetActive(true);
            UpdateShopUI();

            if (elementsToOff != null) elementsToOff.SetActive(false);

            Time.timeScale = 0f;

            StopAllCoroutines();
            StartCoroutine(SlideInShop());
        }
    }

    public void CloseShop()
    {
        if (PauseMenu.isCountingDown) return;

        if (AudioManager.Instance != null && AudioManager.Instance.sfxEasterEgg != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxButtonClicked);
        }

        if (shopPanel != null)
        {
            if (elementsToOff != null) elementsToOff.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(SlideOutShop());
        }
    }

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

    public void BuySinglePaper()
    {
        if (PauseMenu.isCountingDown) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxShopBuy);

        if (ClientManager.TotalGold >= singlePaperPrice)
        {
            ClientManager.TotalGold -= singlePaperPrice;
            ClientManager.PaperCount += 1;
            UpdateShopUI();
        }
    }

    public void BuyBundlePaper()
    {
        if (PauseMenu.isCountingDown) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxShopBuy);

        if (ClientManager.TotalGold >= bundlePaperPrice)
        {
            ClientManager.TotalGold -= bundlePaperPrice;
            ClientManager.PaperCount += 3;
            UpdateShopUI();
        }
    }

    public void BuySingleInk()
    {
        if (PauseMenu.isCountingDown) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxShopBuy);

        if (ClientManager.TotalGold >= singleInkPrice)
        {
            ClientManager.TotalGold -= singleInkPrice;
            ClientManager.InkCount += 1;
            UpdateShopUI();
        }
    }

    public void BuyBundleInk()
    {
        if (PauseMenu.isCountingDown) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxShopBuy);

        if (ClientManager.TotalGold >= bundleInkPrice)
        {
            ClientManager.TotalGold -= bundleInkPrice;
            ClientManager.InkCount += 3;
            UpdateShopUI();
        }
    }

    public void UpdateShopUI()
    {
        if (paperCountText != null) paperCountText.text = ClientManager.PaperCount.ToString();
        if (inkCountText != null) inkCountText.text = ClientManager.InkCount.ToString();
        if (goldText != null) goldText.text = ClientManager.TotalGold.ToString();
    }
}