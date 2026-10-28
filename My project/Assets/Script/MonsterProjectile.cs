
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    [Header("총알 설정")]
    public float moveSpeed = 12f;
    public int damage = 1;
    public float lifeTime = 10f;

    [Header("충돌 설정")]
    public float hitRadius = 0.15f;
    public LayerMask hitLayers = ~0;

    private Vector3 moveDirection;
    private Transform owner;

    private bool initialized;
    private bool hasHit;

    public void Setup(Vector3 direction)
    {
        Setup(direction, null);
    }

    public void Setup(
        Vector3 direction,
        Transform shooter
    )
    {
        moveDirection = direction.normalized;
        owner = shooter;

        initialized = true;
        hasHit = false;

        if (moveDirection.sqrMagnitude < 0.001f)
        {
            moveDirection = Vector3.forward;
        }

        transform.rotation =
            Quaternion.LookRotation(moveDirection);

        Destroy(
            gameObject,
            Mathf.Max(0.01f, lifeTime)
        );
    }

    void Update()
    {
        if (!initialized || hasHit)
            return;

        float moveDistance =
            moveSpeed * Time.deltaTime;

        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            Mathf.Max(0.001f, hitRadius),
            moveDirection,
            moveDistance,
            hitLayers,
            QueryTriggerInteraction.Ignore
        );

        float closestDistance = float.MaxValue;
        Collider closestCollider = null;

        foreach (RaycastHit hit in hits)
        {
            if (IsOwner(hit.collider.transform))
                continue;

            if (hit.collider.transform.IsChildOf(transform))
                continue;

            Health targetHealth =
                hit.collider.GetComponentInParent<Health>();

            // 다른 몬스터는 무시
            if (targetHealth != null &&
                !targetHealth.CompareTag("Player"))
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestCollider = hit.collider;
            }
        }

        if (closestCollider != null)
        {
            transform.position +=
                moveDirection * closestDistance;

            HitTarget(closestCollider);
            return;
        }

        transform.position +=
            moveDirection * moveDistance;
    }

    bool IsOwner(Transform target)
    {
        if (owner == null)
            return false;

        return target == owner ||
               target.IsChildOf(owner);
    }

    void HitTarget(Collider other)
    {
        if (hasHit)
            return;

        if (IsOwner(other.transform))
            return;

        Health health =
            other.GetComponentInParent<Health>();

        if (health != null &&
            !health.CompareTag("Player"))
        {
            return;
        }

        hasHit = true;

        if (health != null &&
            health.CompareTag("Player"))
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!initialized || hasHit)
            return;

        if (IsOwner(other.transform))
            return;

        if (((1 << other.gameObject.layer) &
             hitLayers.value) == 0)
        {
            return;
        }

        HitTarget(other);
    }
}
