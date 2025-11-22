using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
    private static ProjectileSystem instance;
    public static ProjectileSystem Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬에 없으면 생성해서라도 제공 (싱글톤)
                GameObject obj = new GameObject("ProjectileSystem");
                instance = obj.AddComponent<ProjectileSystem>();
            }
            return instance;
        }
    }

    [Header("Prefabs")]
    public GameObject projectilePrefab;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public GameObject Spawn(Vector3 pos, Vector3 dir, float damage, float speed, float lifetime, Color color, float scale, float knockback = 0f, Transform homingTarget = null)
    {
        GameObject proj = null;
        if (projectilePrefab != null)
        {
            proj = Instantiate(projectilePrefab, pos, Quaternion.identity);
        }
        else
        {
            proj = new GameObject("Projectile");
            proj.transform.position = pos;
            var sr = proj.AddComponent<SpriteRenderer>();
            
            Texture2D tex = new Texture2D(16, 16);
            Color[] colors = new Color[16 * 16];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.white;
            tex.SetPixels(colors);
            tex.Apply();
            
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            sr.color = color;
            
            var col = proj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            
            var rb = proj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.isKinematic = true;

            proj.AddComponent<ProjectileController>();
        }

        if (proj != null)
        {
            var sr = proj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
            
            proj.transform.localScale = Vector3.one * scale;

            var pc = proj.GetComponent<ProjectileController>();
            if (pc == null) pc = proj.AddComponent<ProjectileController>();
            
            pc.Initialize(dir, damage, speed, lifetime, knockback, homingTarget);
        }
        
        return proj;
    }
}

