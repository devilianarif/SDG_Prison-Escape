using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


public class LobbyManager : MonoBehaviour
{
    [Header("Character Data")]


    public PlayerState playerState;
    public CardChara[] characterDatabase; // semua data karakter


    [Header("Player UI Elements")]
    public TMP_Text[] playerCharaNameText;
    public TMP_Text[] playerHealthText;
    public TMP_Text[] playerInfoTurnsText;
    public Image[] playerCardbg;
    public Image[] playerCardChara;
    public Image[] playerHealthBarFill;
    public Button[] playerActionButtons;
    public string sceneActionUI;
    public Image[] playerMatiImage;
    public TMP_Text[] playerMatiText;
    [Header("Set Turn System")]
    public TMP_Text[] turnInfoText;
    public Button[] backStepTurnButton;
    public Button[] resetAllTurnsButton;

    [Header("Police UI Elements")]
    public Button policeActionButton;
    public string scenePoliceActionUI;



    //------------------------------------------
    void Start()
    {
        if (playerState.backup[0] == null)
            playerState.ResetPlayerData(); // paksa  jika belum
        GetPlayerSelected();
        faseTurn();
        btnklik();
        UpdateHealthUI();
        playerMatiUI();

    }
    void btnklik()
    {
        for (int i = 0; i < backStepTurnButton.Length; i++)
        {

            backStepTurnButton[i].onClick.AddListener(() => BackStepPlayer());
        }



        for (int i = 0; i < 4; i++)
        {
            int p = i;
            playerActionButtons[i].onClick.AddListener(() => actionPerTurnPlayer(p));
        }
        policeActionButton.onClick.AddListener(() => actionPerTurnPolice());
        for (int i = 0; i < resetAllTurnsButton.Length; i++)
        {
            resetAllTurnsButton[i].onClick.AddListener(() => resetTurn());
        }


    }

    public void resetTurn()
    {
        playerState.ResetPlayerData();

        UpdateHealthUI();
        playerMatiUI();
        faseTurn();

    }


    //------------------------------------------
    // Ambil data karakter dari ScriptableObject
    //------------------------------------------
    public void GetPlayerSelected()
    {
        for (int i = 0; i < 4; i++)
        {
            int charIndex = playerState.players[i].characterIndex;
            CardChara data = characterDatabase[charIndex];

            playerCharaNameText[i].text = playerState.players[i].playername;
            playerCardbg[i].sprite = data.cardfull;
            playerCardChara[i].sprite = data.karakter;
        }
    }



    //------------------------------------------
    // Setup UI turn awal
    //------------------------------------------
    public void faseTurn()
    {
        for (int i = 0; i < 4; i++)
            playerInfoTurnsText[i].gameObject.SetActive(false);

        int t = playerState.currentTurn;
        for (int i = 0; i < turnInfoText.Length; i++)
            turnInfoText[i].text = "Turn " + t;

        // StartCoroutine(ShowTurnText());

        for (int i = 0; i < backStepTurnButton.Length; i++)
            backStepTurnButton[i].gameObject.SetActive(false);

        for (int i = 0; i < resetAllTurnsButton.Length; i++)
            resetAllTurnsButton[i].gameObject.SetActive(false);

        for (int i = 0; i < 4; i++)
            playerActionButtons[i].gameObject.SetActive(false);

        policeActionButton.gameObject.SetActive(false);

        NextTurn();


    }

