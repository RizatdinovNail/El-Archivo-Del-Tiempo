using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class dialogueHistory : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI historyText;


    [HideInInspector] public List<string> historyList = new List<string>();
    private uiManager UM;
    void Start()
    {
        UM = GetComponent<gameCore>().UM;
    }

    public void AddToHistory(string speaker, string text)
    {
        string formatted = $"<b>{speaker}:</b> {text}";
        historyList.Add(formatted);
        RefreshHistoryText();
    }

    private void RefreshHistoryText()
    {
        historyText.text = "";

        foreach (var line in historyList)
        {
            historyText.text += line + "\n\n";
        }

        // Scroll to the bottom automatically
        Canvas.ForceUpdateCanvases();
    }
}
