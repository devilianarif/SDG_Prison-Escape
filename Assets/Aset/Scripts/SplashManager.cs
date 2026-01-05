using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        RequestCameraPermission();

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

    void RequestCameraPermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif
    }
}
