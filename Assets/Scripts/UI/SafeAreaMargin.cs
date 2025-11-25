using UnityEngine;
using UnityEngine.EventSystems;

public class SafeAreaMargin : UIBehaviour
{
    [Header("Edges to Apply")]
    [SerializeField]
    private bool applyLeft = true;

    [SerializeField]
    private bool applyRight = false;

    [SerializeField]
    private bool applyTop = false;

    [SerializeField]
    private bool applyBottom = true;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Rect lastSafeArea;
    private Vector2 initialAnchoredPosition;
    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    protected override void Start()
    {
        // Start에서 초기 위치를 저장하여 에디터에서 배치한 위치를 기준으로 삼음
        base.Start();
        initialAnchoredPosition = rectTransform.anchoredPosition;
        isInitialized = true;
        Refresh();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if (isInitialized)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        // Safe Area가 실제로 변경되었는지 확인하여 불필요한 연산 방지
        if (lastSafeArea != Screen.safeArea)
        {
            lastSafeArea = Screen.safeArea;
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        float scaleFactor = canvas.scaleFactor;

        // scaleFactor가 0인 경우 방지 (보통 0일 수 없으나 안전장치)
        if (scaleFactor <= 0.001f)
            scaleFactor = 1f;

        // Screen Space의 Safe Area 여백 계산
        float leftMargin = lastSafeArea.x;
        float bottomMargin = lastSafeArea.y;
        float rightMargin = Screen.width - (lastSafeArea.x + lastSafeArea.width);
        float topMargin = Screen.height - (lastSafeArea.y + lastSafeArea.height);

        // Canvas Space로 변환 및 적용할 오프셋 계산
        float xOffset = 0f;
        float yOffset = 0f;

        if (applyLeft)
            xOffset += leftMargin / scaleFactor;
        if (applyRight)
            xOffset -= rightMargin / scaleFactor; // 오른쪽 여백은 왼쪽으로 밀어야 함
        if (applyBottom)
            yOffset += bottomMargin / scaleFactor;
        if (applyTop)
            yOffset -= topMargin / scaleFactor; // 위쪽 여백은 아래로 밀어야 함

        rectTransform.anchoredPosition = initialAnchoredPosition + new Vector2(xOffset, yOffset);
    }
}
