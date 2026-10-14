using UnityEngine;

using UnityEngine;
 
public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;   // 근접, 원거리
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public int maxAlive = 10;
 
    float timer;

     void Update()
    {
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;
 
        int alive = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (alive >= maxAlive) return;
 
        int e = Random.Range(0, enemyPrefabs.Length);
        int s = Random.Range(0, spawnPoints.Length);
        Vector3 pos = spawnPoints[s].position;
        Instantiate(enemyPrefabs[e], pos, Quaternion.identity);
    }
}
