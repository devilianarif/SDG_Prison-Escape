using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceHistory : MonoBehaviour
{
public GameObject panel;
public TMP_Text text;
public Button toggleButton;
public Canvas canvasUI;

float normalDistance = 30f;
float historyDistance = 1f;

public Button BackButton; 
List<int> list = new List<int>();

void Start()
{
    panel.SetActive(false); 
    toggleButton.gameObject.SetActive(false);

    if (toggleButton != null)
        toggleButton.onClick.AddListener(Toggle);

        if (BackButton != null)
BackButton.onClick.AddListener(ClosePanel);
}
void ClosePanel()
{
panel.SetActive(false);
SendBack();
}
public void Toggle()
{
if (!toggleButton.gameObject.activeSelf)
return;

bool show = !panel.activeSelf;
panel.SetActive(show);

if (show)
{
    BringToFront();
    UpdateUI();
}
else
{
    SendBack();
}


}

public void Add(int value)
{
    list.Insert(0, value);
}

void UpdateUI()
{
    string s = "";
    for (int i = 0; i < list.Count; i++)
        s += list[i] + "\n";

    text.text = s;
}

public void SetHistoryActive(bool active)
{
    toggleButton.gameObject.SetActive(active);

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