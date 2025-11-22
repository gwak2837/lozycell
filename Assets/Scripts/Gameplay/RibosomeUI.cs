using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RibosomeUI : MonoBehaviour
{
    [Header("Slot UI Components")]
    public Image[] slotImages; // 3 Slots
    public Image[] slotBorders; // 3 Borders
    public TextMeshProUGUI[] slotTexts; // Optional: Text on slots

    [Header("Font Settings")]
    public TMP_FontAsset customFontAsset; // Assign Korean-supporting font here

    [Header("Colors")]
    public Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    [Header("Dopamine Settings")]
    public float punchScaleNormal = 1.2f;
    public float punchScaleCombo = 1.5f; // 3번째 완성 시 더 커짐
    public float shakeIntensity = 10f; // 흔들림 강도

    private void Start()
    {
        if (ArcadeManager.Instance != null)
        {
            ArcadeManager.Instance.OnCodonUpdated += UpdateVisuals;
            // Force update immediately to sync with current state
            UpdateVisuals(ArcadeManager.Instance.GetCurrentCodon());
        }
        else
        {
            UpdateVisuals(new List<BaseType>());
        }
    }

    private void OnDestroy()
    {
        if (ArcadeManager.Instance != null)
        {
            ArcadeManager.Instance.OnCodonUpdated -= UpdateVisuals;
        }
    }

    // Factory method to create UI at runtime
    public static RibosomeUI CreateDefaultUI(Transform parentCanvas)
    {
        // Create Panel
        GameObject panelObj = new GameObject("RibosomePanel");
        panelObj.transform.SetParent(parentCanvas, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0, 50);
        panelRect.sizeDelta = new Vector2(350, 120);

        // Background
        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.7f);

        // Layout
        HorizontalLayoutGroup layout = panelObj.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 15;
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.childControlHeight = false;
        layout.childControlWidth = false;

        // Component
        RibosomeUI ui = panelObj.AddComponent<RibosomeUI>();

        // Try to load a default font asset for the factory-created instance
        TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts/AppleSDGothicNeo SDF");
        if (defaultFont == null)
            defaultFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
        if (defaultFont == null)
            defaultFont = Resources.Load<TMP_FontAsset>("Fonts/Pretendard-Medium SDF");
        if (defaultFont != null)
            ui.customFontAsset = defaultFont;

        ui.slotImages = new Image[3];
        ui.slotBorders = new Image[3];
        ui.slotTexts = new TextMeshProUGUI[3];

        // Create Slots
        for (int i = 0; i < 3; i++)
        {
            GameObject slotObj = new GameObject($"Slot_{i}");
            slotObj.transform.SetParent(panelObj.transform, false);

            // 1. Border (Background slightly larger)
            GameObject borderObj = new GameObject("Border");
            borderObj.transform.SetParent(slotObj.transform, false);
            RectTransform borderRt = borderObj.AddComponent<RectTransform>();
            borderRt.anchorMin = new Vector2(0.5f, 0.5f);
            borderRt.anchorMax = new Vector2(0.5f, 0.5f);
            borderRt.anchoredPosition = Vector2.zero;
            borderRt.sizeDelta = new Vector2(100, 100); // Slightly larger than 90x90 slot

            Image borderImg = borderObj.AddComponent<Image>();
            borderImg.color = new Color(1f, 1f, 1f, 0f); // Transparent by default
            ui.slotBorders[i] = borderImg;

            // 2. Slot Image (Foreground)
            GameObject imgObj = new GameObject("Image");
            imgObj.transform.SetParent(slotObj.transform, false);
            RectTransform imgRt = imgObj.AddComponent<RectTransform>();
            imgRt.sizeDelta = new Vector2(90, 90);

            Image img = imgObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            ui.slotImages[i] = img;

            RectTransform rt = slotObj.AddComponent<RectTransform>(); // Slot Container
            rt.sizeDelta = new Vector2(100, 100); // Match border size for spacing

            // Text (Child of Image so it's on top)
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(imgObj.transform, false);
            RectTransform textRt = textObj.AddComponent<RectTransform>();
            textRt.anchoredPosition = Vector2.zero;
            textRt.sizeDelta = new Vector2(90, 90);

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 45;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;

            // Add Outline for visibility
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = Color.black;

            // Apply Custom Font if available
            if (ui.customFontAsset != null)
            {
                tmp.font = ui.customFontAsset;
            }

            ui.slotTexts[i] = tmp;
        }

        return ui;
    }

    // Track running jackpot to stop it if state changes
    private Coroutine jackpotRoutine;

    private void UpdateVisuals(List<BaseType> currentCodon)
    {
        // Stop any existing jackpot effect to prevent color overrides
        if (jackpotRoutine != null)
        {
            StopCoroutine(jackpotRoutine);
            jackpotRoutine = null;
            // Force reset colors to ensure clean slate
            for (int i = 0; i < slotImages.Length; i++)
            {
                // If cleared, set to empty, otherwise restore base color
                if (i >= currentCodon.Count)
                    slotImages[i].color = emptyColor;

                // Reset border
                if (slotBorders != null && i < slotBorders.Length && slotBorders[i] != null)
                    slotBorders[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }

        if (currentCodon == null)
            currentCodon = new List<BaseType>();

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < currentCodon.Count)
            {
                BaseType type = currentCodon[i];
                Color c = GetColorForBase(type);

                slotImages[i].color = c;
                if (slotTexts.Length > i && slotTexts[i] != null)
                {
                    // Apply font if set (runtime update support)
                    if (customFontAsset != null && slotTexts[i].font != customFontAsset)
                    {
                        slotTexts[i].font = customFontAsset;
                    }

                    slotTexts[i].text = type.ToString();
                    // Fix contrast: If Yellow(G), use Black text. Else White.
                    // Assuming G is the bright yellow one.
                    if (type == BaseType.G)
                        slotTexts[i].color = Color.black;
                    else
                        slotTexts[i].color = Color.white;
                }

                if (i == currentCodon.Count - 1)
                {
                    // 3rd slot gets a jackpot effect
                    if (i == 2)
                    {
                        // Calculate amino acid to get skill color
                        string aminoAcid = CodonTable.GetAminoAcid(currentCodon[0], currentCodon[1], currentCodon[2]);
                        AminoAcidData data = CodonTable.GetData(aminoAcid);
                        jackpotRoutine = StartCoroutine(JackpotEffect(data.Color));
                    }
                    else
                    {
                        StartCoroutine(PunchAnimation(slotImages[i].transform, punchScaleNormal));
                    }
                }
            }
            else
            {
                slotImages[i].color = emptyColor;
                if (slotTexts.Length > i && slotTexts[i] != null)
                {
                    slotTexts[i].text = "";
                }
            }
        }
    }

    private IEnumerator PunchAnimation(Transform target, float maxScale)
    {
        float duration = 0.15f;
        float time = 0;
        Vector3 originalScale = Vector3.one;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * (maxScale - 1f);
            target.localScale = originalScale * scale;
            yield return null;
        }
        target.localScale = originalScale;
    }

    private IEnumerator JackpotEffect(Color skillColor)
    {
        // "Casino Win" Style: Border Flashing with Skill Color
        int flashes = 5;
        float interval = 0.1f;

        for (int i = 0; i < flashes; i++)
        {
            // Borders ON (Skill Color)
            foreach (var border in slotBorders)
            {
                if (border != null)
                {
                    // Use skill color with full opacity
                    border.color = new Color(skillColor.r, skillColor.g, skillColor.b, 1f);
                }
            }
            yield return new WaitForSeconds(interval);

            // Borders OFF (Transparent)
            foreach (var border in slotBorders)
            {
                if (border != null)
                    border.color = new Color(1f, 1f, 1f, 0f);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private Color GetColorForBase(BaseType type) => BaseColorConfig.GetColor(type);
}
