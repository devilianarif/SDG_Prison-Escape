using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    public Button btnStart;
    public Button btnCredit;
    public Button btnBackFromCredit;

    public GameObject panelCredit;
    public string scene;

    void Start()
    {
        panelCredit.SetActive(false);

        btnStart.onClick.AddListener(StartGame);
        btnCredit.onClick.AddListener(OpenCredit);
        btnBackFromCredit.onClick.AddListener(CloseCredit);
    }

    void StartGame()
    {
        SceneManager.LoadScene(scene);
    }

    void OpenCredit()
    {
        panelCredit.SetActive(true);
    }

    void CloseCredit()
    {
        panelCredit.SetActive(false);
    }


}