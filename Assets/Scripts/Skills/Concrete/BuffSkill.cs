using UnityEngine;

public enum BuffType
{
    SpeedUp,
    AttackUp,
    Shield,
    Heal,
    Invulnerability,
}

public class BuffSkill : SkillStrategy
{
    public BuffType buffType;
    public float amount; // For speed/attack multiplier, or heal amount
    public float duration;

    public override void Activate(PlayerSkillController controller, Color skillColor = default)
    {
        if (controller.Stats == null)
            return;

        switch (buffType)
        {
            case BuffType.SpeedUp:
                controller.Stats.SetSpeedMultiplier(amount, duration);
                break;
            case BuffType.AttackUp:
                controller.Stats.SetDamageMultiplier(amount, duration);
                break;
            case BuffType.Shield:
                controller.Stats.EnableShield(duration);
                break;
            case BuffType.Heal:
                // Amount for heal is usually flat HP or percentage.
                // Description says "1/3 HP".
                // We can use amount as "percentage" if < 1? Or just flat.
                // Let's assume amount is flat. Or logic here.
                // Gln: "1/3 of missing health".
                // Let's handle Gln logic specifically if needed, or make this generic.
                // If amount is 0.33, treat as ratio?
                // Let's say amount > 1 is flat, <= 1 is ratio of MAX or MISSING?
                // Simple: Amount is flat. For Gln, we configure it with say 33 (if 100 max).
                // Or specialized "HealSkill".
                // Let's use Amount as Flat for now.
                controller.Stats.Heal(amount);
                break;
            case BuffType.Invulnerability:
                controller.Stats.EnableInvulnerability(duration);
                break;
        }
    }
}
