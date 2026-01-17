using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardTypeRoll : MonoBehaviour
{
    [Header("Probability (Auto Balance)")]
    [Range(0, 100)]
    public int badPercent = 70;

    [Range(0, 100)]
    public int luckyPercent = 20;

    [Range(0, 100)]
    public int skillPercent = 10;
    int lastBad;
    int lastLucky;
    int lastSkill;

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

    void OnValidate()
    {
        badPercent = Mathf.Clamp(badPercent, 0, 100);
        luckyPercent = Mathf.Clamp(luckyPercent, 0, 100);
        skillPercent = Mathf.Clamp(skillPercent, 0, 100);

        int changed = -1;
        if (badPercent != lastBad)
            changed = 0;
        else if (luckyPercent != lastLucky)
            changed = 1;
        else if (skillPercent != lastSkill)
            changed = 2;

        int total = badPercent + luckyPercent + skillPercent;

        if (total != 100)
        {
            int delta = total - 100;

            if (changed == 0)
            {
                Adjust(ref luckyPercent, ref skillPercent, delta);
            }
            else if (changed == 1)
            {
                Adjust(ref badPercent, ref skillPercent, delta);
            }
            else if (changed == 2)
            {
                Adjust(ref badPercent, ref luckyPercent, delta);
            }
        }

        lastBad = badPercent;
        lastLucky = luckyPercent;
        lastSkill = skillPercent;
    }

    void Adjust(ref int a, ref int b, int delta)
    {
        if (delta > 0)
        {
            int takeA = Mathf.Min(a, delta / 2);
            int takeB = delta - takeA;

            a -= takeA;
            b -= takeB;
        }
        else
        {
            int add = -delta;
            a += add / 2;
            b += add - add / 2;
        }

        a = Mathf.Clamp(a, 0, 100);
        b = Mathf.Clamp(b, 0, 100);
    }

    void OpenHistory()
    {
        if (history != null)
            history.OpenPanel();
    }

    bool IsValidProbability()
    {
        return badPercent + luckyPercent + skillPercent == 100;
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

    string RollByPercentage()
    {
        int roll = Random.Range(1, 101); // 1 - 100

        if (roll <= badPercent)
            return "Bad";

        if (roll <= badPercent + luckyPercent)
            return "Lucky";

        return "Skill";
    }

    void DoRoll()
    {
        if (!IsValidProbability())
        {
            Debug.LogError("Total probability must be 100%");
            return;
        }

        lastType = RollByPercentage();

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