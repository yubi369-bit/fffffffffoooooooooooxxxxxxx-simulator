
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 2f;
    public float detectRange = 15f;
    public float attackRange = 1.5f;

    [Header("Y축 고정")]
    [Tooltip("몬스터 Y좌표를 항상 이 값으로 고정")]
    public float fixedY = 0f;

    [Header("회전 설정")]
    public Transform visualModel;

    public ModelForwardDirection modelForward =
        ModelForwardDirection.ForwardZ;

    [Tooltip("0이면 즉시 회전")]
    public float rotationSpeed = 0f;

    [Header("공격 설정")]
    public int damage = 1;
    public float attackCooldown = 1f;

    public enum ModelForwardDirection
    {
        ForwardZ,
        BackwardZ,
        RightX,
        LeftX
    }

    private Health health;
    private BoxCollider boxCollider;
    private Rigidbody rb;

    private Transform player;
    private float lastAttackTime = -999f;

    private Quaternion initialVisualRotation;
    private bool hasVisualModel;

    void Awake()
    {
        health = GetComponent<Health>();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        // 물리 엔진이 Y좌표를 변경하지 못하도록 설정
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 시작부터 Y좌표를 0으로 고정
        FixYPosition();
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

        FixYPosition();
    }

    void Update()
    {
        // 매 프레임 Y좌표 고정
        FixYPosition();

        // 사망 시 기존 처리 유지
        if (health != null && health.isEnemyDead)
        {
            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }

            return;
        }

        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        // 플레이어의 X, Z 좌표만 사용
        Vector3 targetPosition = new Vector3(
            player.position.x,
            fixedY,
            player.position.z
        );

        Vector3 currentPosition = new Vector3(
            transform.position.x,
            fixedY,
            transform.position.z
        );

        Vector3 toPlayer =
            targetPosition - currentPosition;

        float distance = toPlayer.magnitude;

        if (distance > detectRange)
            return;

        if (distance < 0.001f)
            return;

        Vector3 direction = toPlayer.normalized;

        // 플레이어 쪽을 바라보기
        RotateTowardsPlayer(direction);

        // 플레이어 추적
        if (distance > attackRange)
        {
            float moveDistance = Mathf.Min(
                moveSpeed * Time.deltaTime,
                distance - attackRange
            );

            Vector3 nextPosition =
                currentPosition +
                direction * moveDistance;

            // 이동 후에도 Y좌표는 0
            nextPosition.y = fixedY;

            transform.position = nextPosition;
        }
        else
        {
            // 기존 근접 공격
            if (Time.time >=
                lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;

                Health playerHealth =
                    player.GetComponent<Health>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }

    void LateUpdate()
    {
        // 다른 스크립트가 Y좌표를 변경해도 다시 고정
        FixYPosition();
    }

    void FindPlayer()
    {
        GameObject p =
            GameObject.FindGameObjectWithTag("Player");

        if (p != null)
        {
            player = p.transform;
        }
    }

    void FixYPosition()
    {
        Vector3 position = transform.position;

        position.y = fixedY;

        transform.position = position;
    }

    void RotateTowardsPlayer(Vector3 direction)
    {
        Quaternion lookRotation =
            Quaternion.LookRotation(
                direction,
                Vector3.up
            );

        float correctionAngle = GetCorrectionAngle();

        if (hasVisualModel)
        {
            ApplyRotation(transform, lookRotation);

            visualModel.localRotation =
                Quaternion.Euler(
                    0f,
                    correctionAngle,
                    0f
                ) * initialVisualRotation;
        }
        else
        {
            Quaternion correctedRotation =
                lookRotation *
                Quaternion.Euler(
                    0f,
                    correctionAngle,
                    0f
                );

            ApplyRotation(
                transform,
                correctedRotation
            );
        }
    }

    void ApplyRotation(
        Transform target,
        Quaternion rotation
    )
    {
        if (rotationSpeed <= 0f)
        {
            target.rotation = rotation;
        }
        else
        {
            transform.rotation =
                Quaternion.RotateTowards(
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
}
