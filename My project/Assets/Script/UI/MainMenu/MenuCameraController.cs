using System.Collections;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    [Header("카메라 & 기준 위치")]
    public Transform mainCamera;        // 이동할 실제 메인 카메라
    public Transform menuCamPoint;     // 메인 메뉴 기본 위치
    public Transform settingsCamPoint; // 설정 화면 카메라 위치

    [Header("이동 연출 설정")]
    public float moveDuration = 1.0f;  // 이동하는 데 걸리는 시간(초)
    // 시작과 끝을 부드럽게 감속해주는 곡선
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("여우 목 늘이기 제어 (선택 사항)")]
    public DeerFoxMenuController foxController; // 설정 창 갔을 때 목 늘이기 멈추려면 연결

    private Coroutine currentMoveCoroutine;

    void Start()
    {
        if (mainCamera == null && Camera.main != null)
            mainCamera = Camera.main.transform;
    }

    // [설정 버튼] 누르면 호출할 함수
    public void OnClickSettingsButton()
    {
        MoveCameraTo(settingsCamPoint);

        // 설정 창에서는 여우 목이 마우스 따라오는 걸 멈추고 싶다면 비활성화
        if (foxController != null)
            foxController.enabled = false;
    }

    // [설정 닫기 / 뒤로가기 버튼] 누르면 호출할 함수
    public void OnClickBackButton()
    {
        MoveCameraTo(menuCamPoint);

        // 메인 메뉴로 돌아오면 여우 목 조작 다시 활성화
        if (foxController != null)
            foxController.enabled = true;
    }

    public void MoveCameraTo(Transform targetPoint)
    {
        if (currentMoveCoroutine != null)
            StopCoroutine(currentMoveCoroutine);

        currentMoveCoroutine = StartCoroutine(SmoothMove(targetPoint));
    }

    private IEnumerator SmoothMove(Transform target)
    {
        Vector3 startPos = mainCamera.position;
        Quaternion startRot = mainCamera.rotation;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // EaseInOut 커브 적용으로 부드러운 가속/감속
            float curveT = easeCurve.Evaluate(t);

            mainCamera.position = Vector3.Lerp(startPos, target.position, curveT);
            mainCamera.rotation = Quaternion.Slerp(startRot, target.rotation, curveT);

            yield return null;
        }

        // 오차 보정 (최종 위치에 정확히 안착)
        mainCamera.position = target.position;
        mainCamera.rotation = target.rotation;
    }
}