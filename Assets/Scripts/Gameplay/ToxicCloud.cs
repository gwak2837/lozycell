using System.Collections;
using UnityEngine;

public class ToxicCloud : MonoBehaviour
{
    private float damagePerSecond;
    private float duration;
    private float radius;

    public void Initialize(float damage, float durationTime, float areaRadius)
    {
        damagePerSecond = damage;
        duration = durationTime;
        radius = areaRadius;

        // Apply size immediately
        transform.localScale = Vector3.one * radius;
        Destroy(gameObject, duration);
    }

    public void SetColor(Color color)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Keep some transparency if originally intended, or override.
            // Usually effects have alpha. Let's assume color passed has alpha or we multiply.
            // If passed color is opaque (like Color.yellow), we might want to add alpha.
            // But let's just set it for now.
            sr.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }

    private void Start()
    {
        // Ensure it has a trigger collider if not present
        var col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
        }

        // Ensure it has a Rigidbody2D for trigger events (Kinematic)
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Ignore player interaction for now, or allow it to pass through freely.
        // The player shouldn't destroy it or be hurt by it.

        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
