using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanelManager : MonoBehaviour
{
    public GameObject gameOverPanel;

    private void Update()
    {
        if (GameManager.instance.isGameOver == false)
        {
            gameOverPanel.SetActive (false);
        }
        
        if (GameManager.instance.isGameOver)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void ClickGameOverPanel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.instance.isGameOver = false;

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenuScene");
    }
}
