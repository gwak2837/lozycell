using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class RibosomeUI : MonoBehaviour
{
    [Header("Slot UI Components")]
    public Image[] slotImages; // 3 Slots
    public TextMeshProUGUI[] slotTexts; // Optional: Text on slots
    
    [Header("Colors")]
    public Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    public Color colorA = new Color(0.2f, 0.8f, 0.2f);
    public Color colorU = new Color(1f, 0.2f, 0.2f);
    public Color colorG = new Color(1f, 0.92f, 0.016f);
    public Color colorC = new Color(0f, 0.5f, 1f);

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
        ui.slotImages = new Image[3];
        ui.slotTexts = new TextMeshProUGUI[3];

        // Create Slots
        for (int i = 0; i < 3; i++)
        {
            GameObject slotObj = new GameObject($"Slot_{i}");
            slotObj.transform.SetParent(panelObj.transform, false);
            
            Image img = slotObj.AddComponent<Image>();
            // Default sprite if none (white square), or load knob if possible, but usually white square is fine for color tint
            // img.sprite = ... 
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(90, 90);
            ui.slotImages[i] = img;

            // Text
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
            ui.slotTexts[i] = tmp;
        }
        
        return ui;
    }

    private void UpdateVisuals(List<BaseType> currentCodon)
    {
        if (currentCodon == null) currentCodon = new List<BaseType>();
        
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < currentCodon.Count)
            {
                BaseType type = currentCodon[i];
                Color c = GetColorForBase(type);
                
                slotImages[i].color = c;
                if (slotTexts.Length > i && slotTexts[i] != null)
                {
                    slotTexts[i].text = type.ToString();
                }

                if (i == currentCodon.Count - 1)
                {
                    StartCoroutine(PunchAnimation(slotImages[i].transform));
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

    private IEnumerator PunchAnimation(Transform target)
    {
        float duration = 0.2f;
        float time = 0;
        Vector3 originalScale = Vector3.one;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f; 
            target.localScale = originalScale * scale;
            yield return null;
        }
        target.localScale = originalScale;
    }

    private Color GetColorForBase(BaseType type)
    {
        switch (type)
        {
            case BaseType.A: return colorA;
            case BaseType.U: return colorU;
            case BaseType.G: return colorG;
            case BaseType.C: return colorC;
            default: return Color.white;
        }
    }
}
