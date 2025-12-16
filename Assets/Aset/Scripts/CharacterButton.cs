using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterButton : MonoBehaviour
{
    [Header("Character Data (Scriptable Object)")]
    public CardChara[] characterData;

    [Header("UI Settings")]
    public Button[] buttonListChara;

    public Image[] characterImageCard;
    public Image[] charaImageKarakter;
    public TMP_Text[] charaRoletext;
    public TMP_Text[] charaabilityAstext;
    public TMP_Text[] charaefektext;
    public TMP_Text[] selectedLabel;
    public Image[] panelselectedChara;

    public PlayerSlotUI playerSlotUI;

    int[] characterOwner;
    int currentPlayer = 0;

    void Start()
    {
        characterOwner = new int[buttonListChara.Length];

        for (int i = 0; i < characterOwner.Length; i++)
            characterOwner[i] = -1;

        SetupButtons();
        ResetPanels();
        ResetLabels();
        DisplayCharacterSprites();
        DisplayCharacterNames();
    }



    void DisplayCharacterSprites()
    {
        for (int i = 0; i < characterImageCard.Length; i++)
        {
            if (i < characterData.Length)
                characterImageCard[i].sprite = characterData[i].cardfull;
        }
        for (int i = 0; i < charaImageKarakter.Length; i++)
        {
            if (i < characterData.Length)
                charaImageKarakter[i].sprite = characterData[i].karakter;
        }
    }

    void DisplayCharacterNames()
    {
        for (int i = 0; i < charaRoletext.Length; i++)
        {
            if (i < characterData.Length)
                charaRoletext[i].text = characterData[i].role;
        }
        for (int i = 0; i < charaabilityAstext.Length; i++)
        {
            if (i < characterData.Length)
                charaabilityAstext[i].text = characterData[i].ability_as;
        }
        for (int i = 0; i < charaefektext.Length; i++)
        {
            if (i < characterData.Length)
                charaefektext[i].text = characterData[i].efek;
        }
    }



    void SetupButtons()
    {
        for (int i = 0; i < buttonListChara.Length; i++)
        {
            int idx = i;
            buttonListChara[i].onClick.AddListener(() => SelectCharacter(idx));
        }
    }

    void ResetPanels()
    {
        for (int i = 0; i < panelselectedChara.Length; i++)
            panelselectedChara[i].gameObject.SetActive(false);
    }

    void ResetLabels()
    {
        for (int i = 0; i < selectedLabel.Length; i++)
            selectedLabel[i].text = "";
    }



    public void SetActivePlayer(int p)
{
    currentPlayer = p;
    RefreshInteractable();
}


    void SelectCharacter(int charIndex)
    {

        for (int i = 0; i < characterOwner.Length; i++)
        {
            if (characterOwner[i] == currentPlayer)
            {
                UnlockCharacter(i);
                break;
            }
        }


        if (characterOwner[charIndex] != -1)
            return;

        LockCharacter(charIndex, currentPlayer);


        playerSlotUI.AdvanceTurn();


        RefreshInteractable();
    }



    void LockCharacter(int charIndex, int playerIndex)
    {
        characterOwner[charIndex] = playerIndex;

        panelselectedChara[charIndex].gameObject.SetActive(true);
        selectedLabel[charIndex].text = "Player " + (playerIndex + 1) + " Selected";
    }

    void UnlockCharacter(int charIndex)
    {
        characterOwner[charIndex] = -1;

        panelselectedChara[charIndex].gameObject.SetActive(false);
        selectedLabel[charIndex].text = "";
    }



    void RefreshInteractable()
    {
        for (int i = 0; i < buttonListChara.Length; i++)
        {
            bool free = characterOwner[i] == -1;
            bool mine = characterOwner[i] == currentPlayer;

            buttonListChara[i].interactable = free || mine;
        }
    }



    public int GetCharacterOfPlayer(int playerIndex)
    {
        for (int i = 0; i < characterOwner.Length; i++)
        {
            if (characterOwner[i] == playerIndex)
                return i;
        }
        return -1;
    }
}
