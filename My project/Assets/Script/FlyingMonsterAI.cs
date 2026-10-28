
using UnityEngine;

public class FlyingMonsterAI : MonoBehaviour
{
    [Header("플레이어 설정")]
    [Tooltip("비워두면 Player 태그로 자동 검색")]
    public Transform targetPlayer;

    [Header("추적 설정")]
    public float moveSpeed = 7f;

    [Tooltip("플레이어와 유지할 수평 거리")]
    public float stoppingDistance = 2f;

    [Tooltip("거리 판정 여유값")]
    public float distanceTolerance = 0.1f;

    [Header("비행 설정")]
    public float flyHeightOffset = 3f;
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;

    [Header("회전 설정")]
    [Tooltip("0이면 즉시 플레이어를 바라봄")]
    public float rotationSpeed = 0f;

    [Tooltip("몬스터 외형 자식 오브젝트")]
    public Transform visualModel;

    [Tooltip("모델 정면 방향 보정")]
    public float modelYawOffset = 0f;

    [Header("디버그")]
    public bool showDebugLogs = false;

    private Rigidbody rb;
    private Health health;

    private float hoverOffset;

    private Quaternion initialModelRotation;
    private bool hasVisualModel;

    private Vector3 lastHorizontalDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        FindPlayer();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        hasVisualModel =
            visualModel != null &&
            visualModel != transform;

        if (hasVisualModel)
        {
            initialModelRotation = visualModel.localRotation;
        }

        hoverOffset = Random.Range(0f, 100f);

        lastHorizontalDirection = -transform.forward;
        lastHorizontalDirection.y = 0f;

        if (lastHorizontalDirection.sqrMagnitude < 0.001f)
            lastHorizontalDirection = Vector3.back;

        lastHorizontalDirection.Normalize();
    }

    void Update()
    {
        // 사망한 몬스터는 추적 중단
        if (health != null && health.isEnemyDead)
            return;

        if (targetPlayer == null)
        {
            FindPlayer();

            if (targetPlayer == null)
                return;
        }

        FollowPlayer();
        FacePlayer();
    }

    void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            targetPlayer = player.transform;
        }
    }

    void FollowPlayer()
    {
        Vector3 playerPosition = targetPlayer.position;
        Vector3 monsterPosition = transform.position;

        // 수평 거리 계산
        Vector3 horizontalDifference =
            monsterPosition - playerPosition;

        horizontalDifference.y = 0f;

        float horizontalDistance =
            horizontalDifference.magnitude;

        if (horizontalDistance > 0.001f)
        {
            lastHorizontalDirection =
                horizontalDifference.normalized;
        }

        // 몬스터가 플레이어와 유지할 위치
        Vector3 desiredPosition =
            playerPosition +
            lastHorizontalDirection *
            Mathf.Max(0f, stoppingDistance);

        // 공중 부유 효과
        float hoverY = Mathf.Sin(
            (Time.time + hoverOffset) * hoverFrequency
        ) * hoverAmplitude;

        desiredPosition.y =
            playerPosition.y +
            flyHeightOffset +
            hoverY;

        // 매 프레임 목표 위치를 갱신
        float distanceToDesired =
            Vector3.Distance(
                transform.position,
                desiredPosition
            );

        if (distanceToDesired > distanceTolerance)
        {
            Vector3 newPosition =
                Vector3.MoveTowards(
                    transform.position,
                    desiredPosition,
                    moveSpeed * Time.deltaTime
                );

            if (rb != null && rb.isKinematic)
            {
                rb.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }
        }

        if (showDebugLogs)
        {
            Debug.DrawLine(
                transform.position,
                desiredPosition,
                Color.green
            );

            Debug.DrawLine(
                transform.position,
                playerPosition,
                Color.red
            );
        }
    }

    void FacePlayer()
    {
        Vector3 direction =
            targetPlayer.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion lookRotation =
            Quaternion.LookRotation(
                direction.normalized,
                Vector3.up
            );

        Quaternion correctedRotation =
            lookRotation;

        if (hasVisualModel)
        {
            visualModel.localRotation =
                Quaternion.Euler(
                    0f,
                    modelYawOffset,
                    0f
                ) * initialModelRotation;
        }
        else
        {
            correctedRotation *=
                Quaternion.Euler(
                    0f,
                    modelYawOffset,
                    0f
                );
        }

        if (rotationSpeed <= 0f)
        {
            transform.rotation = correctedRotation;
        }
        else
        {
            transform.rotation =
                Quaternion.RotateTowards(
                    transform.rotation,
                    correctedRotation,
                    rotationSpeed * Time.deltaTime
                );
        }
    }

    void OnDrawGizmosSelected()
    {
        if (targetPlayer == null)
            return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            targetPlayer.position,
            stoppingDistance
        );

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            transform.position,
            targetPlayer.position
        );
    }
}
