using UnityEngine;

public class LootableEnemy : MonoBehaviour
{
    public enum RewardType { Health, Ammo, SpeedBuff }

    [Header("상호작용 설정")]
    public RewardType rewardType = RewardType.Health;
    public float rewardValue = 30f;
    public float interactionDistance = 3f;
    
    [Header("UI 안내 메세지")]
    public string interactPrompt = "Press [E] to Search Body";

    private bool isInteractable = true;

    // 플레이어가 상호작용했을 때 호출되는 함수
    public void Interact(GameObject player)
    {
        if (!isInteractable) return;

        // 보상 지급
        ApplyReward(player);

        // 한 번 상호작용 후 비활성화 및 오브젝트 파괴
        isInteractable = false;
        Destroy(gameObject); 
    }

    private void ApplyReward(GameObject player)
    {
        // 플레이어에게 부착된 스크립트 가져오기
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        switch (rewardType)
        {
            case RewardType.Health:
                stats.Heal(rewardValue);
                Debug.Log($"체력 {rewardValue} 회복!");
                break;

            case RewardType.Ammo:
                stats.AddAmmo((int)rewardValue);
                Debug.Log($"탄약 {rewardValue} 획득!");
                break;

            case RewardType.SpeedBuff:
                stats.ApplySpeedBuff(rewardValue, 5f); // 5초간 이동속도 증가
                Debug.Log("이동 속도 증가!");
                break;
        }
    }
}