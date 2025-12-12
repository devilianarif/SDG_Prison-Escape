using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardTypeRoll : MonoBehaviour
{
    public Image card;
    public string[] cardtypename;
    public Sprite[] cardTypeSprites;
    public TMP_Text cardTypeText;
    public Button rolltypecard;
    public CardTypeRollHistory history;
    public Button historyButton;
    public string lastType;

    public GameManager gameManager;
    void Start()
    {
        rolltypecard.onClick.AddListener(RandomSprite);
        historyButton.onClick.AddListener(OpenHistory);

        historyButton.gameObject.SetActive(true);
        cardTypeText.text = "";
    }

    void OpenHistory()
    {
        if (history != null)
            history.OpenPanel();
    }

    void RandomSprite()
    {
        int index = Random.Range(0, cardTypeSprites.Length);

        card.sprite = cardTypeSprites[index];
        string name = cardtypename[index];
        cardTypeText.text = name;
        Debug.Log("Kartu terpilih: " + name);

        if (history != null)
            history.AddHistory(name);

        lastType = name;

        gameManager.playerState.SetTypeCard(lastType); // save realtime
        gameManager.SaveState();
        gameManager.UpdateChecklist();


    }
}
