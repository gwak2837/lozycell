using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Direct Target Skill")]
public class DirectTargetSkill : SkillStrategy
{
    public enum EffectType
    {
        HeavyDamage,
        Freeze,
        Stun,
        Critical,
    }

    [Header("Effect Settings")]
    public EffectType effectType;
    public float damage = 50f;
    public float duration = 1f;
    public float slowAmount = 0f;

    public override void Activate(PlayerSkillController controller)
    {
        EnemyController target = SkillUtility.GetClosestEnemy(controller.transform.position);
        if (target == null)
            return;

        switch (effectType)
        {
            case EffectType.HeavyDamage:
            case EffectType.Critical:
                target.TakeDamage(damage);
                if (effectType == EffectType.Critical && damage > 50)
                    Debug.Log("CRITICAL HIT!");
                SkillEffects.Instance.CreateLightningStrike(target.transform.position);
                break;

            case EffectType.Stun:
                target.TakeDamage(damage);
                target.ApplySlow(0f, duration);
                SkillEffects.Instance.CreateLightningStrike(target.transform.position);
                break;

            case EffectType.Freeze:
                target.ApplySlow(0f, duration);
                var sr = target.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color original = sr.color;
                    sr.color = new Color(0.5f, 0.8f, 1f);
                    controller.StartCoroutine(RestoreColorAfter(sr, original, duration));
                }
                break;
        }
    }

    private System.Collections.IEnumerator RestoreColorAfter(SpriteRenderer sr, Color original, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sr != null)
            sr.color = original;
    }
}
