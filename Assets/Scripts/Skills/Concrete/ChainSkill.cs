using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Chain Skill")]
public class ChainSkill : SkillStrategy
{
    public enum ChainType
    {
        Lightning,
        LaserLink,
    }

    [Header("Chain Settings")]
    public ChainType chainType;
    public int chainCount = 5;
    public float damage = 25f;
    public float range = 10f;

    [Header("Laser Link Settings")]
    public float linkDuration = 2f;
    public float tickRate = 0.2f;

    public override void Activate(PlayerSkillController controller)
    {
        Vector3 startPos = controller.transform.position;

        // Get enemies logic - could move to Utility if reused often
        List<EnemyController> enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
        enemies.Sort(
            (a, b) =>
                Vector3
                    .Distance(startPos, a.transform.position)
                    .CompareTo(Vector3.Distance(startPos, b.transform.position))
        );

        if (chainType == ChainType.Lightning)
        {
            int count = Mathf.Min(chainCount, enemies.Count);
            for (int i = 0; i < count; i++)
            {
                enemies[i].TakeDamage(damage);

                if (i > 0)
                    SkillEffects.Instance.CreateLightningLine(
                        enemies[i - 1].transform.position,
                        enemies[i].transform.position
                    );
                else
                    SkillEffects.Instance.CreateLightningLine(startPos, enemies[i].transform.position);
            }
        }
        else if (chainType == ChainType.LaserLink)
        {
            if (enemies.Count >= 2)
            {
                controller.StartCoroutine(LaserLinkCoroutine(enemies[0], enemies[1]));
            }
            else if (enemies.Count == 1)
            {
                controller.StartCoroutine(LaserLinkPlayerCoroutine(controller, enemies[0]));
            }
        }
    }

    private IEnumerator LaserLinkCoroutine(EnemyController e1, EnemyController e2)
    {
        float elapsed = 0;
        float lastTick = 0;

        GameObject laserLine = SkillEffects.Instance.CreateLaserLine(e1.transform.position, e2.transform.position);
        LineRenderer lr = laserLine.GetComponent<LineRenderer>();

        while (elapsed < linkDuration && e1 != null && e2 != null)
        {
            elapsed += Time.deltaTime;

            lr.SetPosition(0, e1.transform.position);
            lr.SetPosition(1, e2.transform.position);

            if (elapsed - lastTick > tickRate)
            {
                lastTick = elapsed;
                e1.TakeDamage(damage);
                e2.TakeDamage(damage);
            }

            yield return null;
        }

        Destroy(laserLine);
    }

    private IEnumerator LaserLinkPlayerCoroutine(PlayerSkillController controller, EnemyController enemy)
    {
        float elapsed = 0;
        float lastTick = 0;

        GameObject laserLine = SkillEffects.Instance.CreateLaserLine(
            controller.transform.position,
            enemy.transform.position
        );
        LineRenderer lr = laserLine.GetComponent<LineRenderer>();

        while (elapsed < linkDuration && enemy != null && controller != null)
        {
            elapsed += Time.deltaTime;

            lr.SetPosition(0, controller.transform.position);
            lr.SetPosition(1, enemy.transform.position);

            if (elapsed - lastTick > tickRate)
            {
                lastTick = elapsed;
                enemy.TakeDamage(damage);
            }

            yield return null;
        }

        Destroy(laserLine);
    }
}
