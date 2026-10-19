using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("PlayScene");
    }
}
