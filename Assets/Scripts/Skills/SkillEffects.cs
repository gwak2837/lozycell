using UnityEngine;

public class SkillEffects : MonoBehaviour
{
    private static SkillEffects instance;
    public static SkillEffects Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("SkillEffects");
                instance = obj.AddComponent<SkillEffects>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void CreateVisualRing(Vector3 position, float radius, Color color)
    {
        GameObject ring = new GameObject("VisualRing");
        ring.transform.position = position;

        var sr = ring.AddComponent<SpriteRenderer>();
        sr.color = new Color(color.r, color.g, color.b, 0.3f);

        Texture2D tex = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        Vector2 center = new Vector2(32, 32);

        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist < 32 && dist > 28)
                    pixels[y * 64 + x] = Color.white;
                else
                    pixels[y * 64 + x] = Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 32);

        ring.transform.localScale = Vector3.one * (radius / 2f);
        Destroy(ring, 0.5f);
    }

    public void CreateLightningLine(Vector3 from, Vector3 to)
    {
        GameObject line = new GameObject("LightningLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();

        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.yellow;
        lr.endColor = Color.yellow;

        lr.SetPosition(0, from);
        lr.SetPosition(1, to);

        Destroy(line, 0.2f);
    }

    public void CreateLightningStrike(Vector3 position)
    {
        GameObject strike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        strike.transform.position = position;
        strike.transform.localScale = new Vector3(0.5f, 5f, 0.5f);
        Destroy(strike.GetComponent<Collider>());

        var renderer = strike.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.yellow;
        renderer.material = mat;

        Destroy(strike, 0.3f);
    }

    public void CreateElectricAura(Transform target)
    {
        GameObject aura = new GameObject("ElectricAura");
        aura.transform.SetParent(target);
        aura.transform.localPosition = Vector3.zero;

        var sr = aura.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 1f, 0f, 0.3f);

        Texture2D tex = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        Vector2 center = new Vector2(32, 32);

        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist < 30)
                    pixels[y * 64 + x] = Color.white;
                else
                    pixels[y * 64 + x] = Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 32);

        aura.transform.localScale = Vector3.one * 2f;
        Destroy(aura, 5f);
    }

    public GameObject CreateLaserLine(Vector3 from, Vector3 to)
    {
        GameObject line = new GameObject("LaserLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();

        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0.8f, 0.8f, 0f);
        lr.endColor = new Color(0.8f, 0.8f, 0f);

        lr.SetPosition(0, from);
        lr.SetPosition(1, to);

        return line;
    }
}
