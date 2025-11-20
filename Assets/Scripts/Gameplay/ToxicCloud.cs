using UnityEngine;
using System.Collections;

public class ToxicCloud : MonoBehaviour
{
    public float damagePerSecond = 5f;
    public float duration = 5f;
    public float radius = 3f;

    private void Start()
    {
        Destroy(gameObject, duration);
        transform.localScale = Vector3.one * radius;
        
        // Ensure it has a trigger collider if not present
        var col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}

