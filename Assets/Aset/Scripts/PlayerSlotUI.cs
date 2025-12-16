using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSlotUI : MonoBehaviour
{
    public Button[] Teamlist;
    public Button[] buttonplayerList;
    public Image[] buttonImage;
    public Sprite spriteNormal;
    public Sprite spriteActive;

    public TMP_Text[] playerNameText;
    public TMP_Text[] statusText;
    public CharacterButton characterButton;

    int activeIndex = 0;

    public float normalScale = 1f;
    public float activeScale = 1.12f;
    public float animSpeed = 10f;

    Vector3[] targetScale;
    // urutan giliran: Player 1, 3, 4, 2
    int[] turnOrder = { 0, 2, 3, 1 };

    // team berdasarkan giliran
    // 0 = Team A, 1 = Team B
    int[] teamByTurn = { 0, 1, 1, 0 };
    int currentTurn = 0;

    public int GetCurrentPlayerByTurn()
    {
        return turnOrder[currentTurn];
    }

    public void AdvanceTurn()
    {
        currentTurn++;

        if (currentTurn >= turnOrder.Length)
            return;

        int nextPlayer = turnOrder[currentTurn];
        SelectPlayer(nextPlayer);
    }

    void Start()
    {
        SetupButtons();
        SetPlayerNames();

        targetScale = new Vector3[buttonplayerList.Length];
        for (int i = 0; i < buttonplayerList.Length; i++)
        {
            targetScale[i] = Vector3.one * normalScale;
        }

        UpdateUI();
    }

    void Update()
    {
        AnimateScale();
    }

    void SetupButtons()
    {
        for (int i = 0; i < buttonplayerList.Length; i++)
        {
            int index = i;
            buttonplayerList[i].onClick.AddListener(() => SelectPlayer(index));
        }
    }

    void SetPlayerNames()
    {
        for (int i = 0; i < playerNameText.Length; i++)
        {
            playerNameText[i].text = "Player " + (i + 1);
        }
    }

    public void SelectPlayer(int index)
    {
        activeIndex = index;

        int turnIndex = GetTurnIndexByPlayer(index);
        if (turnIndex == -1) return;

        currentTurn = turnIndex;

        int team = teamByTurn[currentTurn];
        UpdateTeamUI(team);

        if (characterButton != null)
            characterButton.SetActivePlayer(index);

        UpdateUI();
    }

    void UpdateTeamUI(int team)
    {
        for (int i = 0; i < Teamlist.Length; i++)
        {
            Teamlist[i].interactable = (i == team);
        }
    }


    void UpdateUI()
    {
        for (int i = 0; i < buttonImage.Length; i++)
        {
            bool active = (i == activeIndex);

            buttonImage[i].sprite = active ? spriteActive : spriteNormal;
            statusText[i].text = active ? "Selected" : "Wait";

            targetScale[i] = active ? Vector3.one * activeScale : Vector3.one * normalScale;
        }
    }
    int GetTurnIndexByPlayer(int playerIndex)
    {
        for (int i = 0; i < turnOrder.Length; i++)
        {
            if (turnOrder[i] == playerIndex)
                return i;
        }
        return -1;
    }

    void AnimateScale()
    {
        for (int i = 0; i < buttonplayerList.Length; i++)
        {
            buttonplayerList[i].transform.localScale = Vector3.Lerp(
                buttonplayerList[i].transform.localScale,
                targetScale[i],
                Time.deltaTime * animSpeed
            );
        }
    }


}