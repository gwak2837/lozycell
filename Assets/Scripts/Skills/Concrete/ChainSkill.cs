using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Chain Skill")]
public class ChainSkill : SkillStrategy
{
    public int maxTargets = 4;
    public float jumpRange = 5f;
    public float damage = 10f;
    public GameObject linkPrefab; // Visual Line
    public float linkDuration = 0.5f;

    public override void Activate(PlayerSkillController controller, Color skillColor = default)
    {
        Vector3 currentPos = controller.transform.position;
        List<EnemyController> hitEnemies = new List<EnemyController>();

        // Find first target
        EnemyController currentTarget = SkillUtility.FindNearestEnemy(currentPos, jumpRange);

        if (currentTarget == null)
            return;

        // Start Chain
        for (int i = 0; i < maxTargets; i++)
        {
            if (currentTarget == null)
                break;
            if (hitEnemies.Contains(currentTarget))
                break;

            // Apply Damage
            currentTarget.TakeDamage(damage);
            hitEnemies.Add(currentTarget);

            // Visual Link
            if (linkPrefab != null)
            {
                SpawnLink(currentPos, currentTarget.transform.position, skillColor);
            }

            // Next Step
            currentPos = currentTarget.transform.position;
            currentTarget = FindNextTarget(currentPos, hitEnemies);
        }
    }

    private EnemyController FindNextTarget(Vector3 pos, List<EnemyController> exclude)
    {
        var enemies = SkillUtility.FindEnemiesInRadius(pos, jumpRange);
        EnemyController nearest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (exclude.Contains(enemy))
                continue;

            float d = Vector3.Distance(pos, enemy.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = enemy;
            }
        }
        return nearest;
    }

    private void SpawnLink(Vector3 start, Vector3 end, Color color)
    {
        GameObject link = Instantiate(linkPrefab, start, Quaternion.identity);
        LineRenderer lr = link.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);

            if (color.a > 0)
            {
                lr.startColor = color;
                lr.endColor = new Color(color.r, color.g, color.b, 0f); // Fade out? Or same color
                // Let's make it fade out or same color.
                // Usually lightning effects fade.
            }
        }
        // If no LineRenderer, maybe it stretches?
        // Assuming LineRenderer for now.
        Destroy(link, linkDuration);
    }
}
