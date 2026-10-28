
using UnityEngine;
using UnityEngine.UI;
using System;

public class Health : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHp = 3;
    public int currentHp;
    public Slider hpSlider;

    [Header("적 사망 설정")]
    [Tooltip("적이 사망했는지 확인")]
    public bool isEnemyDead = false;

    [Tooltip("일반 적 사망 후 오브젝트 삭제 여부")]
    public bool destroyOnDeath = true;

    [Header("디버그")]
    [SerializeField]
    private bool isDead = false;

    // 웨이브 처치 카운트용 사망 이벤트
    public event Action<Health> OnDied;

    void Start()
    {
        maxHp = Mathf.Max(1, maxHp);
        currentHp = maxHp;

        isDead = false;
        isEnemyDead = false;

        UpdateBar();
    }

    // 데미지 받기
    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHp = Mathf.Max(
            0,
            currentHp - damage
        );

        Debug.Log(
            gameObject.name + " HP: " + currentHp
        );

        UpdateBar();

        if (currentHp <= 0)
        {
            Die();
        }
    }

    // 체력 회복
    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentHp = Mathf.Min(
            maxHp,
            currentHp + amount
        );

        Debug.Log(
            gameObject.name +
            " 체력 회복! 현재 HP: " + currentHp
        );

        UpdateBar();
    }

    // HP 슬라이더 업데이트
    void UpdateBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value =
                (float)currentHp / Mathf.Max(1, maxHp);
        }
    }

    // 사망 처리
    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHp = 0;

        UpdateBar();

        if (CompareTag("Player"))
        {
            Debug.Log("게임 오버!");

            OnDied?.Invoke(this);

            Time.timeScale = 0f;
        }
        else
        {
            // 적 사망 상태 적용
            isEnemyDead = true;

            // WaveEnemy가 이 이벤트를 받아
            // 웨이브 처치 수를 증가시킴
            OnDied?.Invoke(this);

            // 보상 상호작용이 있는 적은
            // 사망 후에도 오브젝트를 유지
            LootableEnemy loot =
                GetComponent<LootableEnemy>();

            if (loot != null)
            {
                Debug.Log(
                    gameObject.name +
                    " 사망! 보상 획득 가능"
                );

                return;
            }

            // 일반 적은 기존 방식대로 삭제
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}
