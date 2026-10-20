using UnityEngine;
using UnityEngine.InputSystem;

public class DeerFoxMenuController : MonoBehaviour
{
    [Header("연결할 오브젝트")]
    public Transform neckBase;
    public Transform head;
    public Transform neck;

    [Header("카메라 & 마우스 설정")]
    public Camera mainCamera;
    public float followSpeed = 10f;
    public float maxDistance = 15f;
    public float minDistance = 0.5f;

    [Header("목 두께 설정")]
    public float neckWidth = 0.4f;
    public float neckHeight = 0.4f;

    [Header("머리 회전 보정")]
    // 얼굴이 반대로 뒤집혀 있으므로 Y축에 180도를 기본으로 줌
    public Vector3 headRotationOffset = new Vector3(0, 180f, 0);

    private Plane mousePlane;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (neckBase != null)
        {
            mousePlane = new Plane(-mainCamera.transform.forward, neckBase.position);
        }
    }

    void Update()
    {
        StretchTowardsMouse();
    }

    void StretchTowardsMouse()
    {
        if (Mouse.current == null || neckBase == null || head == null) return;

        Plane mousePlane = new Plane(-mainCamera.transform.forward, neckBase.position);

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
        Vector3 targetWorldPos = head.position;

        if (mousePlane.Raycast(ray, out float enterDistance))
        {
            targetWorldPos = ray.GetPoint(enterDistance);
        }

        Vector3 direction = targetWorldPos - neckBase.position;
        float distance = Mathf.Clamp(direction.magnitude, minDistance, maxDistance);
        Vector3 clampedTargetPos = neckBase.position + direction.normalized * distance;

        // 머리 위치 이동
        head.position = Vector3.Lerp(head.position, clampedTargetPos, Time.deltaTime * followSpeed);

        // 머리 회전 (오프셋 각도를 적용하여 방향 바로잡기)
        if (direction != Vector3.zero)
        {
            head.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(headRotationOffset);
        }

        // 목 큐브 늘리기
        if (neck != null)
        {
            neck.position = (neckBase.position + head.position) * 0.5f;
            neck.rotation = Quaternion.LookRotation(head.position - neckBase.position);

            float currentLength = Vector3.Distance(neckBase.position, head.position);
            neck.localScale = new Vector3(neckWidth, neckHeight, currentLength);
        }
    }
}