using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class CardGameManager : MonoBehaviour
{
    [Header("script ")]
    public GameManager gameManager;
    public QRScannerCard scannerCardType;
    public QRScannerSkip scannerCardSkip;
    public CardDatabase database;
    [Header("UI kartu")]
    public Button ScanKartuButton;
    public Button ScanKartuSkipButton;
    public string lastCardID;
    private bool isScanningCardType = false;
    private bool isScanningCardSkip = false;
    [Header("set kartu")]
    public GameObject cardInfoPanel;
    public GameObject scanUIPanel;
    [Header("data kartu")]
    public Image cardbg;
    public Image carddisplay;
    public TMP_Text cardname;
    public TMP_Text cardjenis;
    public TMP_Text cardnamedetail;
    public TMP_Text cardInfodetail;
    [Header("player kartu")]
    public Button actionCard;
    public Button[] cardPlayerButton;
    public Image[] cardplayer;
    public Image[] karakterPlayer;

    public float aktifplayerY = 35f;
    public float defaultplayerY = 0;
    public TMP_Text[] matiPlayertext;
    public GameObject[] matiPlayer;

    public TMP_Text[] healthText;
    public TMP_Text[] playerNameText;
    public Image[] FillHealthBar;

    CardData lastScannedCard;
    int selectedTargetIndex = -1;

    public Action OnScanStarted;
    public Action OnCardInfoShown;
    public Action OnCardApplied;

    void Start()
    {
        ScanKartuButton.onClick.AddListener(StartScanningCardType);
        ScanKartuSkipButton.onClick.AddListener(StartScanningCardSkip);

        scannerCardType.StopCamera();
        scannerCardSkip.StopCamera();

        scannerCardType.OnQRRead += HandleQR;
        scannerCardSkip.OnQRRead += HandleSkipQR;
        showdataplayer();
        ShowCharacterDisplay();
        cardInfoPanel.SetActive(false);
        scanUIPanel.SetActive(false);
    }

    void StartScanningCardType()
    {
        if (isScanningCardType) return;

        isScanningCardType = true;
        isScanningCardSkip = false;

        scannerCardType.StartCamera();
        scannerCardSkip.StopCamera();

        OnScanStarted?.Invoke();
    }

    void StartScanningCardSkip()
    {
        if (isScanningCardSkip) return;

        isScanningCardSkip = true;
        isScanningCardType = false;

        scannerCardSkip.StartCamera();
        scannerCardType.StopCamera();
    }

    void HandleQR(string id)
    {
        lastCardID = id;
        if (!isScanningCardType) return;

        var card = database.GetCard(id);
        if (card == null) return;

        lastScannedCard = card;
        // ===== TOLAK SKIP CARD DI SCANNER KARTU ==== 
        if (card.isSkipCard)  // :contentReference[oaicite:0]{index=0}
        {
            scannerCardType.StopCamera();
            isScanningCardType = false;

            ResetScannerState();
            gameManager.SkipActionFaseTurn();  // langsung masuk ke fase skip
            return;
        }
        gameManager.playerState.SetScannedCardID(id);
        gameManager.SaveState();
        gameManager.UpdateChecklist();
        DisplayCardInfo(card);

        scannerCardType.StopCamera();
        isScanningCardType = false;
    }
    void HandleSkipQR(string id)
    {
        lastCardID = id;
        if (!isScanningCardSkip) return;

        var card = database.GetCard(id);
        if (card == null) return;

        // jangan simpan scanned ID untuk skip
        lastScannedCard = card;

        bool skip = card.isSkipCard;

        if (!skip)
        {
            scannerCardSkip.StopCamera();
            isScanningCardSkip = false;
            ResetScannerState();
            gameManager.ForceBackToFase1FromSkip();
            return;
        }

        PerformSkipAction(card);

        scannerCardSkip.StopCamera();
        isScanningCardSkip = false;


    }
    void PerformSkipAction(CardData card)
    {
        gameManager.EndActionAndReturnToLobby();
    }

    void DisplayCardInfo(CardData card)
    {
        cardbg.sprite = card.spriteCard;
        carddisplay.sprite = card.karakter;
        cardnamedetail.text = card.nama;
        cardname.text = card.nama;
        cardjenis.text = card.cardType.ToString();

        switch (card.cardType)
        {
            case CardType.Bad:
                cardjenis.color = HexToColor("AE4E4E");
                break;

            case CardType.Lucky:
                cardjenis.color = HexToColor("4AAF6A");
                break;

            case CardType.Skill:
                cardjenis.color = HexToColor("4C7BAE");
                break;
        }


        cardInfodetail.text = "Attack : " + card.attack + "  Heal : " + card.heal + "\nInformation : " + card.informasi + "\nEffect : " + card.efekkartu ;

        ShowCardInfoPanel();
    }
    Color HexToColor(string hex)
    {
        Color c;
        ColorUtility.TryParseHtmlString("#" + hex, out c);
        return c;
    }

    public void ShowCardInfoPanel()
    {
        cardInfoPanel.SetActive(true);
        gameManager.nextFaseButtonText.text = "Apply";
        gameManager.nextFaseButton.gameObject.SetActive(true);
        HideScanUIPanel();
        ActionCardData();
        actionCard.interactable = gameManager.IsAllChecklistOK();
        OnCardInfoShown?.Invoke();
    }

    public void showdataplayer()
    {

        var ps = gameManager.playerState;

        for (int i = 0; i < 4; i++)
        {
            int hp = ps.players[i].health;

            playerNameText[i].text = ps.players[i].playername;
            healthText[i].text = hp.ToString() + "/5";

            float fill = Mathf.Clamp01(hp / 5f);
            FillHealthBar[i].fillAmount = fill;

            bool isDead = hp <= 0;

            if (i < matiPlayer.Length)
                matiPlayer[i].SetActive(isDead);

            if (i < matiPlayertext.Length)
                matiPlayertext[i].text = isDead
                    ? "Player " + (i + 1) + " di penjara"
                    : "";

            Debug.Log("HP P3 sekarang: " + gameManager.playerState.players[2].health);
        }


    }
    public void ShowCharacterDisplay()
    {
        var ps = gameManager.playerState;

        for (int i = 0; i < 4; i++)
        {
            int idx = ps.players[i].characterIndex;

            CardChara data = database.GetCharacter(idx);
            if (data == null) continue;

            if (i < cardplayer.Length)
                cardplayer[i].sprite = data.cardfull;

            if (i < karakterPlayer.Length)
                karakterPlayer[i].sprite = data.karakter;
        }
    }


    public void HideCardInfoPanel()
    {
        cardInfoPanel.SetActive(false);
    }

    public void ActionCardData()
    {
        if (lastScannedCard == null) return;

        var ps = gameManager.playerState;
        selectedTargetIndex = -1;

        bool isReLife = lastScannedCard.isRelifePlayer;
        int current = ps.currentPlayerIndex;

        for (int i = 0; i < cardPlayerButton.Length; i++)
        {
            int hp = ps.players[i].health;
            bool isDead = hp <= 0;

            cardPlayerButton[i].gameObject.SetActive(true);
            cardPlayerButton[i].onClick.RemoveAllListeners();

            // panel mati dan text
            if (i < matiPlayer.Length)
                matiPlayer[i].SetActive(isDead);

            if (i < matiPlayertext.Length)
                matiPlayertext[i].text = isDead
                    ? "Player " + (i + 1) + " di penjara"
                    : "";

            RectTransform rt = cardPlayerButton[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, defaultplayerY);

            if (isReLife)
            {
                bool allow = (isDead || i == current);

                cardPlayerButton[i].interactable = allow;

                if (allow)
                {
                    int idx = i;
                    cardPlayerButton[i].onClick.AddListener(() => SelectPlayerTarget(idx));
                }

                continue;
            }

            cardPlayerButton[i].interactable = !isDead;

            if (!isDead)
            {
                int idx = i;
                cardPlayerButton[i].onClick.AddListener(() => SelectPlayerTarget(idx));
            }
        }

        actionCard.onClick.RemoveAllListeners();
        actionCard.onClick.AddListener(ApplyCardToSelectedPlayer);

        HighlightCurrentTurn();


    }
    void SelectPlayerTarget(int index)
    {
        selectedTargetIndex = index;

        for (int i = 0; i < cardPlayerButton.Length; i++)
        {
            RectTransform rt = cardPlayerButton[i].GetComponent<RectTransform>();

            float y = i == index ? aktifplayerY : defaultplayerY;

            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
        }
    }
    void ApplyCardToSelectedPlayer()
    {
        if (!gameManager.IsAllChecklistOK())
            return;

        if (selectedTargetIndex == -1)
            return;

        var ps = gameManager.playerState;

        // target mati normal → batal
        if (ps.players[selectedTargetIndex].health <= 0 &&
            !lastScannedCard.isRelifePlayer)
            return;

        // KHUSUS RELIFE
        if (lastScannedCard.isRelifePlayer)
        {
            ps.players[selectedTargetIndex].health = 5;

            BtnPlayerHide();
            HideCardInfoPanel();

            gameManager.EndActionAndReturnToLobby();
            OnCardApplied?.Invoke();
            return;
        }

        if (lastScannedCard.isEfekReDice)
        {
            HideCardInfoPanel();
            HideScanUIPanel();
            gameManager.OpenReDicePanel();
            return;
        }

        int hpChange = GetHpChangeFromCard(lastScannedCard);
        gameManager.ApplyCardEffectToPlayer(selectedTargetIndex, hpChange);

        BtnPlayerHide();
        HideCardInfoPanel();

        gameManager.EndActionAndReturnToLobby();
        OnCardApplied?.Invoke();


    }

    public int GetHpChangeFromCard(CardData card)
    {
        if (card.cardType == CardType.Bad)
        {
            return card.attack;
        }

        if (card.cardType == CardType.Lucky)
        {
            return -card.heal;
        }

        if (card.cardType == CardType.Skill)
        {
            return card.attack;
        }

        return 0;
    }
    public void ShowScanUIPanel()
    {
        scanUIPanel.SetActive(true);

        showdataplayer();

        int curr = gameManager.playerState.currentPlayerIndex;
        var ps = gameManager.playerState;

        for (int i = 0; i < cardPlayerButton.Length; i++)
        {
            int hp = ps.players[i].health;
            bool isDead = hp <= 0;

            cardPlayerButton[i].gameObject.SetActive(true);

            // tombol mati → tidak bisa di-klik
            // tombol hidup → hanya bisa diklik kalau bukan current player
            cardPlayerButton[i].interactable = !isDead;

            // panel mati
            if (i < matiPlayer.Length)
                matiPlayer[i].SetActive(isDead);

            if (i < matiPlayertext.Length)
                matiPlayertext[i].text = isDead
                    ? "Player " + (i + 1) + " di penjara"
                    : "";

            RectTransform rt = cardPlayerButton[i].GetComponent<RectTransform>();

            // posisi awal default
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, defaultplayerY);
        }

        HighlightCurrentTurn();


    }

    public void HighlightCurrentTurn()
    {
        var ps = gameManager.playerState;
        int curr = ps.currentPlayerIndex;

        bool dead = ps.players[curr].health <= 0;

        for (int i = 0; i < cardPlayerButton.Length; i++)
        {
            RectTransform rt = cardPlayerButton[i].GetComponent<RectTransform>();

            // kalau current player mati, tidak ada yang naik
            float y = (!dead && i == curr) ? aktifplayerY : defaultplayerY;

            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
        }

        // kalau player turn hidup → langsung set target
        selectedTargetIndex = dead ? -1 : curr;


    }


    public void HideScanUIPanel()
    {
        scanUIPanel.SetActive(false);
        BtnPlayerHide();
    }

    public void BtnPlayerShow()
    {
        foreach (var btn in cardPlayerButton)
            btn.gameObject.SetActive(true);
    }

    public void BtnPlayerHide()
    {
        foreach (var btn in cardPlayerButton)
            btn.gameObject.SetActive(false);
    }

    public void ResetScannerState()
    {
        isScanningCardType = false;
        isScanningCardSkip = false;

        scannerCardType.StopCamera();
        scannerCardSkip.StopCamera();

        ResetPreview();
    }

    void ResetPreview()
    {
        if (scannerCardType != null)
        {
            if (scannerCardType.defaultTexture != null)
                scannerCardType.preview.texture = scannerCardType.defaultTexture;
            else
                scannerCardType.preview.texture = null;
        }

        if (scannerCardSkip != null)
        {
            if (scannerCardSkip.defaultTexture != null)
                scannerCardSkip.preview.texture = scannerCardSkip.defaultTexture;
            else
                scannerCardSkip.preview.texture = null;
        }
    }


}