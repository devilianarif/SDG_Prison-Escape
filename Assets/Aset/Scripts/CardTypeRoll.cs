using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardTypeRoll : MonoBehaviour
{
    public TMP_Text cardTypeText;
    public Button rolltypecard;
    public CardTypeRollHistory history;
    public Button historyButton;

    public GameManager gameManager;

    public string lastType;

    // daftar tipe kartu
    private string[] cardTypes = { "Bad", "Lucky", "Skill" };

    void Start()
    {
        rolltypecard.onClick.AddListener(RandomType);
        historyButton.onClick.AddListener(OpenHistory);

        historyButton.gameObject.SetActive(true);
        cardTypeText.text = "";
    }

    void OpenHistory()
    {
        if (history != null)
            history.OpenPanel();
    }

    void RandomType()
    {
        int index = Random.Range(0, cardTypes.Length);
        string type = cardTypes[index];

        cardTypeText.text = type;
        cardTypeText.color = GetColorByType(type);

        Debug.Log("Kartu terpilih: " + type);

        if (history != null)
            history.AddHistory(type);

        lastType = type;

        gameManager.playerState.SetTypeCard(lastType);
        gameManager.SaveState();
        gameManager.UpdateChecklist();
    }

    Color GetColorByType(string type)
    {
        switch (type)
        {
            case "Bad":
                return HexToColor("AE4E4E");

            case "Lucky":
                return HexToColor("4AAF6A");

            case "Skill":
                return HexToColor("4C7BAE");

            default:
                return Color.white;
        }
    }

    Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#" + hex, out color);
        return color;
    }
}
