using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : SkillStrategy
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public int projectileCount = 1;
    public float damage = 10f;
    public float speed = 10f;
    public float lifetime = 2f;
    public float spreadAngle = 15f;

    [Header("Behaviors")]
    public bool isHoming = false;
    public bool isBoomerang = false;
    public float knockback = 0f;

    public override void Activate(PlayerSkillController controller)
    {
        if (projectilePrefab == null)
            return;

        Vector3 spawnPos = controller.transform.position;

        // Basic direction: Forward (or towards nearest enemy?)
        // Arcade style usually targets nearest or faces movement.
        // Let's try finding nearest enemy first, else face forward/random.
        // Actually, Plan implies auto-targeting or facing.
        // Ala (Multishot) -> usually forward/random.
        // Tyr (Homing) -> seeks.

        // Determine base direction
        Vector3 baseDir = Vector3.up;
        EnemyController nearest = SkillUtility.FindNearestEnemy(spawnPos, 20f);

        if (nearest != null)
        {
            baseDir = (nearest.transform.position - spawnPos).normalized;
        }
        else
        {
            // Use input direction or random?
            // Let's use random if no enemy, or just UP.
            baseDir = Random.insideUnitCircle.normalized;
            if (baseDir == Vector3.zero)
                baseDir = Vector3.up;
        }

        float startAngle = -spreadAngle * (projectileCount - 1) / 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = startAngle + (spreadAngle * i);
            Quaternion rotation = Quaternion.AngleAxis(angleOffset, Vector3.forward);
            Vector3 finalDir = rotation * baseDir;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            ProjectileController pc = proj.GetComponent<ProjectileController>();

            Transform target = (isHoming && nearest != null) ? nearest.transform : null;

            if (pc != null)
            {
                pc.Initialize(finalDir, damage, speed, lifetime, knockback, target, isBoomerang, controller.transform);
            }
        }
    }
}
