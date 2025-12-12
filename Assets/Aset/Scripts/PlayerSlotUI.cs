using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSlotUI : MonoBehaviour
{
    public Button[] buttonList;
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

    void Start()
    {
        SetupButtons();
        SetPlayerNames();

        targetScale = new Vector3[buttonList.Length];
        for (int i = 0; i < buttonList.Length; i++)
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
        for (int i = 0; i < buttonList.Length; i++)
        {
            int index = i;
            buttonList[i].onClick.AddListener(() => SelectPlayer(index));
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

        if (characterButton != null)
            characterButton.SetActivePlayer(index);

        UpdateUI();


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

    void AnimateScale()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].transform.localScale = Vector3.Lerp(
                buttonList[i].transform.localScale,
                targetScale[i],
                Time.deltaTime * animSpeed
            );
        }
    }


}