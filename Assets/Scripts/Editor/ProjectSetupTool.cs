using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class ProjectSetupTool : EditorWindow
{
    [MenuItem("Lozycell/Auto Setup Project")]
    public static void ShowWindow()
    {
        GetWindow<ProjectSetupTool>("Project Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Lozycell Auto Setup", EditorStyles.boldLabel);

        if (GUILayout.Button("1. Add Tags & Layers"))
        {
            AddTag("AminoAcid");
        }

        if (GUILayout.Button("2. Create Base Scene"))
        {
            SetupBaseScene();
        }

        if (GUILayout.Button("3. Create Arcade Scene"))
        {
            SetupArcadeScene();
        }

        if (GUILayout.Button("4. Add Scenes to Build Settings"))
        {
            AddScenesToBuildSettings();
        }

        if (GUILayout.Button("5. Create Skill Assets (Prefabs)"))
        {
            CreateSkillAssets();
        }
    }

    private static void CreateSkillAssets()
    {
        // Ensure folders exist
        if (!Directory.Exists("Assets/Prefabs")) Directory.CreateDirectory("Assets/Prefabs");
        if (!Directory.Exists("Assets/Prefabs/Skills")) Directory.CreateDirectory("Assets/Prefabs/Skills");

        // 1. Create Projectile Prefab
        GameObject projObj = new GameObject("ProjectileBase");

        // Visuals
        SpriteRenderer sr = projObj.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"); // Built-in circle
        sr.color = Color.white;

        // Physics
        CircleCollider2D col = projObj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;

        Rigidbody2D rb = projObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Logic
        projObj.AddComponent<ProjectileController>();

        // Save Prefab
        string projPath = "Assets/Prefabs/Skills/ProjectileBase.prefab";
        PrefabUtility.SaveAsPrefabAsset(projObj, projPath);
        DestroyImmediate(projObj);
        Debug.Log($"Created Projectile Prefab at {projPath}");

        // 2. Create Toxic Cloud Prefab
        GameObject cloudObj = new GameObject("ToxicCloudBase");

        SpriteRenderer cloudSr = cloudObj.AddComponent<SpriteRenderer>();
        cloudSr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"); // Square/Box
        cloudSr.color = new Color(0.5f, 0f, 0.5f, 0.4f); // Transparent Purple
        cloudObj.transform.localScale = new Vector3(3, 3, 1);

        CircleCollider2D cloudCol = cloudObj.AddComponent<CircleCollider2D>();
        cloudCol.isTrigger = true;
        cloudCol.radius = 0.5f; // Scales with object

        cloudObj.AddComponent<ToxicCloud>();

        string cloudPath = "Assets/Prefabs/Skills/ToxicCloudBase.prefab";
        PrefabUtility.SaveAsPrefabAsset(cloudObj, cloudPath);
        DestroyImmediate(cloudObj);
        Debug.Log($"Created Toxic Cloud Prefab at {cloudPath}");

        // 3. Assign to PlayerSkillController in Scene (if open)
        PlayerSkillController psc = Object.FindFirstObjectByType<PlayerSkillController>();
        if (psc != null)
        {
            // No longer holding prefabs directly in controller, handled by singletons or concrete skills
            // But we might want to refresh references if needed.
            // ProjectileSystem.Instance.projectilePrefab = ... (if we want to assign here, but ProjectileSystem is singleton in scene)

            // Let's find ProjectileSystem and assign
            ProjectileSystem ps = Object.FindFirstObjectByType<ProjectileSystem>();
            if (ps == null)
            {
                GameObject psObj = new GameObject("ProjectileSystem");
                ps = psObj.AddComponent<ProjectileSystem>();
            }
            ps.projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(projPath);
            EditorUtility.SetDirty(ps);

            Debug.Log("Auto-assigned prefabs to active ProjectileSystem.");
        }
    }

    private static void AddTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag)) { found = true; break; }
        }

        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tag;
            tagManager.ApplyModifiedProperties();
            Debug.Log("Tag '" + tag + "' added.");
        }
        else
        {
            Debug.Log("Tag '" + tag + "' already exists.");
        }
    }

    private static void SetupBaseScene()
    {
        string scenePath = "Assets/Scenes/BaseScene.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Camera
        GameObject cam = new GameObject("Main Camera");
        cam.AddComponent<Camera>();
        cam.AddComponent<UniversalAdditionalCameraData>();
        cam.AddComponent<AudioListener>();
        cam.tag = "MainCamera";
        cam.transform.position = new Vector3(0, 0, -10);

        // 2. GameManager
        GameObject gm = new GameObject("GameManager");
        GameManager gmScript = gm.AddComponent<GameManager>();

        // 3. BaseManager
        GameObject bm = new GameObject("BaseManager");
        BaseManager bmScript = bm.AddComponent<BaseManager>();

        // 4. UI
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // UI Elements
        bmScript.aminoAcidText = CreateTMPText(canvasGO, "AminoText", "Amino Acids: 0", new Vector2(0, 400));
        bmScript.levelText = CreateTMPText(canvasGO, "LevelText", "Level: 1", new Vector2(0, 350));
        bmScript.attackText = CreateTMPText(canvasGO, "AttackText", "Attack: 10", new Vector2(0, 300));

        Button upgradeBtn = CreateButton(canvasGO, "UpgradeBtn", "Upgrade (100 AA)", new Vector2(-150, 0));
        bmScript.upgradeButton = upgradeBtn;

        Button arcadeBtn = CreateButton(canvasGO, "ArcadeBtn", "Go to Arcade", new Vector2(150, 0));
        bmScript.startArcadeButton = arcadeBtn;

        // Save
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("BaseScene created at " + scenePath);
    }

    private static void SetupArcadeScene()
    {
        string scenePath = "Assets/Scenes/ArcadeScene.unity";
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Camera
        GameObject cam = new GameObject("Main Camera");
        cam.AddComponent<Camera>();
        cam.AddComponent<UniversalAdditionalCameraData>();
        cam.AddComponent<AudioListener>();
        cam.tag = "MainCamera";
        cam.transform.position = new Vector3(0, 0, -10);
        cam.GetComponent<Camera>().backgroundColor = Color.black;
        cam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

        // 2. ArcadeManager
        GameObject am = new GameObject("ArcadeManager");
        ArcadeManager amScript = am.AddComponent<ArcadeManager>();

        // 3. Player
        GameObject player = new GameObject("Player");
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"); // Simple circle
        sr.color = Color.cyan;
        player.transform.localScale = new Vector3(2, 2, 1);

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        BoxCollider2D col = player.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1, 1);

        player.AddComponent<PlayerController>();

        // 4. Amino Acid Prefab
        GameObject aminoObj = new GameObject("AminoAcid");
        SpriteRenderer asr = aminoObj.AddComponent<SpriteRenderer>();
        asr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        asr.color = Color.yellow;

        CircleCollider2D ac = aminoObj.AddComponent<CircleCollider2D>();
        ac.isTrigger = true;

        aminoObj.tag = "AminoAcid";
        aminoObj.AddComponent<GeneticBase>();

        // Create Prefab
        string prefabPath = "Assets/AminoAcid.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(aminoObj, prefabPath);
        Object.DestroyImmediate(aminoObj);

        // Assign to Manager
        amScript.aminoAcidPrefab = prefab;

        // 5. Spawn Center
        GameObject spawnCenter = new GameObject("SpawnCenter");
        amScript.spawnArea = spawnCenter.transform;

        // 6. UI
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        amScript.progressText = CreateTMPText(canvasGO, "ProgressText", "0 / 100", new Vector2(0, 450));

        // Save
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("ArcadeScene created at " + scenePath);
    }

    private static void AddScenesToBuildSettings()
    {
        EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
        List<EditorBuildSettingsScene> newSettings = new List<EditorBuildSettingsScene>(original);

        string[] scenesToAdd = new string[] { "Assets/Scenes/BaseScene.unity", "Assets/Scenes/ArcadeScene.unity" };

        foreach (string scenePath in scenesToAdd)
        {
            bool exists = false;
            foreach (var existingScene in original)
            {
                if (existingScene.path == scenePath)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                newSettings.Add(new EditorBuildSettingsScene(scenePath, true));
            }
        }

        EditorBuildSettings.scenes = newSettings.ToArray();
        Debug.Log("Scenes added to Build Settings.");
    }

    // Helpers
    private static TextMeshProUGUI CreateTMPText(GameObject parent, string name, string content, Vector2 position)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(400, 50);

        return tmp;
    }

    private static Button CreateButton(GameObject parent, string name, string content, Vector2 position)
    {
        // Background
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        Image img = go.AddComponent<Image>();
        img.color = Color.white;

        Button btn = go.AddComponent<Button>();

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(200, 60);

        // Text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        return btn;
    }
}
