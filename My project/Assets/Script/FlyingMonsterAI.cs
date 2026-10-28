using UnityEngine;

public class FlyingMonsterAI : MonoBehaviour
{
    [Header("타겟 설정")]
    public Transform targetPlayer;          // 추적할 플레이어

    [Header("비행 설정")]
    public float moveSpeed = 5f;             // 이동 속도
    public float rotationSpeed = 5f;         // 회전 속도
    public float flyHeightOffset = 3f;       // 플레이어 머리 위 유지할 고도(높이)
    public float stoppingDistance = 2f;      // 플레이어와 유지할 최소 거리

    [Header("공중 부유 연출 (Swaying)")]
    public float hoverAmplitude = 0.5f;      // 위아래로 둥둥 떠다니는 높이
    public float hoverFrequency = 2f;        // 둥둥 떠다니는 속도

    private Rigidbody rb;
    private float startYOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 플레이어 자동 탐색 (태그 기준)
        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) targetPlayer = playerObj.transform;
        }

        // Rigidbody를 사용할 경우 중력 영향 해제
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; // 직접 Position을 제어할 경우 true
        }

        startYOffset = Random.Range(0f, 100f); // 몬스터마다 둥둥 거리는 박자 다르게 설정
    }

    void Update()
    {
        if (targetPlayer == null) return;

        FlyToTarget();
    }

    private void FlyToTarget()
    {
        // 1. 공중 목표 위치 계산 (플레이어 위치 + 공중 높이 + 위아래 둥둥거림 효과)
        float hoverY = Mathf.Sin((Time.time + startYOffset) * hoverFrequency) * hoverAmplitude;
        Vector3 targetPosition = targetPlayer.position + Vector3.up * (flyHeightOffset + hoverY);

        // 2. 거리가 멀 때만 플레이어 쪽으로 이동
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetPosition, 
                moveSpeed * Time.deltaTime
            );
        }

        // 3. 플레이어 쪽을 바라보도록 회전 (Y축 기준 회전 또는 바라보기)
        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}