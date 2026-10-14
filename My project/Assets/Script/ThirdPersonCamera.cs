using UnityEngine;
using UnityEngine.InputSystem; // New Input System 네임스페이스 필수

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("타겟")]
    [SerializeField] private Transform target; // 따라갈 캐릭터 Transform

    [Header("거리 와 오프셋")]
    [SerializeField] private float defaultDistance = 5.0f; // 기본 카메라 거리
    [SerializeField] private float minDistance = 0.5f;     // 장애물 충돌 시 최소 거리
    [SerializeField] private float height = 2.0f;          // 캐릭터 머리 위 높이 오프셋
    [SerializeField] private float smoothTime = 0.12f;    // 카메라 이동 부드러움 정도

    [Header("회전 제한")]
    [SerializeField] private float mouseSensitivity = 1.0f; // 마우스 감도 
    [SerializeField] private float minPitch = -20.0f;       // 아래쪽 회전 제한
    [SerializeField] private float maxPitch = 80.0f;        // 위쪽 회전 제한

    [Header("장애물 인식")]
    [SerializeField] private LayerMask obstacleLayers;      // 장애물로 인식할 레이어 
    [SerializeField] private float cameraRadius = 0.2f;     // 카메라 자체 두께 

    private float currentYaw = 0.0f;   // X축 회전각 (좌우)
    private float currentPitch = 0.0f; // Y축 회전각 (상하)
    private Vector3 currentVelocity;

    private void Start()
    {
        // 마우스 커서 잠금 및 숨김 (필요 시 주석 해제)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        currentPitch = angles.x;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. New Input System을 통한 마우스 델타(이동량) 입력 수집
        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
        {
            // Mouse.current.delta.ReadValue()는 픽셀 단위 델타를 반환하므로 적절히 감도를 곱함
            mouseDelta = Mouse.current.delta.ReadValue() * 0.1f;
        }

        currentYaw += mouseDelta.x * mouseSensitivity;
        currentPitch -= mouseDelta.y * mouseSensitivity;

        // 상하 회전각 제한
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        // 2. 회전 및 타겟 위치 계산
        Quaternion targetRotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 targetPosition = target.position + Vector3.up * height;

        // 3. 장애물 충돌 검사 (SphereCast)
        Vector3 desiredCameraDir = targetRotation * -Vector3.forward;
        float targetDistance = defaultDistance;

        if (Physics.SphereCast(targetPosition, cameraRadius, desiredCameraDir, out RaycastHit hit, defaultDistance, obstacleLayers))
        {
            targetDistance = Mathf.Clamp(hit.distance, minDistance, defaultDistance);
        }

        // 4. 최종 목표 위치 계산 및 적용
        Vector3 desiredPosition = targetPosition + desiredCameraDir * targetDistance;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
        transform.LookAt(targetPosition);
    }
}