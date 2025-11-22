using System.Collections.Generic;
using UnityEngine;

public static class SkillUtility
{
    public static List<EnemyController> FindEnemiesInRadius(Vector3 center, float radius)
    {
        List<EnemyController> enemies = new List<EnemyController>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);
        foreach (var col in colliders)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }
        return enemies;
    }

    public static List<EnemyController> GetAllEnemies()
    {
        // Assuming EnemySpawner or a Manager keeps track, or expensive FindObjects.
        // For now, fallback to FindObjects or check ArcadeManager if it has a list.
        // ArcadeManager has EnemySpawner. Let's check if EnemySpawner has a list.
        // If not, FindObjectsByType is okay for prototype/small scale.
        // Optimization: Cache this or add a static list in EnemyController.
        return new List<EnemyController>(Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None));
    }

    public static EnemyController FindNearestEnemy(Vector3 position, float range = Mathf.Infinity)
    {
        EnemyController nearest = null;
        float minDist = range * range;

        // Optimization: use LayerMask if possible
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range);

        foreach (var col in colliders)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                float dist = (enemy.transform.position - position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }
        }
        return nearest;
    }
}
