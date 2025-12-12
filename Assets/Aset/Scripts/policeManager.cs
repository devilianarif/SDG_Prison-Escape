using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class policeManager : MonoBehaviour
{
    public string sceneLoby;
    public Button backToLobyButton;
    public Button applyToLobyButton;
    public Button rollPoliceButton;
    public TMP_Text valueText;
    [Header("ui penjara")]
    public GameObject penjaraPlayerui;
    public Button penjaraplayer;
    public Button[] playerpilihbtn;
    public Button backplayerpenjara;
    public TMP_Text[] namaplayer;
    public Image[] healthbar;
    public GameObject[] paneltandapenjara;
    public TMP_Text[] textTandaPenajra;



    public PlayerState playerState;
    public DiceRoller[] diceRoller;
    public Wheel2DRoller wheel2DRoller;
    public Canvas uiCanvas;
    int oldPoliceDice;
    string oldPoliceWheel;
    bool hasBackup = false;
    int penjaraTarget = -1;

    void Start()
    {
        backToLobyButton.onClick.AddListener(backLoby);
        applyToLobyButton.onClick.AddListener(applyLoby);
        rollPoliceButton.onClick.AddListener(RollPolice);
        penjaraplayer.onClick.AddListener(ShowPenjaraUI);
        backplayerpenjara.onClick.AddListener(HidePenjaraUI);
        penjaraPlayerui.SetActive(false);
        uiCanvas.planeDistance = 30;
        penjaraplayer.onClick.AddListener(ApplyPenjara);
        applyToLobyButton.interactable = playerState.polices[0].lastDiceResult > 0;
        pilihplayer();

    }

    public void backLoby()
    {
        if (hasBackup)
        {
            playerState.polices[0].lastDiceResult = oldPoliceDice;
            playerState.polices[0].laststepwhellValue = oldPoliceWheel;
        }

        SceneManager.LoadScene(sceneLoby);
    }


    public void applyLoby()
    {
        playerState.NextPlayer();
        SceneManager.LoadScene(sceneLoby);

    }

    public void RollPolice()
    {
        oldPoliceDice = playerState.polices[0].lastDiceResult;
        oldPoliceWheel = playerState.polices[0].laststepwhellValue;
        hasBackup = true;

        StartCoroutine(RollSequencePolice());
    }
    IEnumerator RollSequencePolice()
    {
        valueText.text = "...";

        // reset hasil dadu lama dulu
        for (int i = 0; i < diceRoller.Length; i++)
        {
            diceRoller[i].lastResult = 0;
            if (diceRoller[i].valueReader != null)
                diceRoller[i].valueReader.ForceValue(0);
        }

        // mulai roll semua dadu
        for (int i = 0; i < diceRoller.Length; i++)
            diceRoller[i].RollButton();

        if (playerState.polices[0].isWheel)
            wheel2DRoller.Rotate();

        // sekarang baru nunggu hasil BARU
        yield return new WaitUntil(() =>
            diceRoller[0].lastResult != 0 &&
            diceRoller[1].lastResult != 0 &&
            diceRoller[2].lastResult != 0 &&
            diceRoller[3].lastResult != 0
        );

        if (playerState.polices[0].isWheel)
            yield return new WaitUntil(() => wheel2DRoller.snapDone);

        string diceOnly =
            "P1 (" + diceRoller[0].lastResult + ") - " +
            "P2 (" + diceRoller[1].lastResult + ") - " +
            "P3 (" + diceRoller[2].lastResult + ") - " +
            "P4 (" + diceRoller[3].lastResult + ") ";

        valueText.text = diceOnly;

        applyToLobyButton.interactable = true;

        playerState.polices[0].lastDiceResult = diceRoller[0].lastResult;
        playerState.polices[1].lastDiceResult = diceRoller[1].lastResult;
        playerState.polices[2].lastDiceResult = diceRoller[2].lastResult;
        playerState.polices[3].lastDiceResult = diceRoller[3].lastResult;

        if (playerState.polices[0].isWheel)
            playerState.SetPoliceWheel(wheel2DRoller.lastWheelValue, 0);


    }


    public void ShowPenjaraUI()
    {
        penjaraPlayerui.SetActive(true);
        uiCanvas.planeDistance = 1;
        LoadPlayerPenjaraUI();
    }
    public void HidePenjaraUI()
    {
        penjaraPlayerui.SetActive(false);
        uiCanvas.planeDistance = 30;
    }



    public void SelectPenjaraTarget(int index)
    {
        penjaraTarget = index;

        for (int i = 0; i < playerpilihbtn.Length; i++)
        {
            float scale = i == index ? 0.19f : 0.15f;
            playerpilihbtn[i].transform.localScale = new Vector3(scale, scale, scale);
        }


    }
    public void ApplyPenjara()
    {
        if (penjaraTarget == -1) return;

        playerState.players[penjaraTarget].health = 0;

        HidePenjaraUI();


    }
    public void LoadPlayerPenjaraUI()
    {
        for (int i = 0; i < 4; i++)
        {
            namaplayer[i].text = playerState.players[i].playername;

            float fill = Mathf.Clamp01(playerState.players[i].health / 5f);
            healthbar[i].fillAmount = fill;

            bool dead = playerState.players[i].health <= 0;

            paneltandapenjara[i].SetActive(dead);

            textTandaPenajra[i].text = dead ? "Player " + (i + 1) + " di penjara" : "";
            playerpilihbtn[i].interactable = !dead;
        }


    }
    public void pilihplayer()
    {
        for (int i = 0; i < playerpilihbtn.Length; i++)
        {
            int idx = i;
            playerpilihbtn[i].onClick.AddListener(() => SelectPenjaraTarget(idx));
        }
    }
}
