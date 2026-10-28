
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CrosshairCursor : MonoBehaviour
{
    [Header("조준점 설정")]
    [Tooltip("조준점으로 사용할 이미지")]
    public Sprite crosshairSprite;

    [Tooltip("조준점 크기")]
    public float crosshairSize = 32f;

    [Header("UI 설정")]
    [Tooltip("비워두면 Canvas를 자동으로 생성합니다")]
    public Canvas targetCanvas;

    [Header("커서 설정")]
    public bool lockCursorOnStart = true;

    private Image crosshairImage;
    private RectTransform crosshairRect;

    void Start()
    {
        CreateCrosshair();

        if (lockCursorOnStart)
        {
            LockCursor();
        }
    }

    void Update()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame &&
            Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }

        UpdateCrosshair();
    }

    void CreateCrosshair()
    {
        if (targetCanvas == null)
        {
            GameObject canvasObject =
                new GameObject(
                    "CrosshairCanvas",
                    typeof(RectTransform),
                    typeof(Canvas),
                    typeof(CanvasScaler)
                );

            targetCanvas =
                canvasObject.GetComponent<Canvas>();

            targetCanvas.renderMode =
                RenderMode.ScreenSpaceOverlay;

            targetCanvas.sortingOrder = 100;

            CanvasScaler scaler =
                canvasObject.GetComponent<CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(1920, 1080);

            scaler.screenMatchMode =
                CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            scaler.matchWidthOrHeight = 0.5f;
        }

        GameObject crosshairObject =
            new GameObject(
                "Crosshair",
                typeof(RectTransform),
                typeof(Image)
            );

        crosshairObject.transform.SetParent(
            targetCanvas.transform,
            false
        );

        crosshairImage =
            crosshairObject.GetComponent<Image>();

        crosshairRect =
            crosshairObject.GetComponent<RectTransform>();

        crosshairImage.sprite = crosshairSprite;
        crosshairImage.raycastTarget = false;

        crosshairImage.preserveAspect = true;

        // 화면 정중앙 고정
        crosshairRect.anchorMin =
            new Vector2(0.5f, 0.5f);

        crosshairRect.anchorMax =
            new Vector2(0.5f, 0.5f);

        crosshairRect.pivot =
            new Vector2(0.5f, 0.5f);

        crosshairRect.anchoredPosition =
            Vector2.zero;

        crosshairRect.sizeDelta =
            new Vector2(
                crosshairSize,
                crosshairSize
            );

        UpdateCrosshair();
    }

    void UpdateCrosshair()
    {
        if (crosshairImage == null)
            return;

        // 마우스 고정 중에만 조준점 표시
        bool isLocked =
            Cursor.lockState == CursorLockMode.Locked;

        crosshairImage.enabled = isLocked;

        crosshairRect.sizeDelta =
            new Vector2(
                crosshairSize,
                crosshairSize
            );
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
