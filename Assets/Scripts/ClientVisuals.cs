using UnityEngine;
using TMPro; // <--- 1. ZMIANA: Dodajemy bibliotekę do TextMeshPro

public class ClientVisuals : MonoBehaviour
{
    [Header("To podepnij w Inspectorze")]
    public GameObject bubbleObject; 
    public TMP_Text bubbleText; // <--- 2. ZMIANA: Typ zmiennej na uniwersalny TMP

    // Funkcja do pokazania dymka z konkretnym tekstem
    public void ShowBubble(string textToSay)
    {
        if (bubbleObject != null)
        {
            if (bubbleText != null)
            {
                bubbleText.text = textToSay;
            }

            bubbleObject.SetActive(true);
        }
    }

    public void HideBubble()
    {
        if (bubbleObject != null)
        {
            bubbleObject.SetActive(false);
        }
    }
}