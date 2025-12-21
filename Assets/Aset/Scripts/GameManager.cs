using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerState playerState;
    [Header("Dice UI")]
    public DiceValueReader diceValueReader;
    bool isInReDice = false;


    public string sceneloby;
    [Header("Turn Player")]
    public string playerturn;
    public int defaultCanvasPlaneDistance = 1;
    public int diceCanvasPlaneDistance = 30;

    [Header("UI Action Buttons")]
    public Image[] actionButtonsImage;
    public Sprite defaultButton;
    public Sprite activeButton;
    public float normalScale = 1f;
    public float activeScale = 1.12f;
    public float animSpeed = 10f;

    [Header("Action Fase")]
    public Button nextFaseButton;
    public TMP_Text nextFaseButtonText;
    public Button backFaseButton;
    public Button[] actionFaseButtons;
    public GameObject[] actionFaseUI;
    public GameObject actionFase32UI;
    public bool matikanbackdanfasetbn = false;

    [Header("Skip Action Fase")]
    public Button skipActionFaseTurnButton;
    public GameObject skipActionFaseTurnUI;

    [Header("Fase 1 - Roll Dice")]
    public DiceRoller diceRoller;
    public GameObject[] dice3dplane1;

    [Header("Fase 2 - Roll Type Card")]
    public CardTypeRoll cardTypeRoller;

    [Header("Fase 3 - Scan Card + Action Card")]
    public CardGameManager cardGameManager;
    [Header("Fase 3 -action card dice efek")]
    public Button applydice;
    public GameObject[] fase33;
    public Button[] pilihdadu;
    public TMP_Text[] pilihdadutext;
    int pilihDiceResult = -1;
    int latestDiceResult = 0;
    int beforeDiceResult = 0;
    [Header("Fasse 3 - rerol card")]
    public GameObject fase34;
    public Button applyrerolcard;
    public bool isRerollingCardType = false;
    private string latestRerolledCardType = "";

    [Header("Fase Skip - Scan Card Skip")]
    public CardGameManager skipCardGameManager;

    public Canvas mainCanvas;

    [Header("fase info")]
    public GameObject[] validcentang;
    int currentFase = 0;
    int totalFase = 3;
    public bool justApplied = false;
    void Start()
    {
        isRerollingCardType = false;

        if (justApplied)
        {
            justApplied = false;
            return; // jangan ActionFaseUI() + jangan SaveState()
        }
        SetupActionButtons();

        foreach (var ui in actionFaseUI)
            ui.SetActive(false);

        actionFase32UI.SetActive(false);
        skipActionFaseTurnUI.SetActive(false);

        cardGameManager.OnScanStarted += HandleScanStart;
        cardGameManager.OnCardInfoShown += HandleCardInfoShown;
        cardGameManager.OnCardApplied += HandleCardApplied;

        ActionFaseUI();
    }


    // CALLBACK DARI CardGameManager


    void HandleScanStart()
    {
        nextFaseButton.gameObject.SetActive(false);

    }

    void HandleCardInfoShown()
    {
        // nextFaseButton.gameObject.SetActive(true);
        // nextFaseButtonText.text = "Apply";
        nextFaseButton.gameObject.SetActive(false);
        SaveState();
    }

    void HandleCardApplied()
    {
        nextFaseButtonText.text = "Next";
        nextFaseButton.gameObject.SetActive(false);
        // SaveState();
    }

    public void SaveState()
    {
        if (!playerState.IsPlayerTurn())
            return;

        int curr = playerState.currentPlayerIndex;

        if (playerState.players[curr].health <= 0)
            return; // tidak boleh menyimpan state untuk player mati



        if (currentFase == 1)
            playerState.SetTypeCard(cardTypeRoller.lastType);

        if (currentFase == 2)
            playerState.SetScannedCardID(cardGameManager.lastCardID);


    }
    public int GetFinalDiceValue(int rawValue)
    {
        int curr = playerState.currentPlayerIndex;

        var chara = cardGameManager
            .database
            .GetCharacter(playerState.players[curr].characterIndex);

        bool buffActive =
            isInReDice &&
            chara != null &&
            chara.bufvdice &&
            playerState.players[curr].charaBuffCooldown <= 0;

        if (buffActive)
            return rawValue + Mathf.RoundToInt(chara.valuedice);

        return rawValue;
    }


    // SETUP TOMBOL

    void SetupActionButtons()
    {

        for (int i = 0; i < actionFaseButtons.Length; i++)
        {
            int idx = i;
            actionFaseButtons[i].onClick.AddListener(() =>
            {
                currentFase = idx;
                ActionFaseUI();
                SaveState();
            });
        }

        nextFaseButton.onClick.AddListener(() =>
        {
            currentFase++;
            if (currentFase >= totalFase)
                currentFase = totalFase - 1;

            ActionFaseUI();
            SaveState();
        });

        backFaseButton.onClick.AddListener(() =>
        {
            currentFase--;
            if (currentFase < 0) currentFase = 0;

            ActionFaseUI();
            SaveState();
        });

        skipActionFaseTurnButton.onClick.AddListener(() =>
        {
            SkipActionFaseTurn();
            SaveState();
        });
    }
    void UpdateFaseButtonsLock()
    {
        foreach (var btn in actionFaseButtons)
            btn.interactable = !matikanbackdanfasetbn;
    }
    void UpdateNextButtonState()
{
    int curr = currentFase;

    bool allowNext = false;

    switch (curr)
    {
        case 0: // FASE 1: Roll Dice
            allowNext = playerState.players[playerState.currentPlayerIndex].lastDiceResult > 0;
            break;

        case 1: // FASE 2: Roll Type Card
            allowNext = !string.IsNullOrEmpty(
                playerState.players[playerState.currentPlayerIndex].lastTypeCard
            );
            break;

        case 2: // FASE 3: Scan Card
            allowNext = !string.IsNullOrEmpty(
                playerState.players[playerState.currentPlayerIndex].lastScannedCardID
            );
            break;
    }

    nextFaseButton.interactable = allowNext;
}



    // SWITCH UI FASE

    public void ActionFaseUI()
    {
        // ==== FORCE CLOSE PANEL REROLL ====
        if (fase34 != null)
            fase34.SetActive(false);

        isRerollingCardType = false;

        // === LOCK / UNLOCK FASE & BACK ===
        UpdateFaseButtonsLock();
        backFaseButton.gameObject.SetActive(!matikanbackdanfasetbn);
        HideReDicePanel();
        cardGameManager.HideCardInfoPanel();
        cardGameManager.HideScanUIPanel();
        for (int i = 0; i < actionButtonsImage.Length; i++)
        {
            bool active = (i == currentFase);
            actionButtonsImage[i].sprite = active ? activeButton : defaultButton;
        }

        for (int i = 0; i < actionFaseUI.Length; i++)
            actionFaseUI[i].SetActive(i == currentFase);

        if (currentFase == 0) ActionFase1();
        if (currentFase == 1) ActionFase2();
        if (currentFase == 2) ActionFase3();
        if (currentFase != 2)
            playerState.players[playerState.currentPlayerIndex].lastScannedCardID = "";
        UpdateChecklist();
        UpdateNextButtonState();

    }

    //=====================================================
    // FASE 1
    //=====================================================
    void ActionFase1()
    {

        if (mainCanvas)
            mainCanvas.planeDistance = diceCanvasPlaneDistance;

        hideskipUI();
        backFaseButton.gameObject.SetActive(false);

        if (diceRoller != null)
        {
            diceRoller.ResetDiceFully();
        }

        showdice3dui();

        nextFaseButtonText.text = "Next";
        nextFaseButton.gameObject.SetActive(true);
    }

    void showdice3dui()
    {
        for (int i = 0; i < dice3dplane1.Length; i++)
        {

            dice3dplane1[i].SetActive(true);
        }
    }
    void hidedice3dui()
    {
        for (int i = 0; i < dice3dplane1.Length; i++)
        {

            dice3dplane1[i].SetActive(false);
        }
    }
    //=====================================================
    // FASE 2
    //=====================================================
    void ActionFase2()
    {
        hidedice3dui();
        // backFaseButton.gameObject.SetActive(true);
        hideskipUI();
        if (mainCanvas)
            mainCanvas.planeDistance = defaultCanvasPlaneDistance;

        nextFaseButtonText.text = "Next";

        nextFaseButton.gameObject.SetActive(true);
    }

    //=====================================================
    // FASE 3
    //=====================================================
    void ActionFase3()
    {
        hidedice3dui();
        cardGameManager.ResetScannerState();
        // backFaseButton.gameObject.SetActive(true);
        nextFaseButton.gameObject.SetActive(false);
        hideskipUI();

        if (mainCanvas)
            mainCanvas.planeDistance = defaultCanvasPlaneDistance;



        cardGameManager.ShowScanUIPanel();

        cardGameManager.HighlightCurrentTurn();
        // nextFaseButtonText.text = "Apply";


    }
    public void OpenReDicePanel()
    {
        int curr = playerState.currentPlayerIndex; // <<< HARUS PALING ATAS

        beforeDiceResult = playerState.players[curr].lastDiceResult;
        pilihDiceResult = -1;
        latestDiceResult = 0;

        pilihdadutext[0].text = "Before: " + beforeDiceResult;
        pilihdadutext[1].text = "Latest: 0";

        isInReDice = true;

        if (mainCanvas != null)
            mainCanvas.planeDistance = diceCanvasPlaneDistance;

        foreach (var ui in actionFaseUI)
            ui.SetActive(false);

        cardGameManager.HideScanUIPanel();
        cardGameManager.HideCardInfoPanel();

        for (int i = 0; i < fase33.Length; i++)
            if (fase33[i] != null)
                fase33[i].SetActive(true);

        applydice.gameObject.SetActive(false);
        applydice.onClick.RemoveAllListeners();
        applydice.onClick.AddListener(ApplyReDiceEffect);

        pilihdadu[0].onClick.RemoveAllListeners();
        pilihdadu[1].onClick.RemoveAllListeners();

        // === tombol BEFORE ===
        pilihdadu[0].onClick.AddListener(() =>
        {
            pilihDiceResult = beforeDiceResult;

            // tampilkan NILAI ORIGINAL SAJA
            pilihdadutext[0].text = "Before: " + beforeDiceResult + " (selected)";
            pilihdadutext[1].text = "Latest: " + latestDiceResult;

            // tampilkan DETAIL di resultText
            int buff = GetBuffValue();
            diceValueReader.ShowResult(beforeDiceResult, buff);
            SaveDiceSelection(beforeDiceResult);

            applydice.gameObject.SetActive(true);
        });


        // === tombol LATEST ===
        pilihdadu[1].onClick.AddListener(() =>
     {
         if (latestDiceResult == 0) return;

         pilihDiceResult = latestDiceResult;

         pilihdadutext[1].text = "Latest: " + latestDiceResult + " (selected)";
         pilihdadutext[0].text = "Before: " + beforeDiceResult;

         int buff = GetBuffValue();
         diceValueReader.ShowResult(latestDiceResult, buff);
         SaveDiceSelection(latestDiceResult);


         applydice.gameObject.SetActive(true);
     });


    }

    void SaveDiceSelection(int rawValue)
    {
        int curr = playerState.currentPlayerIndex;
        int finalValue = rawValue;

        int buff = GetBuffValue();
        if (buff > 0)
            finalValue += buff;

        playerState.players[curr].lastDiceResult = finalValue;

        Debug.Log($"[DICE SAVE] Raw={rawValue}, Buff={buff}, Final={finalValue}");

        UpdateChecklist();
    }

    int GetBuffValue()
    {
        int curr = playerState.currentPlayerIndex;

        var chara = cardGameManager
            .database
            .GetCharacter(playerState.players[curr].characterIndex);

        if (
            isInReDice &&
            chara != null &&
            chara.bufvdice &&
            playerState.players[curr].charaBuffCooldown <= 0
        )
        {
            return Mathf.RoundToInt(chara.valuedice);
        }

        return 0;
    }

    public void UpdateLatestDice(int rawValue)
    {
        latestDiceResult = rawValue;
        pilihdadutext[1].text = "Latest: " + rawValue;
    }



    void ApplyReDiceEffect()
    {
        if (pilihDiceResult == -1)
            return;

        int curr = playerState.currentPlayerIndex;
        int baseDice = pilihDiceResult;
        int finalDice = baseDice;

        var chara = cardGameManager
            .database
            .GetCharacter(playerState.players[curr].characterIndex);

        bool buffActive = false;

        if (chara != null && chara.bufvdice && playerState.players[curr].charaBuffCooldown <= 0)
        {
            finalDice += Mathf.RoundToInt(chara.valuedice);
            buffActive = true;
            playerState.players[curr].charaBuffCooldown = chara.coldoncharabuf;
        }

        playerState.players[curr].lastDiceResult = finalDice;

        Debug.Log(
            $"[DICE APPLY] Base={baseDice} | BuffActive={buffActive} | Final={finalDice}"
        );

        var card = cardGameManager.GetLastScannedCard();
        if (card != null && card.cooldownroundcard > 0)
            playerState.players[curr].cardCooldown = card.cooldownroundcard;

        EndActionAndReturnToLobby();
    }


    public void HideReDicePanel()
    {
        for (int i = 0; i < fase33.Length; i++)
            if (fase33[i] != null)
                fase33[i].SetActive(false);
    }

    public void OpenRerollCardPanel()
    {
        int curr = playerState.currentPlayerIndex;
        cardTypeRoller.rolltypecard.interactable =
            playerState.players[curr].rerollChanceLeft > 0;

        isRerollingCardType = true;
        latestRerolledCardType = "";

        foreach (var ui in actionFaseUI)
            ui.SetActive(false);

        cardGameManager.HideScanUIPanel();
        cardGameManager.HideCardInfoPanel();

        if (fase34 == null)
        {
            Debug.LogError("fase34 belum di-assign di Inspector");
            return;
        }

        fase34.SetActive(true);

        if (applyrerolcard != null)
        {
            applyrerolcard.gameObject.SetActive(false);
            applyrerolcard.onClick.RemoveAllListeners();
            applyrerolcard.onClick.AddListener(ApplyRerollCardType);
        }
    }


    public void SetLatestRerolledCardType(string type)
    {
        latestRerolledCardType = type;

        // update UI teks hasil reroll
        cardTypeRoller.cardTypeText.text = type;

        applyrerolcard.gameObject.SetActive(true);
    }
    void ApplyRerollCardType()
    {
        if (string.IsNullOrEmpty(latestRerolledCardType))
            return;

        int curr = playerState.currentPlayerIndex;
        playerState.players[curr].lastTypeCard = latestRerolledCardType;

        isRerollingCardType = false;
        fase34.SetActive(false);

        EndActionAndReturnToLobby();
    }


    //=====================================================
    // FASE SKIP
    //=====================================================
    public void SkipActionFaseTurn()
    {
        hidedice3dui();
        cardGameManager.ResetScannerState();
        if (mainCanvas)
            mainCanvas.planeDistance = defaultCanvasPlaneDistance;

        backFaseButton.gameObject.SetActive(false);
        nextFaseButton.gameObject.SetActive(false);

        // Sembunyikan semua UI action fase
        foreach (var ui in actionFaseUI)
            ui.SetActive(false);
        actionFase32UI.SetActive(false);


        for (int i = 0; i < actionButtonsImage.Length; i++)
        {
            actionButtonsImage[i].sprite = defaultButton;
        }


        skipActionFaseTurnButton.image.sprite = activeButton;


        skipActionFaseTurnUI.SetActive(true);


        skipCardGameManager.ShowScanUIPanel();


        DisableActionFase3();
    }

    void DisableActionFase3()
    {
        Debug.Log("Fase 3 is disabled.");


        cardGameManager.HideScanUIPanel();


    }

    public void hideskipUI()
    {
        skipActionFaseTurnUI.SetActive(false);
    }
    public void ApplyCardEffectToPlayer(int targetIndex, int hpChange)
    {
        int currentHp = playerState.players[targetIndex].health;

        // kalau HP 0 dan efeknya heal (hpChange < 0), abaikan
        if (currentHp <= 0 && hpChange < 0)
        {
            Debug.Log("Player " + (targetIndex + 1) + " HP 0. Heal is ignored.");
            return;
        }

        if (hpChange > 0)
        {
            // damage
            playerState.players[targetIndex].health -= hpChange;
            if (playerState.players[targetIndex].health < 0)
                playerState.players[targetIndex].health = 0;
        }
        else if (hpChange < 0)
        {
            // heal, hpChange negatif, jadi -hpChange = nilai heal
            playerState.players[targetIndex].health -= hpChange;
            if (playerState.players[targetIndex].health > 5)
                playerState.players[targetIndex].health = 5;
        }

        // di sini kalau mau ditambah efek lain boleh


    }
    public void EndActionAndReturnToLobby()
    {
        playerState.players[playerState.currentPlayerIndex].rerollChanceLeft = 0;

        playerState.NextPlayer();
        justApplied = true; // penting
        Debug.Log("=== Next Player. Now, turn of Player "
            + (playerState.currentPlayerIndex + 1) + " ===");

        SceneManager.LoadScene(sceneloby);


    }
    public void ForceBackToFase1FromSkip()
    {
        Debug.Log("Forced back to Phase 1 from SKIP due to an invalid card.");

        currentFase = 0;
        ActionFaseUI();
    }

    public bool IsAllChecklistOK()
    {
        bool diceOK = validcentang[0].activeSelf;
        bool typeOK = validcentang[1].activeSelf;
        bool cardOK = validcentang[2].activeSelf;

        return diceOK && typeOK && cardOK;


    }
    public void UpdateChecklist()
    {
        int curr = playerState.currentPlayerIndex;

        validcentang[0].SetActive(playerState.players[curr].lastDiceResult > 0);
        validcentang[1].SetActive(!string.IsNullOrEmpty(playerState.players[curr].lastTypeCard));
        validcentang[2].SetActive(!string.IsNullOrEmpty(playerState.players[curr].lastScannedCardID));

        UpdateNextButtonState();

    }
}
