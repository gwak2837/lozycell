using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float damage = 5f;
    public float lifetime = 2f;
    public float knockbackForce = 0f;
    public bool isHoming = false;

    private Transform target;
    private Vector3 direction;
    private bool isInitialized = false;

    public void Initialize(
        Vector3 dir,
        float dmg,
        float spd,
        float life,
        float knockback = 0f,
        Transform homingTarget = null
    )
    {
        direction = dir.normalized;
        damage = dmg;
        speed = spd;
        lifetime = life;
        knockbackForce = knockback;
        target = homingTarget;
        isHoming = (target != null);
        isInitialized = true;

        Destroy(gameObject, lifetime);

        // Align rotation to direction
        RotateToDirection();
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        if (isHoming && target != null)
        {
            Vector3 targetDir = (target.position - transform.position).normalized;
            // Smooth turn
            direction = Vector3.Lerp(direction, targetDir, Time.deltaTime * 5f).normalized;
            RotateToDirection();
        }

        transform.position += direction * speed * Time.deltaTime;
    }

    private void RotateToDirection()
    {
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            if (knockbackForce > 0)
            {
                enemy.ApplyKnockback(direction, knockbackForce);
            }
            Destroy(gameObject);
        }
        // Could also add wall collision check here
    }
}
