using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SettingPanelAnimation : MonoBehaviour
{
    [System.Serializable]
    public class UIElementSetting
    {
        public string elementName;
        public RectTransform targetRect;
        [Header("이동(Position) 애니메이션")]
        public bool useMove = true;
        public Vector2 startOffset;
        public float moveDuration = 0.5f;
        public float moveDelay = 0f;
        public Ease moveEase = Ease.InOutCubic;
        [Header("크기(Scale) 애니메이션")]
        public bool useScale = false;
        public Vector3 startScale = Vector3.zero;
        public float scaleDuration = 0.4f;
        public float scaleDelay = 0f;
        public Ease scaleEase = Ease.OutBack;
        [Header("이동 애니메이션 자체 반복")]
        public bool isLoopMove = false;
        public LoopType loopType = LoopType.Restart;
        public int loopCount = -1;
        [Tooltip("도착지에 도착한 뒤 다음 반복까지 머무르는 쉼(대기) 시간(초)")]
        public float loopDelayInterval = 0.5f;
        [HideInInspector] public Vector2 originPos;
        [HideInInspector] public Sequence loopSequence;
    }
    [Header("각 부위별 애니메이션 세팅")]
    [SerializeField] private List<UIElementSetting> uiElements;
    private Sequence mySequence;
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
    private void OnEnable()
    {
        InitOriginPositions();
        PlayOpenAnimation();
    }
    public void PlayOpenAnimation()
    {
        if (uiElements == null || uiElements.Count == 0) return;
        KillAllTweens();
        ResetToStartState();
        mySequence = DOTween.Sequence();
        foreach (var element in uiElements)
        {
            if (element.targetRect == null) continue;
            if (element.useMove)
            {
                if (element.isLoopMove)
                {
                    element.loopSequence = DOTween.Sequence();
                    element.loopSequence.AppendInterval(element.moveDelay);
                    element.loopSequence.Append(
                        element.targetRect.DOAnchorPos(element.originPos, element.moveDuration)
                                          .SetEase(element.moveEase)
                    );
                    if (element.loopDelayInterval > 0)
                    {
                        element.loopSequence.AppendInterval(element.loopDelayInterval);
                    }
                    element.loopSequence.SetLoops(element.loopCount, element.loopType)
                                        .SetUpdate(true);
                }
                else
                {
                    mySequence.Insert(element.moveDelay,
                        element.targetRect.DOAnchorPos(element.originPos, element.moveDuration)
                                          .SetEase(element.moveEase)
                    );
                }
            }
            if (element.useScale)
            {
                mySequence.Insert(element.scaleDelay,
                    element.targetRect.DOScale(Vector3.one, element.scaleDuration)
                                      .SetEase(element.scaleEase)
                );
            }
        }
        mySequence.SetUpdate(true);
        mySequence.Play();
    }
    private void ResetToStartState()
    {
        foreach (var element in uiElements)
        {
            if (element.targetRect == null) continue;
            if (element.useMove)
            {
                element.targetRect.anchoredPosition = element.originPos + element.startOffset;
            }
            if (element.useScale)
            {
                element.targetRect.localScale = element.startScale;
            }
        }
    }
    private void KillAllTweens()
    {
        mySequence?.Kill();
        foreach (var element in uiElements)
        {
            element.loopSequence?.Kill();
        }
    }
    private void OnDisable()
    {
        KillAllTweens();
    }
}