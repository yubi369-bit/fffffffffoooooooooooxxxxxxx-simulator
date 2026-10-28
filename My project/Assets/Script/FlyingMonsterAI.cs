
using UnityEngine;

public class FlyingMonsterAI : MonoBehaviour
{
    [Header("타겟 설정")]
    public Transform targetPlayer;

    [Header("감지 설정")]
    public float detectRange = 15f;
    public bool rememberPlayer = true;

    [SerializeField]
    private bool hasDetectedPlayer = false;

    [Header("이동 설정")]
    public float moveSpeed = 7f;
    public float stoppingDistance = 2f;

    [Header("비행 높이 설정")]
    public float fixedFlyHeight = 3f;
    public float hoverAmplitude = 0.3f;
    public float hoverFrequency = 2f;

    [Header("회전 설정")]
    [Tooltip("캐릭터 외형 오브젝트를 넣으세요.")]
    public Transform visualModel;

    [Tooltip("캐릭터 모델의 정면 방향")]
    public ModelForwardDirection modelForward =
        ModelForwardDirection.ForwardZ;

    [Tooltip("0이면 즉시 회전, 그 이상이면 부드럽게 회전")]
    public float rotationSpeed = 0f;

    public enum ModelForwardDirection
    {
        ForwardZ,
        BackwardZ,
        RightX,
        LeftX
    }

    private Health health;
    private Rigidbody rb;

    private float hoverOffset;
    private Quaternion initialVisualRotation;
    private bool hasVisualModel;

    void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    void Start()
    {
        FindPlayer();

        hasVisualModel =
            visualModel != null &&
            visualModel != transform;

        if (hasVisualModel)
        {
            initialVisualRotation =
                visualModel.localRotation;
        }

        hoverOffset = Random.Range(0f, 100f);
        hasDetectedPlayer = false;
    }

    void Update()
    {
        if (health != null && health.isEnemyDead)
            return;

        if (targetPlayer == null)
        {
            FindPlayer();

            if (targetPlayer == null)
            {
                MaintainFlyHeight();
                return;
            }
        }

        Vector3 toPlayer =
            targetPlayer.position - transform.position;

        // 플레이어의 Y축 높이는 추적에 사용하지 않음
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        // 플레이어 최초 감지
        if (!hasDetectedPlayer &&
            distance <= detectRange)
        {
            hasDetectedPlayer = true;
        }

        if (!hasDetectedPlayer)
        {
            MaintainFlyHeight();
            return;
        }

        // 한 번 감지하면 계속 기억
        if (!rememberPlayer && distance > detectRange)
        {
            hasDetectedPlayer = false;
            MaintainFlyHeight();
            return;
        }

        // 이동
        MoveToPlayer(toPlayer, distance);

        // 스티브와 동일한 회전 코드 실행
        Vector3 lookDirection =
            targetPlayer.position - transform.position;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.000001f)
        {
            RotateTowardsPlayer(
                lookDirection.normalized
            );
        }
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

    void MoveToPlayer(Vector3 toPlayer, float distance)
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = currentPosition;

        if (distance > stoppingDistance &&
            distance > 0.001f)
        {
            float move = Mathf.Min(
                moveSpeed * Time.deltaTime,
                distance - stoppingDistance
            );

            nextPosition += toPlayer.normalized * move;
        }

        float hoverY = Mathf.Sin(
            (Time.time + hoverOffset) * hoverFrequency
        ) * hoverAmplitude;

        float targetHeight = fixedFlyHeight + hoverY;

        nextPosition.y = Mathf.MoveTowards(
            currentPosition.y,
            targetHeight,
            moveSpeed * Time.deltaTime
        );

        transform.position = nextPosition;
    }

    void MaintainFlyHeight()
    {
        Vector3 position = transform.position;

        float hoverY = Mathf.Sin(
            (Time.time + hoverOffset) * hoverFrequency
        ) * hoverAmplitude;

        position.y = Mathf.MoveTowards(
            position.y,
            fixedFlyHeight + hoverY,
            moveSpeed * Time.deltaTime
        );

        transform.position = position;
    }

    // ===================================
    // EnemyChaser에서 가져온 회전 기능
    // ===================================

    void RotateTowardsPlayer(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(
            direction,
            Vector3.up
        );

        float correctionAngle = GetCorrectionAngle();

        if (hasVisualModel)
        {
            // 부모는 플레이어 방향을 바라봄
            ApplyRotation(transform, lookRotation);

            // 모델의 실제 정면 방향 보정
            visualModel.localRotation =
                Quaternion.Euler(0f, correctionAngle, 0f)
                * initialVisualRotation;
        }
        else
        {
            // 모델이 부모 자체인 경우
            Quaternion correctedRotation =
                lookRotation *
                Quaternion.Euler(0f, correctionAngle, 0f);

            ApplyRotation(transform, correctedRotation);
        }
    }

    void ApplyRotation(Transform target, Quaternion rotation)
    {
        if (rotationSpeed <= 0f)
        {
            // 즉시 플레이어 정면 바라보기
            target.rotation = rotation;
        }
        else
        {
            // 부드럽게 회전
            target.rotation = Quaternion.RotateTowards(
                target.rotation,
                rotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    float GetCorrectionAngle()
    {
        switch (modelForward)
        {
            case ModelForwardDirection.BackwardZ:
                return 180f;

            case ModelForwardDirection.RightX:
                return -90f;

            case ModelForwardDirection.LeftX:
                return 90f;

            default:
                return 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            detectRange
        );

        if (targetPlayer != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                transform.position,
                targetPlayer.position
            );
        }
    }
}
