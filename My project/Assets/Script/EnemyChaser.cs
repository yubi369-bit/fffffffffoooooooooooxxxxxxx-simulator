
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 2f;
    public float detectRange = 15f;
    public float attackRange = 1.5f;

    [Header("회전 설정")]
    [Tooltip("캐릭터 외형 오브젝트를 넣으세요.")]
    public Transform visualModel;

    [Tooltip("캐릭터 모델의 정면 방향")]
    public ModelForwardDirection modelForward = ModelForwardDirection.ForwardZ;

    [Tooltip("0이면 즉시 회전, 그 이상이면 부드럽게 회전")]
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

    private Transform player;
    private float lastAttackTime = -999f;

    private Quaternion initialVisualRotation;
    private bool hasVisualModel;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");

        if (p != null)
        {
            player = p.transform;
        }

        hasVisualModel =
            visualModel != null && visualModel != transform;

        if (hasVisualModel)
        {
            initialVisualRotation = visualModel.localRotation;
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;

        // Y축 높이 차이는 무시
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        // 감지 범위를 벗어나면 대기
        if (distance > detectRange) return;

        if (distance < 0.001f) return;

        Vector3 direction = toPlayer.normalized;

        // 플레이어 방향으로 회전
        RotateTowardsPlayer(direction);

        // 플레이어 추적
        if (distance > attackRange)
        {
            float move = Mathf.Min(
                moveSpeed * Time.deltaTime,
                distance - attackRange
            );

            transform.position += direction * move;
        }
        else
        {
            // 플레이어 공격
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;

                Health health = player.GetComponent<Health>();

                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }
    }

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
}