    //------------------------------------------
    // Sistem turn 1-4 player lalu polisi
    //------------------------------------------
    public void NextTurn()
    {
        int p = playerState.currentPlayerIndex;
        int t = playerState.currentTurn;

        // matikan semua dulu
        for (int i = 0; i < 4; i++)
        {
            playerActionButtons[i].gameObject.SetActive(false);
            playerInfoTurnsText[i].gameObject.SetActive(false);
        }

        policeActionButton.gameObject.SetActive(false);

        // kalau giliran polisi
        if (playerState.IsPoliceTurn())
        {
            policeActionButton.gameObject.SetActive(true);
        }
        // kalau giliran player
        else if (playerState.IsPlayerTurn())
        {
            playerActionButtons[p].gameObject.SetActive(true);
            playerInfoTurnsText[p].gameObject.SetActive(true);
            playerInfoTurnsText[p].text = "P" + (p + 1) + "\n Turn";
        }

        for (int i = 0; i < turnInfoText.Length; i++)
            turnInfoText[i].text = "Turn " + t;



        // refresh HP dan status mati setiap pindah turn
        UpdateHealthUI();
        playerMatiUI();
        if (playerState.currentTurn >= 2)
        {
            for (int i = 0; i < backStepTurnButton.Length; i++)
                backStepTurnButton[i].gameObject.SetActive(true);

            for (int i = 0; i < resetAllTurnsButton.Length; i++)
                resetAllTurnsButton[i].gameObject.SetActive(true);
        }

    }
    IEnumerator ShowTurnText()
    {
        for (int i = 0; i < turnInfoText.Length; i++)
            turnInfoText[i].gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < turnInfoText.Length; i++)
            turnInfoText[i].gameObject.SetActive(false);
    }

    //------------------------------------------
    // Aksi player pindah ke scene Action UI
    //------------------------------------------
    public void actionPerTurnPlayer(int playerIndex)
    {
        playerState.SaveBackup(playerIndex);
        playerState.ResetActionData(playerIndex);
        SceneManager.LoadScene(sceneActionUI);


    }

    //------------------------------------------
    // Aksi polisi pindah ke scene polisi
    //------------------------------------------
    public void actionPerTurnPolice()
    {
        SceneManager.LoadScene(scenePoliceActionUI);
    }


    public void UpdateHealthUI()
    {
        for (int i = 0; i < 4; i++)
        {
            int hp = playerState.players[i].health;
            int maxHP = 5;

            playerHealthText[i].text = hp + " / " + maxHP;
            playerHealthBarFill[i].fillAmount = (float)hp / maxHP;
        }
    }

    public void playerMatiUI()
    {
        //jika player helath 0 tampilkan image mati
        for (int i = 0; i < 4; i++)
        {
            int hp = playerState.players[i].health;

            if (hp <= 0)
            {
                playerMatiImage[i].gameObject.SetActive(true);
                playerMatiText[i].text = "P" + (i + 1) + " eliminated";
            }
            else
            {
                playerMatiImage[i].gameObject.SetActive(false);
                playerMatiText[i].text = "";
            }
        }
    }
    public void BackStepPlayer()
    {
        if (playerState.hasBackstep)
            return;

        playerState.hasBackstep = true;

        for (int i = 0; i < backStepTurnButton.Length; i++)
            backStepTurnButton[i].gameObject.SetActive(false);

        if (playerState.currentPlayerIndex > 0)
            playerState.currentPlayerIndex--;
        else
        {
            playerState.currentPlayerIndex = 3;

            if (playerState.currentTurn > 1)
                playerState.currentTurn--;
        }

        int safety = 0;
        while (safety < 10 && playerState.IsPlayerDead(playerState.currentPlayerIndex))
        {
            safety++;

            if (playerState.currentPlayerIndex > 0)
                playerState.currentPlayerIndex--;
            else
            {
                playerState.currentPlayerIndex = 3;

                if (playerState.currentTurn > 1)
                    playerState.currentTurn--;
            }
        }

        int p = playerState.currentPlayerIndex;

        playerState.RestoreBackup(p);
        playerState.ResetActionData(p);

        UpdateHealthUI();
        playerMatiUI();

        for (int i = 0; i < 4; i++)
        {
            playerActionButtons[i].gameObject.SetActive(false);
            playerInfoTurnsText[i].gameObject.SetActive(false);
        }

        policeActionButton.gameObject.SetActive(false);

        if (playerState.IsPoliceTurn())
        {
            policeActionButton.gameObject.SetActive(true);
        }
        else
        {
            playerActionButtons[p].gameObject.SetActive(true);
            playerInfoTurnsText[p].gameObject.SetActive(true);
            playerInfoTurnsText[p].text = "P" + (p + 1) + "\n Turn";
        }

        for (int i = 0; i < turnInfoText.Length; i++)
            turnInfoText[i].text = "Turn " + playerState.currentTurn;


    }

}
