using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class UISetupTools : MonoBehaviour
{
    [MenuItem("Tools/Setup Game Over UI")]
    public static void SetupGameOverUI()
    {
        // 1. Find Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Debug.Log("Created new Canvas.");
        }

        // 2. Create or Update Lose Panel (Black transparent bg, Red text)
        UpdateOrCreatePanel(canvas.transform, "LosePanel", "GAME OVER", new Color(0, 0, 0, 0.85f), Color.red);

        // 3. Create or Update Win Panel (Black transparent bg, Green text)
        UpdateOrCreatePanel(canvas.transform, "WinPanel", "MISSION COMPLETE", new Color(0, 0, 0, 0.85f), Color.green);

        Debug.Log("UI Setup Complete. Please assign the panels to ArcadeManager in the Inspector.");
    }

    private static void UpdateOrCreatePanel(Transform parent, string name, string message, Color bgColor, Color textColor)
    {
        GameObject panelObj = GameObject.Find(name);
        
        if (panelObj == null)
        {
            // Create Panel
            panelObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelObj.transform.SetParent(parent, false);
            
            RectTransform rt = panelObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            // Create Text
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;

            panelObj.SetActive(false);
            Undo.RegisterCreatedObjectUndo(panelObj, "Create " + name);
            Debug.Log($"Created {name}.");
        }
        else
        {
            Debug.Log($"Updated existing {name}.");
        }

        // Update Visuals
        Image img = panelObj.GetComponent<Image>();
        if (img) img.color = bgColor;

        TextMeshProUGUI tmp = panelObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp)
        {
            tmp.text = message;
            tmp.fontSize = 64;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = textColor;
        }
    }
}

