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

        // 2. Create Lose Panel if not exists
        if (GameObject.Find("LosePanel") == null)
        {
            CreatePanel(canvas.transform, "LosePanel", "GAME OVER", new Color(0.5f, 0, 0, 0.8f));
            Debug.Log("Created LosePanel.");
        }
        else
        {
            Debug.Log("LosePanel already exists.");
        }

        // 3. Create Win Panel if not exists
        if (GameObject.Find("WinPanel") == null)
        {
            CreatePanel(canvas.transform, "WinPanel", "MISSION COMPLETE", new Color(0, 0.5f, 0, 0.8f));
            Debug.Log("Created WinPanel.");
        }
        else
        {
            Debug.Log("WinPanel already exists.");
        }

        Debug.Log("UI Setup Complete. Please assign the panels to ArcadeManager in the Inspector.");
    }

    private static void CreatePanel(Transform parent, string name, string message, Color bgColor)
    {
        // Panel
        GameObject panelObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObj.transform.SetParent(parent, false);
        
        RectTransform rt = panelObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        Image img = panelObj.GetComponent<Image>();
        img.color = bgColor;

        // Text
        GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(panelObj.transform, false);

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.fontSize = 64;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        panelObj.SetActive(false);
        
        // Register Undo for Editor
        Undo.RegisterCreatedObjectUndo(panelObj, "Create " + name);
    }
}

