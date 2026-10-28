
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]
    public float lifeTime = 5f;
    public float hitRadius = 0.1f;

    [Header("충돌 설정")]
    public LayerMask hitLayers = ~0;

    private Vector3 direction;
    private float speed;
    private int damage;

    private bool initialized;
    private bool hasHit;

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

        transform.rotation = Quaternion.LookRotation(direction);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!initialized || hasHit)
            return;

        float distance = speed * Time.deltaTime;

        // 이동 경로의 충돌 확인
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

        // 총알 앞으로 이동
        transform.position += direction * distance;

        StartCoroutine(BulletDestroy(5f));
    }

    //5초 후에 투사체 파괴되도록 하는 코루틴
    IEnumerator BulletDestroy(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Destroy(gameObject);
    }

    void HandleHit(Collider target)
    {
        if (hasHit)
            return;

        hasHit = true;

        // 충돌한 오브젝트 또는 부모에서 Health 검색
        Health health = target.GetComponentInParent<Health>();

        if (health != null)
        {
            // 플레이어는 공격하지 않음
            if (!health.CompareTag("Player"))
            {
                health.TakeDamage(damage);

                Debug.Log(
                    "총알 명중: " + health.gameObject.name
                );
            }
        }

        // 충돌하면 총알 제거
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!initialized || hasHit)
            return;

        if (((1 << other.gameObject.layer) & hitLayers.value) == 0)
            return;

        HandleHit(other);
    }
}
