using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("감지 설정")]
    public float interactRange = 3f;
    public LayerMask interactableLayer; // LootableEnemy가 속한 레이어 지정
    public KeyCode interactKey = KeyCode.E;

    [Header("UI 연결 (선택 사항)")]
    // 상호작용 안내 텍스트 UI가 있다면 연결 (예: TextMeshProGUI)
    // public TMPro.TextMeshProUGUI promptText;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        DetectAndInteract();
    }

    private void DetectAndInteract()
    {
        // 카메라 중앙 시선 방향으로 레이캐스트 발사 (3인칭 슈팅 기준)
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            LootableEnemy lootable = hit.collider.GetComponent<LootableEnemy>();

            if (lootable != null)
            {
                // UI 메세지 표시 예시:
                // if (promptText != null) promptText.text = lootable.interactPrompt;

                // 상호작용 키 입력
                if (Input.GetKeyDown(interactKey))
                {
                    lootable.Interact(gameObject);
                }
                return;
            }
        }

        // 감지된 오브젝트가 없을 경우 UI 숨기기 예시
        // if (promptText != null) promptText.text = "";
    }
}