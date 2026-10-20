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
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}
