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
        [HideInInspector] public Vector2 originPos;
    }
    [Header("각 부위별 퇴장 애니메이션 세팅")]
    [SerializeField] private List<UICloseElementSetting> uiElements;
    private Sequence closeSequence;
    private bool isInitialized = false;



    private void Awake()
    {
        InitOriginPositions();
    }
    private void InitOriginPositions()
    {
        if (isInitialized) return;
        foreach (var element in uiElements)
        {
            if (element.targetRect != null)
            {
                element.originPos = element.targetRect.anchoredPosition;
            }
        }
        isInitialized = true;
    }
    public void ClosePanel()
    {
        InitOriginPositions();
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
        }
        closeSequence.SetUpdate(true);
        closeSequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            MainMenuUIManager.Instance.OnBackButtonClicked();
        });
        closeSequence.Play();
        
    }
    private void OnDisable()
    {
        closeSequence?.Kill();
    }
}