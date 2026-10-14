
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("적 프리팹")]
    [Tooltip("소환할 적 프리팹들을 넣으세요")]
    public GameObject[] enemyPrefabs;

    [Header("플레이어 설정")]
    [Tooltip("비워두면 Player 태그로 자동 검색")]
    public Transform player;

    [Header("소환 위치 설정")]
    [Tooltip("플레이어로부터 적이 생성되는 거리")]
    public float spawnDistance = 15f;

    [Tooltip("바닥 높이를 플레이어 높이에 맞춤")]
    public float spawnHeightOffset = 0f;

    [Header("소환 시간 설정")]
    [Tooltip("게임 시작 시 소환 간격")]
    public float startSpawnInterval = 3f;

    [Tooltip("가장 빠른 최소 소환 간격")]
    public float minSpawnInterval = 0.5f;

    [Tooltip("몇 초마다 소환 간격이 감소할지")]
    public float difficultyIncreaseTime = 20f;

    [Tooltip("난이도 상승 시 감소하는 시간")]
    public float intervalDecrease = 0.2f;

    [Header("적 제한")]
    [Tooltip("동시에 존재할 수 있는 최대 적 수")]
    public int maxAlive = 30;

    [Header("디버그")]
    [SerializeField]
    private float currentSpawnInterval;

    [SerializeField]
    private float elapsedTime;

    [SerializeField]
    private int currentEnemyCount;

    private float spawnTimer;
    private float difficultyTimer;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");

            if (p != null)
            {
                player = p.transform;
            }
        }

        currentSpawnInterval = Mathf.Max(
            minSpawnInterval,
            startSpawnInterval
        );

        spawnTimer = 0f;
        difficultyTimer = 0f;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (player == null)
            return;

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return;

        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;

        // 시간이 지날수록 소환 간격 감소
        UpdateDifficulty();

        // 소환 시간이 되었을 때
        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0f;

            SpawnEnemy();
        }
    }

    void UpdateDifficulty()
    {
        if (difficultyIncreaseTime <= 0f)
            return;

        if (difficultyTimer >= difficultyIncreaseTime)
        {
            difficultyTimer -= difficultyIncreaseTime;

            currentSpawnInterval -= intervalDecrease;

            currentSpawnInterval = Mathf.Max(
                minSpawnInterval,
                currentSpawnInterval
            );

            Debug.Log(
                "적 소환 간격 감소: " +
                currentSpawnInterval.ToString("F2") +
                "초"
            );
        }
    }

    void SpawnEnemy()
    {
        // 현재 살아있는 적 수 확인
        currentEnemyCount =
            GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemyCount >= maxAlive)
            return;

        // 여러 적 프리팹 중 랜덤 선택
        GameObject selectedEnemy = null;

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            int randomIndex = Random.Range(
                0,
                enemyPrefabs.Length
            );

            if (enemyPrefabs[randomIndex] != null)
            {
                selectedEnemy = enemyPrefabs[randomIndex];
                break;
            }
        }

        if (selectedEnemy == null)
            return;

        // 0 ~ 360도 랜덤 방향
        float randomAngle = Random.Range(0f, 360f);

        float angleRadians = randomAngle * Mathf.Deg2Rad;

        Vector3 spawnDirection = new Vector3(
            Mathf.Cos(angleRadians),
            0f,
            Mathf.Sin(angleRadians)
        );

        // 플레이어 기준 거리 15에서 소환
        Vector3 spawnPosition =
            player.position +
            spawnDirection * spawnDistance;

        spawnPosition.y =
            player.position.y + spawnHeightOffset;

        // 적이 플레이어를 바라보는 방향
        Vector3 lookDirection =
            player.position - spawnPosition;

        lookDirection.y = 0f;

        Quaternion spawnRotation = Quaternion.identity;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            spawnRotation = Quaternion.LookRotation(
                lookDirection,
                Vector3.up
            );
        }

        // 적 생성
        Instantiate(
            selectedEnemy,
            spawnPosition,
            spawnRotation
        );

        currentEnemyCount++;
    }

    // Scene 뷰에서 소환 범위 확인
    void OnDrawGizmosSelected()
    {
        Transform target = player;

        if (target == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            target.position,
            spawnDistance
        );
    }
}
