
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("적 프리팹")]
    public GameObject[] enemyPrefabs;

    [Header("플레이어")]
    public Transform player;

    [Header("소환 위치")]
    public float spawnDistance = 15f;
    public float spawnHeightOffset = 0f;

    [Header("웨이브 설정")]
    public int startKillGoal = 10;
    public int killsAddedPerWave = 5;
    public float waveDelay = 3f;

    [Header("소환 간격")]
    public float startSpawnInterval = 3f;
    public float minSpawnInterval = 0.5f;
    public float intervalDecreasePerWave = 0.3f;

    [Header("시간 경과 난이도")]
    public float difficultyIncreaseTime = 20f;
    public float intervalDecreaseOverTime = 0.1f;

    [Header("적 제한")]
    public int maxAlive = 30;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text killText;

    [Header("현재 상태 (확인용)")]
    [SerializeField] private int currentWave = 1;
    [SerializeField] private int waveKills;
    [SerializeField] private int killGoal;
    [SerializeField] private float currentSpawnInterval;

    private float spawnTimer;
    private float elapsedTime;
    private float nextWaveTimer;
    private bool waitingForNextWave;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");

            if (p != null)
                player = p.transform;
        }

        currentWave = 1;
        waveKills = 0;
        killGoal = Mathf.Max(1, startKillGoal);

        currentSpawnInterval =
            Mathf.Max(0.05f, startSpawnInterval);

        spawnTimer = 0f;
        elapsedTime = 0f;

        UpdateUI();
    }

    void Update()
    {
        if (player == null)
            return;

        if (waitingForNextWave)
        {
            nextWaveTimer -= Time.deltaTime;

            if (nextWaveTimer <= 0f)
                StartNextWave();

            return;
        }

        elapsedTime += Time.deltaTime;

        UpdateDifficulty();

        spawnTimer += Time.deltaTime;

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

        int timeSteps = Mathf.FloorToInt(
            elapsedTime / difficultyIncreaseTime
        );

        float waveReduction =
            (currentWave - 1) * intervalDecreasePerWave;

        float timeReduction =
            timeSteps * intervalDecreaseOverTime;

        currentSpawnInterval = Mathf.Max(
            Mathf.Max(0.05f, minSpawnInterval),
            startSpawnInterval -
            waveReduction -
            timeReduction
        );
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null ||
            enemyPrefabs.Length == 0)
            return;

        int alive = GameObject.FindGameObjectsWithTag(
            "Enemy"
        ).Length;

        if (alive >= maxAlive)
            return;

        GameObject selectedEnemy = null;

        int startIndex = Random.Range(
            0, enemyPrefabs.Length
        );

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            int index = (startIndex + i) %
                enemyPrefabs.Length;

            if (enemyPrefabs[index] != null)
            {
                selectedEnemy = enemyPrefabs[index];
                break;
            }
        }

        if (selectedEnemy == null)
            return;

        float angle = Random.Range(0f, 360f);
        float radians = angle * Mathf.Deg2Rad;

        Vector3 direction = new Vector3(
            Mathf.Cos(radians),
            0f,
            Mathf.Sin(radians)
        );

        Vector3 spawnPosition =
            player.position +
            direction * spawnDistance;

        spawnPosition.y =
            player.position.y + spawnHeightOffset;

        Vector3 lookDirection =
            player.position - spawnPosition;

        lookDirection.y = 0f;

        Quaternion rotation = Quaternion.identity;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            rotation = Quaternion.LookRotation(
                lookDirection.normalized
            );
        }

        GameObject enemy = Instantiate(
            selectedEnemy,
            spawnPosition,
            rotation
        );

        // 스포너가 생성한 적에게 웨이브 정보 연결
        WaveEnemy waveEnemy =
            enemy.GetComponent<WaveEnemy>();

        if (waveEnemy == null)
            waveEnemy = enemy.AddComponent<WaveEnemy>();

        waveEnemy.Register(this);
    }

    // 적이 죽을 때 호출되는 함수
    public void RegisterKill()
    {
        if (waitingForNextWave)
            return;

        waveKills++;

        if (waveKills >= killGoal)
        {
            waveKills = killGoal;
            waitingForNextWave = true;
            nextWaveTimer = Mathf.Max(0f, waveDelay);
        }

        UpdateUI();
    }

    void StartNextWave()
    {
        currentWave++;
        waveKills = 0;

        killGoal = Mathf.Max(
            1,
            startKillGoal +
            (currentWave - 1) * killsAddedPerWave
        );

        waitingForNextWave = false;
        spawnTimer = 0f;

        UpdateDifficulty();
        UpdateUI();

        Debug.Log("Wave " + currentWave + " 시작!");
    }

    void UpdateUI()
    {
        if (waveText != null)
        {
            waveText.text = "WAVE " + currentWave;
        }

        if (killText != null)
        {
            if (waitingForNextWave)
            {
                killText.text = "WAVE CLEAR!";
            }
            else
            {
                killText.text =
                    "KILL " + waveKills + " / " + killGoal;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            player.position,
            spawnDistance
        );
    }
}
