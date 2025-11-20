using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

public class ArcadeModeSetup : EditorWindow
{
    [MenuItem("Tools/Arcade Mode Setup")]
    public static void Setup()
    {
        Debug.Log("Starting Arcade Mode Setup...");

        // 1. Ensure Prefabs Directory
        if (!Directory.Exists("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // 2. Create Prefabs
        GameObject enemyPrefab = CreateEnemyPrefab();
        GameObject cloudPrefab = CreateToxicCloudPrefab();
        GameObject slotPrefab = CreateCodonSlotPrefab();

        // 3. Setup Scene Objects
        SetupScene(enemyPrefab, cloudPrefab, slotPrefab);

        Debug.Log("Arcade Mode Setup Complete!");
    }

    private static GameObject CreateEnemyPrefab()
    {
        string path = "Assets/Prefabs/VirusEnemy.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        GameObject go = new GameObject("VirusEnemy");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = Color.red;
        
        // Assign a basic sprite (Knob or UISprite) so it's visible
        // Use built-in "Knob" if available or create a texture
        Texture2D texture = new Texture2D(32, 32);
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }
        texture.Apply();
        
        // We can't save procedural texture into prefab easily without saving asset.
        // Better approach: try to load a standard sprite.
        Sprite knob = Resources.Load<Sprite>("Knob"); // Common Unity sprite
        if (knob == null)
        {
             // Fallback: Try to find ANY sprite or just leave it for user
             // Let's use the same sprite as AminoAcid if possible?
             // Or just create a primitive Quad?
             // SpriteRenderer needs a Sprite.
             // Let's use the "BackgroundSolid" texture or similar if in project?
        }
        
        // Actually, let's just use a primitive Quad for now if no sprite, or create a placeholder sprite asset
        // But to keep it simple in code:
        // We will assume the user has some sprite or we assign 'null' but set color. 
        // Issue: Null sprite = invisible even with color.
        
        // Fix: Load the "SolidBgTexture.png" from the file list if appropriate? No that's BG.
        // Let's check AminoAcid prefab sprite.
        
        string aminoPath = "Assets/AminoAcid.prefab";
        GameObject amino = AssetDatabase.LoadAssetAtPath<GameObject>(aminoPath);
        if (amino != null)
        {
            var aminoSr = amino.GetComponent<SpriteRenderer>();
            if (aminoSr != null) sr.sprite = aminoSr.sprite;
        }
        
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true; // Enemy is a trigger? Or solid? Controller uses TriggerEnter.
        col.radius = 0.5f;

        go.AddComponent<EnemyController>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateToxicCloudPrefab()
    {
        string path = "Assets/Prefabs/ToxicCloud.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        GameObject go = new GameObject("ToxicCloud");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.5f, 0f, 0.5f, 0.4f); // Transparent Purple
        
        // Assign sprite similarly
        string aminoPath = "Assets/AminoAcid.prefab";
        GameObject amino = AssetDatabase.LoadAssetAtPath<GameObject>(aminoPath);
        if (amino != null)
        {
            var aminoSr = amino.GetComponent<SpriteRenderer>();
            if (aminoSr != null) sr.sprite = aminoSr.sprite;
        }

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 1f; // Script handles scale

        go.AddComponent<ToxicCloud>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateCodonSlotPrefab()
    {
        string path = "Assets/Prefabs/CodonSlot.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        GameObject go = new GameObject("CodonSlot");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = Color.white;
        
        // Assign sprite
        string aminoPath = "Assets/AminoAcid.prefab";
        GameObject amino = AssetDatabase.LoadAssetAtPath<GameObject>(aminoPath);
        if (amino != null)
        {
            var aminoSr = amino.GetComponent<SpriteRenderer>();
            if (aminoSr != null) sr.sprite = aminoSr.sprite;
        }
        go.transform.localScale = Vector3.one * 0.5f;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(go.transform);
        textObj.transform.localPosition = Vector3.zero;
        var tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = "";
        tmp.fontSize = 6;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        // Try to load default font asset? TMP usually handles defaults well enough or shows pink.

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static void SetupScene(GameObject enemyPrefab, GameObject cloudPrefab, GameObject slotPrefab)
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PlayerController not found in scene!");
            return;
        }

        // 1. Setup Player
        if (player.GetComponent<PlayerStats>() == null) player.gameObject.AddComponent<PlayerStats>();
        
        CodonRingController ring = player.GetComponentInChildren<CodonRingController>();
        if (ring == null)
        {
            GameObject ringObj = new GameObject("CodonRing");
            ringObj.transform.SetParent(player.transform);
            ringObj.transform.localPosition = Vector3.zero;
            ring = ringObj.AddComponent<CodonRingController>();
        }
        ring.slotPrefab = slotPrefab;

        // 2. Setup Managers
        ArcadeManager arcadeManager = FindFirstObjectByType<ArcadeManager>();
        if (arcadeManager == null)
        {
            GameObject amObj = new GameObject("ArcadeManager");
            arcadeManager = amObj.AddComponent<ArcadeManager>();
        }

        SkillManager skillManager = FindFirstObjectByType<SkillManager>();
        if (skillManager == null)
        {
            GameObject smObj = new GameObject("SkillManager");
            skillManager = smObj.AddComponent<SkillManager>();
        }
        skillManager.toxicCloudPrefab = cloudPrefab;

        EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();
        if (enemySpawner == null)
        {
            GameObject esObj = new GameObject("EnemySpawner");
            enemySpawner = esObj.AddComponent<EnemySpawner>();
        }
        enemySpawner.enemyPrefab = enemyPrefab;

        // 3. Link
        arcadeManager.skillManager = skillManager;
        arcadeManager.enemySpawner = enemySpawner;
        arcadeManager.codonRing = ring;

        EditorUtility.SetDirty(player.gameObject);
        EditorUtility.SetDirty(arcadeManager.gameObject);
        EditorUtility.SetDirty(skillManager.gameObject);
        EditorUtility.SetDirty(enemySpawner.gameObject);
        
        Debug.Log("Scene configured.");
    }
}

