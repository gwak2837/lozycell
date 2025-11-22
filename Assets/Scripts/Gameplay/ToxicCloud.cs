using System.Collections;
using UnityEngine;

public class ToxicCloud : MonoBehaviour
{
    public float damagePerSecond = 5f;
    public float duration = 5f;
    public float radius = 3f;

    public void Initialize(float damage, float durationTime, float areaRadius)
    {
        damagePerSecond = damage;
        duration = durationTime;
        radius = areaRadius;

        // Apply size immediately
        transform.localScale = Vector3.one * radius;
        Destroy(gameObject, duration);
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
            rb.isKinematic = true;
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
