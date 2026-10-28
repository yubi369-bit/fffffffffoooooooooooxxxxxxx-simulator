using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public bool isGameOver = false;
    public static GameManager instance;

    public AudioClip gtaDeadSFX;

    private void Awake()
    {
        instance = this;
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isGameOver = true;
        Time.timeScale = 0.15f;

        GameOverPanelManager.Instance.gameOverVolume.weight = 1f;

        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX(gtaDeadSFX);

        GameOverPanelManager.Instance.GameOverWait();
    }
}
