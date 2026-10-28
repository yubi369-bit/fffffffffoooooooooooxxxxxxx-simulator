
using UnityEngine;

public class FlyingMonsterAI : MonoBehaviour
{
    [Header("플레이어 설정")]
    public Transform targetPlayer;

    [Header("이동 설정")]
    public float moveSpeed = 7f;
    public float stoppingDistance = 2f;

    [Header("비행 설정")]
    public float flyHeightOffset = 3f;
    public float hoverAmplitude = 0.3f;
    public float hoverFrequency = 2f;

    [Header("회전 설정")]
    public float rotationSpeed = 0f;
    public Transform visualModel;
    public float modelYawOffset = 0f;

    [Header("디버그")]
    public bool showDebugLogs = false;

    private Health health;
    private Rigidbody rb;

    private float hoverOffset;
    private Quaternion initialVisualRotation;

    void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        FindPlayer();

        if (visualModel != null && visualModel != transform)
        {
            initialVisualRotation = visualModel.localRotation;
        }

        hoverOffset = Random.Range(0f, 100f);

        // 물리 충돌로 이동이 막히지 않도록 설정
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (health != null && health.isEnemyDead)
            return;

        if (targetPlayer == null)
        {
            FindPlayer();

            if (targetPlayer == null)
                return;
        }

        MoveToPlayer();
        LookAtPlayer();
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

    void MoveToPlayer()
    {
        Vector3 playerPosition = targetPlayer.position;
        Vector3 currentPosition = transform.position;

        Vector3 horizontalDirection =
            playerPosition - currentPosition;

        horizontalDirection.y = 0f;

        float horizontalDistance =
            horizontalDirection.magnitude;

        // 위아래 부유 효과
        float hoverY = Mathf.Sin(
            (Time.time + hoverOffset) * hoverFrequency
        ) * hoverAmplitude;

        float targetHeight =
            playerPosition.y + flyHeightOffset + hoverY;

        Vector3 movement = Vector3.zero;

        // 플레이어와 멀어지면 추적
        if (horizontalDistance > stoppingDistance)
        {
            movement = horizontalDirection.normalized *
                       moveSpeed * Time.deltaTime;

            // 정지 거리보다 더 가까이 접근하지 않도록 제한
            float allowedDistance =
                horizontalDistance - stoppingDistance;

            movement = Vector3.ClampMagnitude(
                movement,
                allowedDistance
            );
        }

        // 높이는 항상 플레이어 기준으로 조절
        float newY = Mathf.MoveTowards(
            currentPosition.y,
            targetHeight,
            moveSpeed * Time.deltaTime
        );

        Vector3 nextPosition =
            currentPosition + movement;

        nextPosition.y = newY;

        // 직접 위치 적용
        transform.position = nextPosition;

        if (showDebugLogs)
        {
            Debug.DrawLine(
                transform.position,
                playerPosition,
                Color.red
            );
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction =
            targetPlayer.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion lookRotation =
            Quaternion.LookRotation(direction);

        if (rotationSpeed <= 0f)
        {
            transform.rotation = lookRotation;
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (visualModel != null && visualModel != transform)
        {
            visualModel.localRotation =
                Quaternion.Euler(
                    0f,
                    modelYawOffset,
                    0f
                ) * initialVisualRotation;
        }
        else
        {
            transform.rotation *=
                Quaternion.Euler(
                    0f,
                    modelYawOffset,
                    0f
                );
        }
    }
}
