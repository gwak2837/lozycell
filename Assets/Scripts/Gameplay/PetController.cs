using System.Collections;
using UnityEngine;

public class PetController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private GameObject projectilePrefab;

    private float shootInterval;
    private float damage;

    private Transform player;
    private float duration;
    private Color petColor = Color.white;

    public void Initialize(Transform owner, float dur)
    {
        player = owner;
        duration = dur;

        shootInterval = GameConfig.Pet.ShootInterval;
        damage = GameConfig.Pet.Damage;

        StartCoroutine(BehaviorRoutine());
    }

    public void SetColor(Color color)
    {
        petColor = color;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }

    private IEnumerator BehaviorRoutine()
    {
        float elapsed = 0;
        float lastShot = 0;

        while (elapsed < duration && player != null)
        {
            elapsed += Time.deltaTime;

            // Follow player (orbit or lag)
            // Simple follow with offset
            Vector3 targetPos = player.position + new Vector3(1.5f, 1.5f, 0);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);

            // Shoot
            if (Time.time - lastShot > shootInterval)
            {
                lastShot = Time.time;
                Shoot();
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
            return;

        EnemyController enemy = SkillUtility.FindNearestEnemy(transform.position, 15f);
        Vector3 dir = Vector3.right;
        Transform target = null;

        if (enemy != null)
        {
            dir = (enemy.transform.position - transform.position).normalized;
            target = enemy.transform;
        }
        else
        {
            // Random or Player direction
            if (player != null)
                dir = (player.position - transform.position).normalized;
            // Actually if no enemy, shoot direction of player movement? Or random.
            // Let's shoot Right by default.
        }

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        ProjectileController pc = proj.GetComponent<ProjectileController>();
        if (pc != null)
        {
            pc.Initialize(dir, damage, 12f, 2f, 0f, target, false, transform);
            pc.SetColor(petColor); // Pass pet color to projectile
        }
    }
}
