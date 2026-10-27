using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("메인 UI 버튼들")]
    public Image logoImage;
    public GameObject startButton;
    public GameObject settingButton;


    public GameObject settingsPanel;

    public AudioClip mainMenuBGM;

    public AudioClip OpenSettingSFX;
    public AudioClip CloseSettingSFX;

    public static MainMenuUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        AudioManager.Instance.PlayBGM(mainMenuBGM);
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnSettingButtonClicked()
    {
        if (logoImage != null)
        {
            logoImage.gameObject.SetActive(false);
        }

        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        if (settingButton != null)
        {
            settingButton.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnBackButtonClicked()
    {
        if (logoImage != null)
        {
            logoImage.gameObject.SetActive(true);
        }
        if (startButton != null)
        {
            startButton.SetActive(true);
        }
        if (settingButton != null)
        {
            settingButton.SetActive(true);
        }
    }

    //설정창 열었을떄 들리는 효과음
    public void OpenSettingSFXPlay()
    {
        AudioManager.Instance.PlaySFX(OpenSettingSFX);
    }

    //설정창 닫았을떄 들리는 효과음
    public void CloseSttingSFXPlay()
    {
        AudioManager.Instance.PlaySFX(CloseSettingSFX);
    }
}
