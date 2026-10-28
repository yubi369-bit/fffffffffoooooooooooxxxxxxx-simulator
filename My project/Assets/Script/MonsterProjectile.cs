using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    [Header("투사체 설정")]
    public float speed = 15f;           // 날아가는 속도
    public float damage = 10f;          // 플레이어에게 줄 데미지
    public float lifeTime = 5f;         // 자동으로 사라질 시간

    [Header("이펙트 (선택 사항)")]
    public GameObject hitEffectPrefab;  // 충돌 시 터지는 이펙트 프리팹

    private Vector3 moveDirection;

    public void Setup(Vector3 direction)
    {
        moveDirection = direction;
        // 총알이 날아가는 방향을 바라보도록 회전
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        // 일정 시간이 지나면 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 설정된 방향으로 직선 이동
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 몬스터 자신에게 부딪히는 것은 무시
        if (other.CompareTag("Enemy") || other.CompareTag("Untagged")) 
            return;

        // 플레이어에게 데미지 전달 예시
        if (other.CompareTag("Player"))
        {
            // 플레이어 데미지 처리 스크립트가 있다면 호출
            // other.GetComponent<PlayerStats>()?.TakeDamage(damage);
            Debug.Log($"플레이어가 몬스터 투사체에 맞아 {damage} 데미지를 입었습니다.");
        }

        // 히트 이펙트 생성
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // 충돌 후 투사체 파괴
        Destroy(gameObject);
    }
}