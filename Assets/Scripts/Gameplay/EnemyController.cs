using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float moveSpeed;
    private float damage;
    private float maxHealth;
    private float defense;
    private float damageInterval;
    private float stopDistance;

    private void OnDrawGizmos()
    {
        // Draw stop distance - Always visible for debugging
        Gizmos.color = Color.yellow;
        // Use config value for Gizmos if called before Awake (Editor time)
        float dist = Application.isPlaying ? stopDistance : GameConfig.Enemy.StopDistance;
        Gizmos.DrawWireSphere(transform.position, dist);
    }

    private float currentHealth;
    private float originalSpeed;

    // Status Flags
    private bool isSlowed = false;
    private bool isKnockedBack = false;
    private bool isStunned = false; // For Stop codon
    private bool isVulnerable = false; // For Serine (Defense 0 / Vulnerable)
    private bool isTouchingPlayer = false;
    private Coroutine damageCoroutine;

    private Transform target;

    private void Awake()
    {
        // Initialize from Config
        moveSpeed = GameConfig.Enemy.DefaultMoveSpeed;
        damage = GameConfig.Enemy.DefaultDamage;
        maxHealth = GameConfig.Enemy.DefaultMaxHealth;
        defense = GameConfig.Enemy.DefaultDefense;
        damageInterval = GameConfig.Enemy.DamageInterval;
        stopDistance = GameConfig.Enemy.StopDistance;

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

        if (isTouchingPlayer)
        {
            // If touching player, don't move independently.
            // Let the physics system or parent transform handle position if we were to parent it.
            // Since we want it to "stick" rigidly, parenting is the easiest way.
            // However, if we just want it to follow exactly without parenting physics issues:
            // We can just do nothing here and let the transform be updated in LateUpdate or similar,
            // BUT if we want it to rotate WITH the player like the shield slots, parenting is best.
            return;
        }

        // Simple chase logic
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 direction = (target.position - transform.position).normalized;

        // Only move if not too close
        if (distance > stopDistance)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        // Face target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            if (!isTouchingPlayer)
            {
                isTouchingPlayer = true;
                // Rigidly stick to player by becoming a child
                transform.SetParent(playerStats.transform);

                // Pull closer to ensure it looks "stuck" to the visual edge if needed
                // Assuming Visual Radius is larger than Collider Radius.
                // We can just keep current position, OR force it to a specific radius.
                // But since we want "edge", current position is likely correct (Collider Edge).

                // Optional: Disable RB or Collider to prevent physics jitter if needed,
                // but we need Collider for taking damage from player projectiles.
                // Freezing RB constraints might be good.
                var rb = GetComponent<Rigidbody2D>();
                if (rb)
                    rb.bodyType = RigidbodyType2D.Kinematic;

                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(DealContactDamage(playerStats));
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // If we are parented, we might not exit the trigger naturally unless the player dies or we die.
        // So this might be less relevant once stuck.
        // But keep it for safety or if detached logic is added later.
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null && transform.parent != playerStats.transform)
        {
            isTouchingPlayer = false;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DealContactDamage(PlayerStats playerStats)
    {
        while (true)
        {
            if (playerStats != null && playerStats.gameObject.activeSelf)
            {
                playerStats.TakeDamage(damage);
            }
            yield return new WaitForSeconds(damageInterval);
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

        // Show Floating Text
        if (FloatingTextManager.Instance != null)
        {
            FloatingTextManager.Instance.Show(effectiveDamage, transform.position, isVulnerable);
        }

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
        ArcadeManager.Instance.HandleEnemyDeath(transform.position);
        Destroy(gameObject);
    }
}
