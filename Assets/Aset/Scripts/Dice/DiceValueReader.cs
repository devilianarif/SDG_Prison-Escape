using UnityEngine;
using TMPro;

public class DiceValueReader : MonoBehaviour
{
public TextMeshProUGUI resultText;
public DiceHistory history;
public GameManager gameManager; 

public bool useHistory = false;

bool hasRecorded;
int currentValue;
public bool isPoliceDice = false;
void Start()
{
    if (history != null)
        history.SetHistoryActive(useHistory);
}

public void SetValue(int value)
{
    if (hasRecorded) return;


    currentValue = value;
    resultText.text = currentValue.ToString();
    Debug.Log("Nilai dadu terbaca: " + currentValue);
    if (!isPoliceDice && gameManager != null)
    {
    gameManager.playerState.SetDiceResult(currentValue);
    gameManager.SaveState();
    }                        

    if (useHistory && history != null)
        history.Add(currentValue);
    
    hasRecorded = true;
}

public void ResetRecord()
{
    hasRecorded = false;
    currentValue = 0;
    resultText.text = "0";
}

public void ForceValue(int v)
{
    currentValue = v;
    resultText.text = v.ToString();
}
public void SetHistoryFeature(bool active)
{
useHistory = active;

if (history != null)
    history.SetHistoryActive(active);


}


public int GetValue()
{
return currentValue;
}
}