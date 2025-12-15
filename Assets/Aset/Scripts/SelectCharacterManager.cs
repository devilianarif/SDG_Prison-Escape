using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectCharacterManager : MonoBehaviour
{
    public PlayerState playerState;
    public CharacterButton characterButton;

    public Button Nextbtn;
    public Button startgame;
    public Button backAcc;

    public GameObject panelACC;

    public Image[] playerCharacterCard;
    public Image[] playerCharacter;
    public TMP_Text[] playerroletextchara;
    public TMP_Text[] playerroletext;
    public TMP_Text[] playerabilityastext;
    public TMP_Text[] playerefektext;


    public Sprite spriteNormal;
    public Sprite spriteActive;

    public float normalScale = 1f;
    public float activeScale = 1.12f;
    public float animSpeed = 10f;

    public string scene;

    public int[] finalCharacterForPlayer = new int[4];

    bool isAllReady = false;
    Vector3 targetScale;

    void Start()
    {
        panelACC.SetActive(false);

        Nextbtn.interactable = false;
        targetScale = Vector3.one * normalScale;

        Nextbtn.onClick.AddListener(OpenACC);
        startgame.onClick.AddListener(LoadSceneNow);
        backAcc.onClick.AddListener(CloseACC);
    }

    void Update()
    {
        CheckReady();
        AnimateStartButton();
    }

    void CheckReady()
    {
        bool complete = true;

        for (int p = 0; p < 4; p++)
        {
            int charIndex = characterButton.GetCharacterOfPlayer(p);

            if (charIndex == -1)
            {
                complete = false;
                break;
            }

            finalCharacterForPlayer[p] = charIndex;
        }

        isAllReady = complete;
        Nextbtn.interactable = isAllReady;

        if (!isAllReady)
            SetStartNormalVisual();
    }

    void SetStartNormalVisual()
    {
        Image img = Nextbtn.GetComponent<Image>();
        img.sprite = spriteNormal;
        targetScale = Vector3.one * normalScale;
    }

    void SetStartActiveVisual()
    {
        Image img = Nextbtn.GetComponent<Image>();
        img.sprite = spriteActive;
        targetScale = Vector3.one * activeScale;
    }

    void AnimateStartButton()
    {
        Nextbtn.transform.localScale = Vector3.Lerp(
            Nextbtn.transform.localScale,
            targetScale,
            Time.deltaTime * animSpeed
        );
    }

    // UI ACC
    void OpenACC()
    {

        if (!isAllReady) return;

        for (int i = 0; i < 4; i++)
            playerState.selectedCharacter[i] = finalCharacterForPlayer[i];

        playerState.ResetPlayerData();  // penting

        for (int i = 0; i < 4; i++)
        {
            int idx = finalCharacterForPlayer[i];
            CardChara data = characterButton.characterData[idx];

            playerState.players[i].playername = data.role;

            playerroletext[i].text = data.role;
            playerabilityastext[i].text = data.ability_as;
            playerefektext[i].text = data.efek;

        }


        FillACC();
        panelACC.SetActive(true);
    }


    void CloseACC()
    {
        panelACC.SetActive(false);
        SetStartNormalVisual();
    }
    void FillACC()
    {
        for (int i = 0; i < 4; i++)
        {
            int charIndex = finalCharacterForPlayer[i];

            CardChara data = characterButton.characterData[charIndex];

            playerCharacterCard[i].sprite = data.cardfull;
            playerCharacter[i].sprite = data.karakter;
            playerroletextchara[i].text = $"Player {i + 1} - {data.role}";
        }
    }


    void LoadSceneNow()
    {
        SceneManager.LoadScene(scene);
    }

}