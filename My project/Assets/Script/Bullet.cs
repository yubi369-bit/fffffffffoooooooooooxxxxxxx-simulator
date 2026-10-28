
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]
    [Tooltip("총알이 자동으로 사라지는 시간 (초)")]
    public float lifeTime = 10f;

    [Tooltip("총알 충돌 판정 반지름")]
    public float hitRadius = 0.1f;

    [Header("충돌 설정")]
    public LayerMask hitLayers = ~0;

    private Vector3 direction;
    private float speed;
    private int damage;

    private bool initialized = false;
    private bool hasHit = false;

    // 총알 발사 시 초기 설정
    public void Initialize(
        Vector3 shootDirection,
        float bulletSpeed,
        int bulletDamage
    )
    {
        direction = shootDirection.normalized;
        speed = bulletSpeed;
        damage = bulletDamage;

        initialized = true;

        // 발사 방향으로 총알 회전
        transform.rotation =
            Quaternion.LookRotation(direction);

        // 발사 후 10초가 지나면 자동 삭제
        Destroy(gameObject, Mathf.Max(0.01f, lifeTime));
    }

    void Update()
    {
        if (!initialized || hasHit)
            return;

        // 총알 이동 거리 계산
        float distance = speed * Time.deltaTime;

        // 이동 경로 충돌 감지
        if (Physics.SphereCast(
            transform.position,
            hitRadius,
            direction,
            out RaycastHit hit,
            distance,
            hitLayers,
            QueryTriggerInteraction.Ignore
        ))
        {
            transform.position = hit.point;

            HandleHit(hit.collider);
            return;
        }

        // 총알 이동
        transform.position += direction * distance;
    }

    void HandleHit(Collider target)
    {
        if (hasHit)
            return;

        hasHit = true;

        // 충돌한 적의 Health 검색
        Health health =
            target.GetComponentInParent<Health>();

        if (health != null)
        {
            // 플레이어 자신에게는 데미지를 주지 않음
            if (!health.CompareTag("Player"))
            {
                health.TakeDamage(damage);

                Debug.Log(
                    "총알 명중: " +
                    health.gameObject.name
                );
            }
        }

        // 충돌하면 즉시 총알 삭제
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!initialized || hasHit)
            return;

        // 설정된 충돌 레이어인지 확인
        if (((1 << other.gameObject.layer) &
             hitLayers.value) == 0)
        {
            return;
        }

        HandleHit(other);
    }
}
