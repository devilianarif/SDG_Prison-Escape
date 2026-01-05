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
        hasRecorded = true;
        

        Debug.Log("Nilai dadu mentah terbaca: " + value);
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

    public void ShowResult(int rawValue, int buffValue)
    {
        if (buffValue > 0)
        {
            int total = rawValue + buffValue;
            resultText.text = $"{rawValue} + {buffValue} (buff effect) = {total}";
            Debug.Log("Final dice value with buff: " + total);
            currentValue = total;
        }
        else
        {
            resultText.text = rawValue.ToString();
            currentValue = rawValue;
        }
    }

}