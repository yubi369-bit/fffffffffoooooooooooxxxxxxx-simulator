using UnityEngine;

public class FlyingMonsterShooter : MonoBehaviour
{
    [Header("사격 대상 설정")]
    public Transform targetPlayer;          // 사격 대상 (플레이어)

    [Header("사격 속성")]
    public GameObject projectilePrefab;     // 발사할 투사체 프리팹
    public Transform firePoint;             // 총알이 발사될 위치 (입, 손, 총구 등)
    public float fireRate = 2f;             // 발사 간격 (초 단위)
    public float attackRange = 15f;         // 사격 가능 거리

    [Header("시각 연출 (선택 사항)")]
    public Animator animator;
    public string attackTriggerName = "Attack"; // 공격 애니메이션 트리거 이름

    private float nextFireTime = 0f;

    void Start()
    {
        // 플레이어 자동 탐색
        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) 
                targetPlayer = playerObj.transform;
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (targetPlayer == null || projectilePrefab == null || firePoint == null) 
            return;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        // 공격 범위 내에 들어오고 발사 쿨타임이 채워졌을 때
        if (distance <= attackRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        // 공격 애니메이션 실행
        if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
        {
            animator.SetTrigger(attackTriggerName);
        }

        // 투사체 생성
        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // 플레이어의 중앙(가슴/몸통)을 겨냥하도록 타겟 위치 조절
        Vector3 targetPosition = targetPlayer.position + Vector3.up * 1f;
        Vector3 shootDirection = (targetPosition - firePoint.position).normalized;

        // 생성된 투사체에 날아가는 방향 전달
        MonsterProjectile projectile = projectileObj.GetComponent<MonsterProjectile>();
        if (projectile != null)
        {
            projectile.Setup(shootDirection);
        }
    }

    // 에디터 씬 뷰에서 공격 사거리 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}