using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float damage = 10f;
    public float maxHealth = 20f;
    private float currentHealth;

    private Transform target;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(Transform playerTransform)
    {
        target = playerTransform;
    }

    private void Update()
    {
        if (target == null) return;

        // Simple chase logic
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Face target (optional)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If hits player
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
            // Destroy enemy on impact? Or bounce? 
            // "Bio Defense" usually implies continuous contact or suicide bombing. 
            // Let's destroy for now to keep it clean, or push back.
            // Plan says "deals contact damage".
            // Let's destroy it to prevent immediate re-triggering and emulate "infecting".
            Destroy(gameObject); 
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Optional: Spawn effect or drop item
        Destroy(gameObject);
    }
}

