using System.Collections.Generic;
using UnityEngine;

public enum GlobalEffectType
{
    Damage,
    Slow,
    Stun,
    DefenseZero,
    DoT,
}

[CreateAssetMenu(menuName = "Skills/Global Skill")]
public class GlobalEffectSkill : SkillStrategy
{
    public GlobalEffectType effectType;
    public float value; // Damage amount, Slow factor, or DPS
    public float duration; // For status

    public override void Activate(PlayerSkillController controller)
    {
        List<EnemyController> enemies = SkillUtility.GetAllEnemies();

        foreach (var enemy in enemies)
        {
            if (enemy == null)
                continue;

            switch (effectType)
            {
                case GlobalEffectType.Damage:
                    enemy.TakeDamage(value);
                    break;
                case GlobalEffectType.Slow:
                    enemy.ApplySlow(value, duration);
                    break;
                case GlobalEffectType.Stun:
                    enemy.ApplyStun(duration);
                    break;
                case GlobalEffectType.DefenseZero:
                    // DefenseZero means set defense to 0 or make vulnerable.
                    // Assuming EnemyController has ApplyVulnerability or similar.
                    // We added ApplyVulnerability in previous step.
                    enemy.ApplyVulnerability(duration);
                    break;
                case GlobalEffectType.DoT:
                    enemy.ApplyDoT(value, duration);
                    break;
            }
        }
    }
}
