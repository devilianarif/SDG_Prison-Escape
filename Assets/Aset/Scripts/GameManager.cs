using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerState playerState;
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

        if (currentFase == 0)
            playerState.SetDiceResult(diceRoller.lastResult);

        if (currentFase == 1)
            playerState.SetTypeCard(cardTypeRoller.lastType);

        if (currentFase == 2)
            playerState.SetScannedCardID(cardGameManager.lastCardID);


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


    // SWITCH UI FASE

    public void ActionFaseUI()
    {
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
        backFaseButton.gameObject.SetActive(true);
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
        backFaseButton.gameObject.SetActive(true);
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
        int curr = playerState.currentPlayerIndex;

        beforeDiceResult = playerState.players[curr].lastDiceResult;
        pilihDiceResult = -1;
        latestDiceResult = 0;

        pilihdadutext[0].text = "Before: " + beforeDiceResult;
        pilihdadutext[1].text = "Latest: 0";

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

        // tombol BEFORE
        pilihdadu[0].onClick.AddListener(() =>
        {
            pilihDiceResult = beforeDiceResult;

            pilihdadutext[0].text = "Before: " + beforeDiceResult + "\nSelected";

            if (latestDiceResult != 0)
                pilihdadutext[1].text = "Latest: " + latestDiceResult;

            applydice.gameObject.SetActive(true);
        });

        // tombol LATEST
        pilihdadu[1].onClick.AddListener(() =>
        {
            if (latestDiceResult == 0)
                return; // belum ada hasil reroll

            pilihDiceResult = latestDiceResult;

            pilihdadutext[1].text = "Latest: " + latestDiceResult + "\nSelected";
            pilihdadutext[0].text = "Before: " + beforeDiceResult;

            applydice.gameObject.SetActive(true);
        });


    }

    public void UpdateLatestDice(int value)
    {
        latestDiceResult = value;

        if (pilihDiceResult == latestDiceResult)
        {
            pilihdadutext[1].text = "Latest: " + latestDiceResult + "\nSelected";
        }
        else
        {
            pilihdadutext[1].text = "Latest: " + latestDiceResult;
        }


    }

    void ApplyReDiceEffect()
    {
        if (pilihDiceResult == -1)
            return;

        int curr = playerState.currentPlayerIndex;

        playerState.players[curr].lastDiceResult = pilihDiceResult;

        EndActionAndReturnToLobby();


    }
    public void HideReDicePanel()
    {
        for (int i = 0; i < fase33.Length; i++)
            if (fase33[i] != null)
                fase33[i].SetActive(false);
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
        Debug.Log("Fase 3 dinonaktifkan.");


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
            Debug.Log("Player " + (targetIndex + 1) + " HP 0. Heal diabaikan.");
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
        playerState.NextPlayer();
        justApplied = true; // penting
        Debug.Log("=== Next Player. Sekarang giliran Player "
            + (playerState.currentPlayerIndex + 1) + " ===");

        SceneManager.LoadScene(sceneloby);


    }
    public void ForceBackToFase1FromSkip()
    {
        Debug.Log("Kembali paksa ke Fase 1 dari SKIP karena kartu invalid.");

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


    }
}
