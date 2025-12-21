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
    [Header("Feature Toggle")]
    public bool useHistory = false;


    // daftar tipe kartu
    private string[] cardTypes = { "Bad", "Lucky", "Skill" };

    void Start()
    {
        rolltypecard.onClick.AddListener(RandomType);

        if (useHistory)
        {
            historyButton.onClick.AddListener(OpenHistory);
            historyButton.gameObject.SetActive(true);
        }
        else
        {
            historyButton.gameObject.SetActive(false);
        }

        cardTypeText.text = "";
    }


    void OpenHistory()
    {
        if (history != null)
            history.OpenPanel();
    }

    void RandomType()
    {
        int curr = gameManager.playerState.currentPlayerIndex;
        var player = gameManager.playerState.players[curr];

        // =========================
        // FASE 2 → ROLL WAJIB
        // =========================
        if (!gameManager.isRerollingCardType)
        {
            DoRoll();
            gameManager.playerState.SetTypeCard(lastType);
            gameManager.SaveState();
            gameManager.UpdateChecklist();
            rolltypecard.interactable = false; // cuma boleh 1x
            return;
        }

        // =========================
        // REROLL
        // =========================
        if (player.rerollChanceLeft <= 0)
        {
            rolltypecard.interactable = false;
            return;
        }

        player.rerollChanceLeft--;
        DoRoll();

        gameManager.SetLatestRerolledCardType(lastType);

        if (player.rerollChanceLeft <= 0)
            rolltypecard.interactable = false;
    }

    void DoRoll()
    {
        int index = Random.Range(0, cardTypes.Length);
        lastType = cardTypes[index];

        cardTypeText.text = lastType;
        cardTypeText.color = GetColorByType(lastType);
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
