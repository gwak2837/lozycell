using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private float speed;
    private float damage;
    private float lifetime;
    private float knockbackForce;
    private bool isHoming;
    private bool isBoomerang;

    private Transform target;
    private Transform owner; // For boomerang return
    private Vector3 direction;
    private bool isInitialized = false;

    // Boomerang state
    private float distanceTraveled = 0f;
    private bool returning = false;

    public void SetColor(Color color)
    {
        // Fail Fast: Require SpriteRenderer
        GetComponent<SpriteRenderer>().color = color;

        // If there's a TrailRenderer, tint it too (Optional but common)
        var tr = GetComponent<TrailRenderer>();
        if (tr != null)
        {
            tr.startColor = color;
            tr.endColor = new Color(color.r, color.g, color.b, 0f);
        }
    }

    public void Initialize(
        Vector3 dir,
        float dmg,
        float spd,
        float life,
        float knockback = 0f,
        Transform homingTarget = null,
        bool boomerang = false,
        Transform shooter = null
    )
    {
        direction = dir.normalized;
        damage = dmg;
        speed = spd;
        lifetime = life;
        knockbackForce = knockback;
        target = homingTarget;
        isHoming = (target != null);
        isBoomerang = boomerang;
        owner = shooter;

        isInitialized = true;

        Destroy(gameObject, lifetime);
        RotateToDirection();
    }

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        Vector3 moveStep = Vector3.zero;

        if (isBoomerang)
        {
            HandleBoomerang();
        }
        else if (isHoming && target != null)
        {
            Vector3 targetDir = (target.position - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDir, Time.deltaTime * 5f).normalized;
            moveStep = direction * speed * Time.deltaTime;
            RotateToDirection();
            transform.position += moveStep;
        }
        else
        {
            moveStep = direction * speed * Time.deltaTime;
            transform.position += moveStep;
        }
    }

    private void HandleBoomerang()
    {
        if (!returning)
        {
            Vector3 step = direction * speed * Time.deltaTime;
            transform.position += step;
            distanceTraveled += step.magnitude;

            if (distanceTraveled >= speed * (lifetime * 0.4f)) // Return after 40% lifetime approx
            {
                returning = true;
            }
        }
        else
        {
            if (owner != null)
            {
                Vector3 toOwner = (owner.position - transform.position).normalized;
                transform.position += toOwner * speed * Time.deltaTime;

                // Destroy if returned
                if (Vector3.Distance(transform.position, owner.position) < 0.5f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // Owner dead? Just go back opposite way or destroy
                Destroy(gameObject);
            }
        }
        transform.Rotate(0, 0, 360 * Time.deltaTime * 2); // Spin effect
    }

    private void RotateToDirection()
    {
        if (direction != Vector3.zero && !isBoomerang)
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

            // Multishot/Normal destroy on hit, Boomerang penetrates (doesn't destroy)
            if (!isBoomerang)
            {
                Destroy(gameObject);
            }
        }
    }
}
