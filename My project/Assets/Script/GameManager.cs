using UnityEngine;

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
        Time.timeScale = 0f;

        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX(gtaDeadSFX);
    }
}
