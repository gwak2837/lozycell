using UnityEngine;

public static class SkillUtility
{
    public static EnemyController GetClosestEnemy(Vector3 fromPos)
    {
        var enemies = Object.FindObjectsOfType<EnemyController>();
        EnemyController closest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(fromPos, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
    }

    public static Vector3 GetClosestEnemyDir(Vector3 fromPos, Vector3 defaultDir)
    {
        EnemyController closest = GetClosestEnemy(fromPos);
        if (closest != null)
        {
            return (closest.transform.position - fromPos).normalized;
        }
        return defaultDir;
    }
}
