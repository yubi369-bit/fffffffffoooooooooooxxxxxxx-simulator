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
        [Header("회전(Rotation) 애니메이션")]
        public bool useRotate = false;
        [Tooltip("등장 전 시작할 Z축 각도 (예: -180, 360)")]
        public float startRotationZ = 180f;
        public float rotateDuration = 0.5f;
        public float rotateDelay = 0f;
        public Ease rotateEase = Ease.OutBack;
        [Header("이동 애니메이션 자체 반복")]
        public bool isLoopMove = false;
        public LoopType loopType = LoopType.Restart;
        public int loopCount = -1;
        public float loopDelayInterval = 0.5f;
        [HideInInspector] public Vector2 originPos;
        [HideInInspector] public Vector3 originRot;
        [HideInInspector] public Sequence loopSequence;
    }
    [Header("각 부위별 애니메이션 세팅")]
    [SerializeField] private List<UIElementSetting> uiElements;
    private Sequence mySequence;
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
    private void OnEnable()
    {
        InitOriginData();
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
                        element.loopSequence.AppendInterval(element.loopDelayInterval);
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
            if (element.useRotate)
            {
                mySequence.Insert(element.rotateDelay,
                    element.targetRect.DOLocalRotate(element.originRot, element.rotateDuration, RotateMode.FastBeyond360)
                                      .SetEase(element.rotateEase)
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
                element.targetRect.anchoredPosition = element.originPos + element.startOffset;
            if (element.useScale)
                element.targetRect.localScale = element.startScale;
            if (element.useRotate)
                element.targetRect.localEulerAngles = new Vector3(element.originRot.x, element.originRot.y, element.startRotationZ);
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