using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHp = 3;
    public int currentHp;
    public Slider hpSlider;

    [Header("텍스트 UI")]
    public GameObject itemText;

    [SerializeField] private float deathRotateSpeed = 1000f;

    public bool isEnemyDead = false;

    private bool isEnemyTrigger;

    private LootableEnemy currentLootableEnemy;

    void Start()
    {
        if (itemText != null)
        {
            itemText.SetActive(false);
        }


        isEnemyTrigger = false;
        currentHp = maxHp;
        UpdateBar();
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver)
        {
            if(itemText != null)
            {
                itemText.SetActive(false);
            }
        }

        if (isEnemyDead)
        {
            transform.Rotate(0f, deathRotateSpeed * Time.deltaTime, 0f);
        }

        if (isEnemyTrigger && currentLootableEnemy != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentLootableEnemy.Interact(gameObject);

                currentLootableEnemy = null;
                if (itemText != null) itemText.SetActive(false);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;
        currentHp -= damage;
        Debug.Log(name + " hp: " + currentHp);
        UpdateBar();

        if (currentHp <= 0) Die();
    }

    void UpdateBar()
    {
        if (hpSlider != null)
            hpSlider.value = (float)currentHp / maxHp;
    }

    void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("게임 오버");
            GameManager.instance.GameOver();
        }
        else if (CompareTag("Enemy"))
        {
            isEnemyDead = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Enemy")) return;


        if (other.CompareTag("Enemy"))
        {
            LootableEnemy target = other.GetComponent<LootableEnemy>();

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
        if (gameObject.CompareTag("Enemy")) return;


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
        currentHp = Mathf.Min(currentHp + amount, maxHp);

        UpdateBar();
    }
}
