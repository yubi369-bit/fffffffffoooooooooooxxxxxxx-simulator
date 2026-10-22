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
        public Vector2 startOffset;             // 출발 지점 (도착지 기준 오프셋)
        public float moveDuration = 0.5f;       // 이동하는 데 걸리는 시간
        public float moveDelay = 0f;            // 최초 등장 전 대기 시간
        public Ease moveEase = Ease.InOutCubic; // 가감속 형태
        [Header("크기(Scale) 애니메이션")]
        public bool useScale = false;
        public Vector3 startScale = Vector3.zero;
        public float scaleDuration = 0.4f;
        public float scaleDelay = 0f;
        public Ease scaleEase = Ease.OutBack;
        // --- [루프 및 루프 딜레이 설정] ---
        [Header("이동 애니메이션 자체 반복")]
        public bool isLoopMove = false;             // 체크하면 출발->도착 반복
        public LoopType loopType = LoopType.Restart;// Restart: 도착 후 출발지로 순간이동해서 다시 출발
        public int loopCount = -1;                  // -1: 무한 반복
        [Tooltip("도착지에 도착한 뒤 다음 반복까지 머무르는 쉼(대기) 시간(초)")]
        public float loopDelayInterval = 0.5f;      // ★ 매 반복마다 쉴 시간!
        [HideInInspector] public Vector2 originPos; // 도착 최종 위치
        [HideInInspector] public Sequence loopSequence; // 개별 루프 시퀀스
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
            // [이동 애니메이션]
            if (element.useMove)
            {
                if (element.isLoopMove)
                {
                    // ★ 핵심: [이동] + [대기시간(AppendInterval)] 묶음으로 Sequence 생성 후 반복
                    element.loopSequence = DOTween.Sequence();
                    // 1. 맨 처음 패널 열릴 때 시작 전 딜레이 (Move Delay)
                    element.loopSequence.AppendInterval(element.moveDelay);
                    // 2. 출발지에서 도착지로 이동
                    element.loopSequence.Append(
                        element.targetRect.DOAnchorPos(element.originPos, element.moveDuration)
                                          .SetEase(element.moveEase)
                    );
                    // 3. 도착한 후 머무는 딜레이 시간 (Loop Delay Interval)
                    if (element.loopDelayInterval > 0)
                    {
                        element.loopSequence.AppendInterval(element.loopDelayInterval);
                    }
                    // 4. [이동 + 대기] 묶음을 통째로 무한 반복 재생
                    element.loopSequence.SetLoops(element.loopCount, element.loopType)
                                        .SetUpdate(true);
                }
                else
                {
                    // 일반 1회 등장 UI
                    mySequence.Insert(element.moveDelay,
                        element.targetRect.DOAnchorPos(element.originPos, element.moveDuration)
                                          .SetEase(element.moveEase)
                    );
                }
            }
            // [스케일 애니메이션]
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