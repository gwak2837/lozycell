using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RibosomeUISetup : EditorWindow
{
    [MenuItem("Tools/Setup Ribosome UI")]
    public static void Setup()
    {
        // 1. Canvas 찾기 또는 생성
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 2. 기존 UI가 있다면 제거 (중복 방지)
        RibosomeUI existingUI = canvas.GetComponentInChildren<RibosomeUI>();
        if (existingUI != null)
        {
            DestroyImmediate(existingUI.gameObject);
        }

        // 3. 패널 생성 (화면 하단 중앙)
        GameObject panelObj = new GameObject("RibosomePanel");
        panelObj.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0, 50); // 바닥에서 약간 위
        panelRect.sizeDelta = new Vector2(350, 120);

        // 배경 (반투명 검정)
        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.7f);

        // 4. RibosomeUI 컴포넌트 추가 및 설정
        RibosomeUI uiScript = panelObj.AddComponent<RibosomeUI>();
        uiScript.slotImages = new Image[3];
        uiScript.slotTexts = new TextMeshProUGUI[3];

        // 레이아웃
        HorizontalLayoutGroup layout = panelObj.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 15;
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.childControlHeight = false;
        layout.childControlWidth = false;

        // 5. 슬롯 3개 생성
        for (int i = 0; i < 3; i++)
        {
            GameObject slotObj = new GameObject($"Slot_{i}");
            slotObj.transform.SetParent(panelObj.transform, false);

            Image img = slotObj.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"); // 기본 원형 스프라이트
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); // 초기 색상

            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(90, 90);

            uiScript.slotImages[i] = img;

            // 텍스트 (A, U, G, C)
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(slotObj.transform, false);
            RectTransform textRt = textObj.AddComponent<RectTransform>();
            textRt.anchoredPosition = Vector2.zero;
            textRt.sizeDelta = new Vector2(90, 90);

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 45;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;

            uiScript.slotTexts[i] = tmp;
        }

        Debug.Log("✅ 리보솜 슬롯머신 UI가 생성되었습니다!");
        Selection.activeGameObject = panelObj;
    }
}
