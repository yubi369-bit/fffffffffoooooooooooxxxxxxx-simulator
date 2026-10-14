
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleManager : MonoBehaviour
{
    [Header("이동할 씬")]

#if UNITY_EDITOR
    [Tooltip("이동할 씬 파일을 여기에 드래그하세요")]
    public SceneAsset targetScene;
#endif

    [SerializeField, HideInInspector]
    private string targetScenePath = "";

    [Header("클릭 설정")]
    public float inputDelay = 0.2f;

    private float startTime;
    private bool isLoading = false;

    void Start()
    {
        Time.timeScale = 1f;

        startTime = Time.unscaledTime;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Inspector에서 씬을 넣으면 경로 자동 저장
        if (targetScene != null)
        {
            targetScenePath = AssetDatabase.GetAssetPath(
                targetScene
            );
        }
        else
        {
            targetScenePath = "";
        }
    }
#endif

    void Update()
    {
        if (isLoading)
            return;

        if (Time.unscaledTime - startTime < inputDelay)
            return;

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (isLoading)
            return;

        if (string.IsNullOrEmpty(targetScenePath))
        {
            Debug.LogError(
                "TitleManager: 이동할 씬을 연결해주세요!"
            );
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetScenePath))
        {
            Debug.LogError(
                "씬을 찾을 수 없습니다. Build Profiles의 Scene List에 추가해주세요: "
                + targetScenePath
            );
            return;
        }

        isLoading = true;

        Time.timeScale = 1f;

        SceneManager.LoadScene(targetScenePath);
    }
}
