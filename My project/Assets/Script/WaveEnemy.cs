
using UnityEngine;

public class WaveEnemy : MonoBehaviour
{
    private EnemySpawner spawner;
    private Health health;

    private bool killReported;

    public void Register(EnemySpawner owner)
    {
        // 기존 등록 해제
        if (health != null)
        {
            health.OnDied -= HandleDeath;
        }

        spawner = owner;
        killReported = false;

        health = GetComponent<Health>();

        if (health == null)
        {
            Debug.LogWarning(
                gameObject.name +
                ": Health 컴포넌트가 없습니다."
            );
            return;
        }

        health.OnDied += HandleDeath;
    }

    void HandleDeath(Health deadHealth)
    {
        if (killReported)
            return;

        killReported = true;

        if (spawner != null)
        {
            spawner.RegisterKill();
        }
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnDied -= HandleDeath;
        }
    }
}
