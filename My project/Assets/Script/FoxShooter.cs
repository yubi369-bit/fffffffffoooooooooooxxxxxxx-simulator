
using UnityEngine;
using UnityEngine.InputSystem;

public class FoxShooter : MonoBehaviour
{
    [Header("총알 설정")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("발사 설정")]
    public float fireRate = 0.25f;
    public float bulletSpeed = 20f;
    public int bulletDamage = 1;

    [Header("조준 설정")]
    public Camera playerCamera;
    public float aimDistance = 100f;
    public LayerMask aimLayers = ~0;

    [Header("발사 방식")]
    public bool automaticFire = true;

    private float nextFireTime;
    private Collider[] ownerColliders;

    void Awake()
    {
        ownerColliders = GetComponentsInChildren<Collider>();
    }

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        bool fireInput = automaticFire
            ? Mouse.current.leftButton.isPressed
            : Mouse.current.leftButton.wasPressedThisFrame;

        if (fireInput && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "FoxShooter: Bullet Prefab 또는 Fire Point가 없습니다."
            );
            return;
        }

        nextFireTime = Time.time + Mathf.Max(0.01f, fireRate);

        Vector3 shootDirection = firePoint.forward;

        // 카메라 중심을 기준으로 목표 지점 계산
        if (playerCamera != null)
        {
            Ray ray = playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

            Vector3 targetPoint =
                ray.origin + ray.direction * aimDistance;

            RaycastHit[] hits = Physics.RaycastAll(
                ray,
                aimDistance,
                aimLayers,
                QueryTriggerInteraction.Ignore
            );

            float closestDistance = float.MaxValue;

            foreach (RaycastHit hit in hits)
            {
                // 플레이어 자신은 조준에서 제외
                if (hit.collider.transform == transform ||
                    hit.collider.transform.IsChildOf(transform))
                {
                    continue;
                }

                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    targetPoint = hit.point;
                }
            }

            Vector3 direction =
                targetPoint - firePoint.position;

            if (direction.sqrMagnitude > 0.001f)
            {
                shootDirection = direction.normalized;
            }
        }

        GameObject bulletObject = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        // 총알이 플레이어 자신을 맞히지 않도록 설정
        Collider[] bulletColliders =
            bulletObject.GetComponentsInChildren<Collider>();

        foreach (Collider bulletCollider in bulletColliders)
        {
            foreach (Collider ownerCollider in ownerColliders)
            {
                if (bulletCollider != null &&
                    ownerCollider != null)
                {
                    Physics.IgnoreCollision(
                        bulletCollider,
                        ownerCollider,
                        true
                    );
                }
            }
        }

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        if (bullet == null)
        {
            Debug.LogError(
                "총알 프리팹에 Bullet.cs가 없습니다."
            );

            Destroy(bulletObject);
            return;
        }

        bullet.Initialize(
            shootDirection,
            bulletSpeed,
            bulletDamage
        );
    }
}
