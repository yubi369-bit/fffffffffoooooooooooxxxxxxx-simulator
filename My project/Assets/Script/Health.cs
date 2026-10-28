
using System;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHp = 10;
    public int currentHp;
    public Slider hpSlider;

    public AudioClip takeDamageSound;
    public AudioClip enemyDeadSound;

    [Header("텍스트 UI")]
    public GameObject itemText;

    [SerializeField]
    private float deathRotateSpeed = 1000f;

    public bool isEnemyDead = false;

    private bool isEnemyTrigger;
    private LootableEnemy currentLootableEnemy;

    // 웨이브 시스템 추가
    // 적이 사망했을 때 WaveEnemy.cs에 알림
    public event Action<Health> OnDied;

    private bool deathEventSent = false;

    void Start()
    {
        if (itemText != null)
        {
            itemText.SetActive(false);
        }

        isEnemyTrigger = false;
        currentHp = maxHp;

        deathEventSent = false;

        UpdateBar();
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver)
        {
            if (itemText != null)
            {
                itemText.SetActive(false);
            }
        }

        if (isEnemyDead)
        {
            transform.Rotate(
                0f,
                deathRotateSpeed * Time.deltaTime,
                0f
            );
        }

        if (isEnemyTrigger &&
            currentLootableEnemy != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentLootableEnemy.Interact(gameObject);

                currentLootableEnemy = null;

                if (itemText != null)
                {
                    itemText.SetActive(false);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (takeDamageSound != null)
        {
            AudioManager.Instance.PlaySFX(
                takeDamageSound
            );
        }

        if (currentHp <= 0)
            return;

        currentHp -= damage;

        Debug.Log(
            name + " hp: " + currentHp
        );

        UpdateBar();

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void UpdateBar()
    {
        if (hpSlider != null)
        {
            hpSlider.value =
                (float)currentHp / maxHp;
        }
    }

    void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("게임 오버");

            // 기존 게임오버 시스템 유지
            GameManager.instance.GameOver();
        }
        else if (CompareTag("Enemy"))
        {
            if (enemyDeadSound != null)
            {
                AudioManager.Instance.PlaySFX(
                    enemyDeadSound
                );
            }

            // 기존 적 사망 상태 유지
            isEnemyDead = true;

            // 웨이브 처치 카운트 추가
            // 한 번만 실행되도록 중복 방지
            if (!deathEventSent)
            {
                deathEventSent = true;

                OnDied?.Invoke(this);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Enemy"))
            return;

        if (other.CompareTag("Enemy"))
        {
            LootableEnemy target =
                other.GetComponent<LootableEnemy>();

            if (itemText != null)
            {
                itemText.SetActive(true);
            }

            if (target != null)
            {
                currentLootableEnemy = target;
                isEnemyTrigger = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("Enemy"))
            return;

        if (other.CompareTag("Enemy"))
        {
            if (itemText != null)
            {
                itemText.SetActive(false);
            }

            currentLootableEnemy = null;
            isEnemyTrigger = false;
        }
    }

    public void Heal(int amount)
    {
        currentHp = Mathf.Min(
            currentHp + amount,
            maxHp
        );

        UpdateBar();
    }
}
