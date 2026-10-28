using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("메인 UI 버튼들")]
    public Image logoImage;
    public GameObject startButton;
    public GameObject settingButton;
    public GameObject quitButton;


    public GameObject settingsPanel;
    public GameObject quitPanel;
    public GameObject realQuitPanel;
    public GameObject realRealQuitPanel;

    public AudioClip mainMenuBGM;
    public AudioClip battleBGM;

    public AudioClip OpenSettingSFX;
    public AudioClip CloseSettingSFX;
    public AudioClip clickBTSound;

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

        if (quitPanel != null)
        {
            quitPanel.SetActive(false);
        }
        if (realQuitPanel != null)
        { 
            realQuitPanel.SetActive(false); 
        }
        if (realRealQuitPanel != null)
        {
            realRealQuitPanel.SetActive(false);
        }

        AudioManager.Instance.PlayBGM(mainMenuBGM);
    }

    public void OnStartButtonClicked()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayBGM(battleBGM);

        GameManager.instance.isGameOver = false;
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

        if (quitButton != null)
        {
            quitButton.SetActive(false);
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
        if (quitButton != null)
        {
            quitButton.SetActive(true);
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

    public void AllButtonClickSFX()
    {
        AudioManager.Instance.PlaySFX(clickBTSound);
    }




    public void ClickOpenQuitPanel()
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

        if (quitButton != null)
        {
            quitButton.SetActive(false);
        }

        if (quitPanel != null)
        {
            quitPanel.SetActive(true);
        }
    }

    public void ClickCloseQuitPanel()
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

        if (quitButton != null)
        {
            quitButton.SetActive(true);
        }

        if (quitPanel != null)
        {
            quitPanel.SetActive(false);
        }

        if (realQuitPanel != null)
        {
            realQuitPanel.SetActive(false);
        }

        if (realRealQuitPanel != null)
        {
            realRealQuitPanel.SetActive(false);
        }
    }

    public void RealQuitPanelOpen()
    {
        if (realQuitPanel != null)
        {
            realQuitPanel.gameObject.SetActive(true);
        }
        if (quitPanel != null)
        {
            quitPanel.gameObject.SetActive(false);
        }
    }

    public void RealRealQuitPanelOpen()
    {
        if (realQuitPanel != null)
        {
            realQuitPanel.gameObject.SetActive(false);
        }
        if (realRealQuitPanel != null)
        {
            realRealQuitPanel.gameObject.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
