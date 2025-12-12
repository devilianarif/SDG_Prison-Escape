using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardTypeRollHistory : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text texthistory;
    public Button backButton;
    public Canvas canvasUI;

    float normalDistance = 1f;
    float historyDistance = 1f;

    List<string> list = new List<string>();

    void Start()
    {
        panel.SetActive(false);

        if (backButton != null)
            backButton.onClick.AddListener(ClosePanel);
    }

    public void AddHistory(string cardName)
    {
        list.Insert(0, cardName);

        if (panel.activeSelf)
            UpdateUI();
    }

    void UpdateUI()
    {
        string s = "";
        for (int i = 0; i < list.Count; i++)
            s += list[i] + "\n";

        texthistory.text = s;
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
        BringToFront();
        UpdateUI();
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        SendBack();
    }

    public void SetHistoryActive(bool active)
    {
        if (!active)
            panel.SetActive(false);
    }

    void BringToFront()
    {
        if (canvasUI != null)
            canvasUI.planeDistance = historyDistance;
    }

    void SendBack()
    {
        if (canvasUI != null)
            canvasUI.planeDistance = normalDistance;
    }
}
