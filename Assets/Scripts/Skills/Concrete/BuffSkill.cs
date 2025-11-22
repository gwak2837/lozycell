using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Buff Skill")]
public class BuffSkill : SkillStrategy
{
    public enum BuffType { Heal, Shield, Speed, Invulnerability }

    [Header("Buff Settings")]
    public BuffType buffType;
    public float duration = 5f;
    public float value = 0f;
    public bool isPercentage = false;

    public override void Activate(PlayerSkillController controller)
    {
        var stats = controller.Stats;
        if (stats == null) return;

        switch (buffType)
        {
            case BuffType.Heal:
                float healAmount = value;
                if (isPercentage) healAmount = stats.maxHealth * (value / 100f);
                stats.Heal(healAmount);
                break;

            case BuffType.Shield:
                stats.EnableShield(duration);
                break;

            case BuffType.Speed:
                stats.SetSpeedMultiplier(value, duration);
                SkillEffects.Instance.CreateElectricAura(controller.transform);
                break;

            case BuffType.Invulnerability:
                stats.EnableInvulnerability(duration);
                break;
        }
    }
}
