using UnityEngine;
using UnityEditor;

public class InfiniteMapSetupTool : EditorWindow
{
    [MenuItem("Lozycell/Setup Infinite Map")]
    public static void SetupInfiniteMap()
    {
        // 1. Setup Camera Follow
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No Main Camera found!");
            return;
        }

        // Ensure 2D Camera
        cam.orthographic = true;
        cam.orthographicSize = 5f;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("No PlayerController found in the scene!");
            return;
        }

        CameraFollow follow = cam.GetComponent<CameraFollow>();
        if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow>();
        
        follow.target = player.transform;
        Debug.Log("CameraFollow setup complete.");

        // 2. Create Background
        GameObject bg = GameObject.Find("InfiniteBackground");
        if (bg == null)
        {
            bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bg.name = "InfiniteBackground";
        }

        // Remove collider as it's just visual
        if (bg.GetComponent<Collider>()) DestroyImmediate(bg.GetComponent<Collider>());

        // Setup Scale (cover screen)
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        // Make it slightly larger to be safe
        bg.transform.localScale = new Vector3(width * 2, height * 2, 1);
        bg.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 10); // Put it behind

        // Setup InfiniteBackground Script
        InfiniteBackground infBg = bg.GetComponent<InfiniteBackground>();
        if (infBg == null) infBg = bg.gameObject.AddComponent<InfiniteBackground>();
        
        SerializedObject so = new SerializedObject(infBg);
        so.FindProperty("targetToFollow").objectReferenceValue = cam.transform;
        so.FindProperty("parallaxMultiplier").vector2Value = new Vector2(0.5f, 0.5f); // Adjust as needed
        so.ApplyModifiedProperties();

        // 3. Create/Assign Material
        Renderer rend = bg.GetComponent<Renderer>();
        
        // Create a solid color material
        string materialPath = "Assets/Settings/BackgroundSolid.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (mat == null)
        {
             // Create texture
            Texture2D tex = CreateGridTexture(); // Now returns solid color
            string texPath = "Assets/Settings/SolidBgTexture.png";
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(texPath, bytes);
            AssetDatabase.Refresh();
            
            TextureImporter importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
            if (importer != null) {
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            Texture2D savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

            mat = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
            if (mat == null) mat = new Material(Shader.Find("Sprites/Default"));
            
            mat.mainTexture = savedTex;
            AssetDatabase.CreateAsset(mat, materialPath);
        }
        
        rend.material = mat;
        
        // Scale texture to match world units (approx)
        // If Quad is 20x10, and we want 1 tile per unit, scale should be 20, 10.
        rend.material.mainTextureScale = new Vector2(bg.transform.localScale.x, bg.transform.localScale.y);

        // 4. Fix Player Jitter
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            Debug.Log("Player Rigidbody Interpolation set to Interpolate.");
        }

        Debug.Log("Infinite Background setup complete.");
    }

    private static Texture2D CreateGridTexture()
    {
        int size = 4;
        Texture2D texture = new Texture2D(size, size);
        Color bgColor = new Color(0.05f, 0.05f, 0.1f); // Deep dark blue/black

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, bgColor);
            }
        }
        texture.Apply();
        return texture;
    }
}

