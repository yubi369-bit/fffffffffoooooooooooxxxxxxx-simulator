using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("플레이어 속성")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public int currentAmmo = 120;
    public float moveSpeed = 5f;

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
    }

    public void ApplySpeedBuff(float multiplier, float duration)
    {
        // 간단한 속도 버프 구현 예시
        StartCoroutine(SpeedBuffRoutine(multiplier, duration));
    }

    private System.Collections.IEnumerator SpeedBuffRoutine(float multiplier, float duration)
    {
        moveSpeed *= multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed /= multiplier;
    }
}