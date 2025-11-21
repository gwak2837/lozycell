using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 1.5f;
    public float damage = 10f;
    public float maxHealth = 20f;
    private float currentHealth;
    private float originalSpeed;
    private bool isSlowed = false;
    private bool isKnockedBack = false;

    private Transform target;

    private void Awake()
    {
        currentHealth = maxHealth;
        originalSpeed = moveSpeed;
    }

    public void Initialize(Transform playerTransform)
    {
        target = playerTransform;
    }

    private void Update()
    {
        if (target == null) return;
        if (isKnockedBack) return;

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

    public void ApplySlow(float factor, float duration)
    {
        if (isSlowed) return; 

        isSlowed = true;
        moveSpeed = originalSpeed * factor;
        
        // Visual feedback
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(0.5f, 0.5f, 1f); // Light Blue

        CancelInvoke(nameof(ResetSpeed));
        Invoke(nameof(ResetSpeed), duration);
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration = 0.2f)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackCoroutine(direction, force, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction, float force, float duration)
    {
        isKnockedBack = true;
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.position += direction * force * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        isKnockedBack = false;
    }

    private void ResetSpeed()
    {
        isSlowed = false;
        moveSpeed = originalSpeed;
        
        // Reset visual
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.white;
    }

    private void Die()
    {
        // Drop Nucleotide
        if (ArcadeManager.Instance != null)
        {
            ArcadeManager.Instance.SpawnDrop(transform.position);
        }

        Destroy(gameObject);
    }
}

