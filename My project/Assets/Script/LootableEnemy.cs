using TMPro;
using UnityEngine;

public class LootableEnemy : MonoBehaviour
{
    public enum RewardType { Health }

    [Header("상호작용 설정")]
    public RewardType rewardType = RewardType.Health;
    public int rewardValue = 30;
    public float interactionDistance = 3f;

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
        Health health = player.GetComponent<Health>();
        if (health == null) return;

        switch (rewardType)
        {
            case RewardType.Health:
                health.Heal(rewardValue);
                Debug.Log($"체력 {rewardValue} 회복!");
                break;
        }
    }
}