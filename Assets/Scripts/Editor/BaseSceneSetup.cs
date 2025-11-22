using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class BaseSceneSetup
{
    [MenuItem("Tools/Setup Base Scene")]
    public static void Setup()
    {
        // Create new scene
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "BaseScene"; // Note: Saving determines the name on disk

        // Create Main Camera
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        camObj.AddComponent<UniversalAdditionalCameraData>();
        cam.orthographic = true;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        camObj.transform.position = new Vector3(0, 0, -10);

        // Create EventSystem
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        // Use InputSystemUIInputModule instead of StandaloneInputModule for new Input System
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // Create UI Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Setup BaseManager
        GameObject managerObj = new GameObject("BaseManager");
        BaseManager manager = managerObj.AddComponent<BaseManager>();

        // 1. Amino Acid Text
        GameObject aaTextObj = CreateText(canvasObj.transform, "AminoAcidText", "Amino Acids: 0", new Vector2(0, 150));
        manager.aminoAcidText = aaTextObj.GetComponent<TextMeshProUGUI>();

        // 2. Level Text
        GameObject lvTextObj = CreateText(canvasObj.transform, "LevelText", "Mitochondria Lv: 1", new Vector2(0, 100));
        manager.levelText = lvTextObj.GetComponent<TextMeshProUGUI>();

        // 3. Attack Text
        GameObject atkTextObj = CreateText(canvasObj.transform, "AttackText", "Attack Power: 10", new Vector2(0, 50));
        manager.attackText = atkTextObj.GetComponent<TextMeshProUGUI>();

        // 4. Upgrade Button
        GameObject upgradeBtnObj = CreateButton(canvasObj.transform, "UpgradeButton", "Upgrade (100 AA)", new Vector2(0, -50));
        manager.upgradeButton = upgradeBtnObj.GetComponent<Button>();

        // 5. Start Button
        GameObject startBtnObj = CreateButton(canvasObj.transform, "StartButton", "Start Arcade Mode", new Vector2(0, -150));
        manager.startArcadeButton = startBtnObj.GetComponent<Button>();

        // Save Scene
        string path = "Assets/Scenes/BaseScene.unity";
        EditorSceneManager.SaveScene(scene, path);
        Debug.Log($"BaseScene saved to {path}");
        
        // Add to Build Settings if not present
        AddSceneToBuildSettings(path);
    }

    private static GameObject CreateText(Transform parent, string name, string content, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        
        TextMeshProUGUI txt = go.AddComponent<TextMeshProUGUI>();
        txt.text = content;
        txt.fontSize = 36;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 50);
        rt.anchoredPosition = anchoredPos;
        
        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Vector2 anchoredPos)
    {
        // Background
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        
        Image img = go.AddComponent<Image>();
        img.color = Color.white;
        
        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 60);
        rt.anchoredPosition = anchoredPos;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(go.transform, false);
        
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 24;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.black;
        
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        return go;
    }
    
    private static void AddSceneToBuildSettings(string path)
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.path == path) return;
        }
        
        var newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];
        System.Array.Copy(EditorBuildSettings.scenes, newScenes, EditorBuildSettings.scenes.Length);
        newScenes[newScenes.Length - 1] = new EditorBuildSettingsScene(path, true);
        EditorBuildSettings.scenes = newScenes;
        Debug.Log($"Added {path} to Build Settings.");
    }
}

