using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{

    public static SceneTransitionManager Instance;


    [Header("전환용 UI")]
    public RectTransform transitionImage;
    public float scaleUpDuration = 0.5f;   // 화면 덮는 시간
    public float targetScale = 30f;

    [Header("새 씬 진입 후 옵션")]
    public bool shrinkAfterLoad = true;    // 새 씬 로드 후 다시 작아지면서 열릴지 여부
    public float scaleDownDuration = 0.5f;

    private void Awake()
    {
        // 싱글톤 적용 & 씬이 바뀌어도 파괴되지 않게 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 넘어가도 Canvas가 살아있음!
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 시작 시에는 비활성화
        if (transitionImage != null)
        {
            transitionImage.localScale = Vector3.zero;
            transitionImage.gameObject.SetActive(false);
        }
    }


    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. 이미지 활성화 및 스케일 0 초기화
        transitionImage.gameObject.SetActive(true);
        transitionImage.localScale = Vector3.zero;

        // 2. 스케일이 커지며 화면 덮기
        float timer = 0f;
        while (timer < scaleUpDuration)
        {
            timer += Time.unscaledDeltaTime; // TimeScale이 0이어도 작동하도록 unscaled 사용
            float t = timer / scaleUpDuration;
            // 부드럽게 커지도록 EaseInOut 보간
            t = Mathf.SmoothStep(0f, 1f, t);

            transitionImage.localScale = Vector3.one * (t * targetScale);
            yield return null;
        }
        transitionImage.localScale = Vector3.one * targetScale;

        // 3. 화면이 완전히 덮인 상태에서 백그라운드로 새 씬 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 씬 로딩이 100% 완료될 때까지 대기 (이 동안 UI는 유지됨)
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 4. 새 씬 로딩 완료 후
        if (shrinkAfterLoad)
        {
            // 다시 작아지면서 새 화면을 드러냄
            timer = 0f;
            while (timer < scaleDownDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = timer / scaleDownDuration;
                t = Mathf.SmoothStep(0f, 1f, t);

                transitionImage.localScale = Vector3.one * Mathf.Lerp(targetScale, 0f, t);
                yield return null;
            }
        }

        // 5. 완료 후 UI 비활성화
        transitionImage.localScale = Vector3.zero;
        transitionImage.gameObject.SetActive(false);
    }
}
