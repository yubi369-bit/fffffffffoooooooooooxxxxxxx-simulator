using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameOverPanelManager : MonoBehaviour
{
    public CinemachineCamera deathCamera;


    public GameObject gameOverPanel;

    public Volume gameOverVolume;

    public static GameOverPanelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver == false)
        {
            gameOverPanel.SetActive (false);
        }
    }

    public void ClickGameOverPanel()
    {
        deathCamera.Priority = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.instance.isGameOver = false;

        Time.timeScale = 1f;

        gameOverVolume.weight = 0f;

        SceneManager.LoadScene("MainMenuScene");
    }

    public void GameOverWait()
    {
        StartCoroutine(ShowPanelAnimation(2.8f));
    }

    IEnumerator ShowPanelAnimation(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (GameManager.instance.isGameOver)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}
