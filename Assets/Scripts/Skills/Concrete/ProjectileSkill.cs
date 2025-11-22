using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : SkillStrategy
{
    [Header("Projectile Settings")]
    public float damage = 10f;
    public float speed = 15f;
    public float lifetime = 2f;
    public float scale = 1f;
    public Color color = Color.white;
    public float knockback = 0f;
    public bool isHoming = false;
    
    [Header("Pattern Settings")]
    public int projectileCount = 1;
    public float spreadAngle = 0f;
    public float delayBetweenShots = 0f;

    public override void Activate(PlayerSkillController controller)
    {
        if (delayBetweenShots > 0)
        {
            controller.StartCoroutine(FireRoutine(controller));
        }
        else
        {
            FireOnce(controller);
        }
    }

    private void FireOnce(PlayerSkillController controller)
    {
        Vector3 startPos = controller.transform.position;
        Vector3 targetDir = SkillUtility.GetClosestEnemyDir(startPos, controller.transform.up);
        Transform targetTransform = isHoming ? SkillUtility.GetClosestEnemy(startPos)?.transform : null;
        
        if (projectileCount == 1)
        {
            ProjectileSystem.Instance.Spawn(startPos, targetDir, damage, speed, lifetime, color, scale, knockback, targetTransform);
        }
        else
        {
            float startAngle = -spreadAngle / 2f;
            float angleStep = spreadAngle / (projectileCount - 1);
            
            for (int i = 0; i < projectileCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * targetDir;
                ProjectileSystem.Instance.Spawn(startPos, dir, damage, speed, lifetime, color, scale, knockback, targetTransform);
            }
        }
    }

    private IEnumerator FireRoutine(PlayerSkillController controller)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 startPos = controller.transform.position;
            Vector3 targetDir = SkillUtility.GetClosestEnemyDir(startPos, controller.transform.up);
            
            if (spreadAngle > 0)
            {
                float randomAngle = Random.Range(-spreadAngle, spreadAngle);
                targetDir = Quaternion.Euler(0, 0, randomAngle) * targetDir;
            }
            
            Transform targetTransform = isHoming ? SkillUtility.GetClosestEnemy(startPos)?.transform : null;
            ProjectileSystem.Instance.Spawn(startPos, targetDir, damage, speed, lifetime, color, scale, knockback, targetTransform);
            
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }
}
