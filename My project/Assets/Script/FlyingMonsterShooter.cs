
using UnityEngine;

public class FlyingMonsterShooter : MonoBehaviour
{
    [Header("플레이어")]
    public Transform targetPlayer;

    [Header("총알 설정")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("발사 설정")]
    public float fireRate = 2f;
    public float attackRange = 20f;
    public float targetHeightOffset = 1f;

    [Header("애니메이션")]
    public Animator animator;
    public bool useAttackAnimation = false;
    public string attackTriggerName = "Attack";

    [Header("디버그")]
    public bool showDebugLogs = true;

    private Health health;
    private float nextFireTime;

    void Start()
    {
        health = GetComponent<Health>();

        FindPlayer();

        if (firePoint == null)
        {
            firePoint = transform.Find("FirePoint");
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
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

        if (projectilePrefab == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            targetPlayer.position
        );

        if (distance > attackRange)
            return;

        if (Time.time < nextFireTime)
            return;

        Shoot();

        nextFireTime = Time.time +
            Mathf.Max(0.01f, fireRate);
    }

    void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            targetPlayer = player.transform;
    }

    void Shoot()
    {
        // 총알 생성 위치
        Vector3 spawnPosition;

        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
        }
        else
        {
            spawnPosition =
                transform.position +
                transform.forward * 1.5f;
        }

        // 플레이어의 몸통 위치 계산
        Vector3 targetPosition =
            targetPlayer.position +
            Vector3.up * targetHeightOffset;

        // 발사 위치에서 플레이어까지의 방향
        Vector3 shootDirection =
            (targetPosition - spawnPosition).normalized;

        if (shootDirection.sqrMagnitude < 0.001f)
            return;

        // 플레이어 방향으로 회전
        Quaternion shootRotation =
            Quaternion.LookRotation(
                shootDirection,
                Vector3.up
            );

        // 총알 생성
        GameObject bulletObject = Instantiate(
            projectilePrefab,
            spawnPosition,
            shootRotation
        );

        // 총알에 발사 방향 직접 전달
        MonsterProjectile projectile =
            bulletObject.GetComponent<MonsterProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "몬스터 총알에 MonsterProjectile이 없습니다!"
            );

            Destroy(bulletObject);
            return;
        }

        projectile.Setup(
            shootDirection,
            transform
        );

        PlayAttackAnimation();

        if (showDebugLogs)
        {
            Debug.DrawLine(
                spawnPosition,
                targetPosition,
                Color.red,
                2f
            );

            Debug.Log(
                name + " 플레이어 방향으로 발사!"
            );
        }
    }

    void PlayAttackAnimation()
    {
        if (!useAttackAnimation || animator == null)
            return;

        foreach (
            AnimatorControllerParameter parameter
            in animator.parameters
        )
        {
            if (parameter.name == attackTriggerName &&
                parameter.type ==
                AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(attackTriggerName);
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (targetPlayer == null)
            return;

        Vector3 spawnPosition = firePoint != null
            ? firePoint.position
            : transform.position;

        Vector3 targetPosition =
            targetPlayer.position +
            Vector3.up * targetHeightOffset;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            spawnPosition,
            targetPosition
        );

        Gizmos.DrawWireSphere(
            targetPosition,
            0.2f
        );
    }
}
