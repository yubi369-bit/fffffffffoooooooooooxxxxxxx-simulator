using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SettingPanelCloseAnimation : MonoBehaviour
{
    [System.Serializable]
    public class UICloseElementSetting
    {
        public string elementName;
        public RectTransform targetRect;
        [Header("퇴장 이동(Position) 애니메이션")]
        public bool useMove = true;
        public Vector2 exitOffset;
        public float moveDuration = 0.4f;
        public float moveDelay = 0f;
        public Ease moveEase = Ease.InBack;
        [Header("퇴장 크기(Scale) 애니메이션")]
        public bool useScale = false;
        public Vector3 targetScale = Vector3.zero;
        public float scaleDuration = 0.3f;
        public float scaleDelay = 0f;
        public Ease scaleEase = Ease.InBack;
        [Header("퇴장 회전(Rotation) 애니메이션")]
        public bool useRotate = false;
        [Tooltip("퇴장할 때 도달할 Z축 각도 (예: 180, 360, -90)")]
        public float exitRotationZ = 180f;
        public float rotateDuration = 0.4f;
        public float rotateDelay = 0f;
        public Ease rotateEase = Ease.InBack;
        [HideInInspector] public Vector2 originPos;
        [HideInInspector] public Vector3 originRot;
    }
    [Header("각 부위별 퇴장 애니메이션 세팅")]
    [SerializeField] private List<UICloseElementSetting> uiElements;
    private Sequence closeSequence;
    private bool isInitialized = false;
    private void Awake()
    {
        InitOriginData();
    }
    private void InitOriginData()
    {
        if (isInitialized) return;
        foreach (var element in uiElements)
        {
            if (element.targetRect != null)
            {
                element.originPos = element.targetRect.anchoredPosition;
                element.originRot = element.targetRect.localEulerAngles;
            }
        }
        isInitialized = true;
    }
    public void ClosePanel()
    {
        InitOriginData();
        DOTween.Kill(gameObject);
        closeSequence?.Kill();
        closeSequence = DOTween.Sequence();
        foreach (var element in uiElements)
        {
            if (element.targetRect == null) continue;
            if (element.useMove)
            {
                Vector2 targetPos = element.originPos + element.exitOffset;
                closeSequence.Insert(element.moveDelay,
                    element.targetRect.DOAnchorPos(targetPos, element.moveDuration)
                                      .SetEase(element.moveEase)
                );
            }
            if (element.useScale)
            {
                closeSequence.Insert(element.scaleDelay,
                    element.targetRect.DOScale(element.targetScale, element.scaleDuration)
                                      .SetEase(element.scaleEase)
                );
            }
            if (element.useRotate)
            {
                Vector3 targetRot = new Vector3(element.originRot.x, element.originRot.y, element.exitRotationZ);
                closeSequence.Insert(element.rotateDelay,
                    element.targetRect.DOLocalRotate(targetRot, element.rotateDuration, RotateMode.FastBeyond360)
                                      .SetEase(element.rotateEase)
                );
            }
        }
        closeSequence.SetUpdate(true);
        closeSequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            if (MainMenuUIManager.Instance != null)
            {
                MainMenuUIManager.Instance.OnBackButtonClicked();
            }
            else
            {
                Debug.LogWarning("MainMenuUIManager 인스턴스를 찾을 수 없습니다.");
            }
        });
        closeSequence.Play();
    }
    private void OnDisable()
    {
        closeSequence?.Kill();
    }
}