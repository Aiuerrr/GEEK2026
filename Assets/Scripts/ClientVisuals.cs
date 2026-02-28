using UnityEngine;
using TMPro;

public class ClientVisuals : MonoBehaviour
{
    public GameObject bubbleObject;
    public TMP_Text bubbleText;

    public void ShowBubble(string textToSay)
    {
        if (bubbleObject != null)
        {
            if (bubbleText != null) bubbleText.text = textToSay;
            bubbleObject.SetActive(true);
        }
    }

    public void HideBubble()
    {
        if (bubbleObject != null) bubbleObject.SetActive(false);
    }
}