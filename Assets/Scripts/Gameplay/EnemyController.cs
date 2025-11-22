using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 1.5f;
    public float damage = 10f;
    public float maxHealth = 20f;
    public float defense = 0f; // New stat

    private float currentHealth;
    private float originalSpeed;

    // Status Flags
    private bool isSlowed = false;
    private bool isKnockedBack = false;
    private bool isStunned = false; // For Stop codon
    private bool isVulnerable = false; // For Serine (Defense 0 / Vulnerable)

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
        if (target == null || isKnockedBack || isStunned)
            return;

        // Simple chase logic
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Face target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        float effectiveDamage = amount;

        if (isVulnerable)
        {
            effectiveDamage *= 1.5f; // 50% more damage if vulnerable (simulating 0 def or exposed)
        }
        else
        {
            effectiveDamage = Mathf.Max(1, amount - defense);
        }

        currentHealth -= effectiveDamage;

        // Visual Flash
        StartCoroutine(FlashColor(Color.red, 0.1f));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplySlow(float factor, float duration)
    {
        if (isSlowed)
            return;
        StartCoroutine(
            StatusCoroutine(
                duration,
                start: () =>
                {
                    isSlowed = true;
                    moveSpeed = originalSpeed * factor;
                    SetColor(new Color(0.5f, 0.5f, 1f));
                },
                end: () =>
                {
                    isSlowed = false;
                    moveSpeed = originalSpeed;
                    ResetColor();
                }
            )
        );
    }

    public void ApplyStun(float duration)
    {
        if (isStunned)
            return;
        StartCoroutine(
            StatusCoroutine(
                duration,
                start: () =>
                {
                    isStunned = true;
                    SetColor(Color.gray);
                },
                end: () =>
                {
                    isStunned = false;
                    ResetColor();
                }
            )
        );
    }

    public void ApplyVulnerability(float duration)
    {
        if (isVulnerable)
            return;
        StartCoroutine(
            StatusCoroutine(
                duration,
                start: () =>
                {
                    isVulnerable = true;
                    SetColor(Color.cyan);
                },
                end: () =>
                {
                    isVulnerable = false;
                    ResetColor();
                }
            )
        );
    }

    public void ApplyDoT(float dps, float duration)
    {
        StartCoroutine(DoTCoroutine(dps, duration));
    }

    private IEnumerator DoTCoroutine(float dps, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            TakeDamage(dps * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration = 0.2f)
    {
        if (isKnockedBack)
            return;
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

    private IEnumerator StatusCoroutine(float duration, System.Action start, System.Action end)
    {
        start?.Invoke();
        yield return new WaitForSeconds(duration);
        end?.Invoke();
    }

    private void SetColor(Color c)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = c;
    }

    private void ResetColor()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.white;
    }

    private IEnumerator FlashColor(Color c, float duration)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = c;
            yield return new WaitForSeconds(duration);
            sr.color = original;
        }
    }

    private void Die()
    {
        if (ArcadeManager.Instance != null)
        {
            ArcadeManager.Instance.SpawnDrop(transform.position);
        }
        Destroy(gameObject);
    }
}
