using System.Collections.Generic;
using UnityEngine;

public class SkillDatabase : MonoBehaviour
{
    public static SkillDatabase Instance { get; private set; }

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private GameObject areaPrefab;

    [SerializeField]
    private GameObject petPrefab;

    [SerializeField]
    private GameObject linkPrefab;

    private Dictionary<string, SkillStrategy> skills = new Dictionary<string, SkillStrategy>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitializeSkills();
        Debug.Log($"SkillDatabase Initialized via Code. Loaded {skills.Count} skills.");
    }

    public SkillStrategy GetSkill(string code)
    {
        if (skills.TryGetValue(code, out SkillStrategy skill))
        {
            return skill;
        }
        return null;
    }

    private void InitializeSkills()
    {
        // 1. Projectile Skills
        CreateProjectileSkill(
            "Ala",
            "Multishot",
            GameConfig.Skills.Ala.Damage,
            GameConfig.Skills.Ala.Speed,
            GameConfig.Skills.Ala.Count,
            GameConfig.Skills.Ala.Spread,
            false,
            0f,
            false
        );
        CreateProjectileSkill(
            "Val",
            "Power Shot",
            GameConfig.Skills.Val.Damage,
            GameConfig.Skills.Val.Speed,
            1,
            0f,
            false,
            GameConfig.Skills.Val.Knockback,
            false
        );
        CreateProjectileSkill(
            "Tyr",
            "Homing Missiles",
            GameConfig.Skills.Tyr.Damage,
            GameConfig.Skills.Tyr.Speed,
            GameConfig.Skills.Tyr.Count,
            GameConfig.Skills.Tyr.Spread,
            true,
            0f,
            false
        );
        CreateProjectileSkill(
            "Pro",
            "Boomerang",
            GameConfig.Skills.Pro.Damage,
            GameConfig.Skills.Pro.Speed,
            1,
            0f,
            false,
            0f,
            true
        );

        // 2. Area Skills
        // Asp - Acid Pool (Green)
        CreateAreaSkill(
            "Asp",
            "Acid Pool",
            GameConfig.Skills.Asp.EffectValue,
            GameConfig.Skills.Asp.Duration,
            GameConfig.Skills.Asp.Radius,
            true,
            false,
            new Color(0.2f, 0.8f, 0.2f)
        );
        // Asn - Grass Knot (Blue/Green) - Slow Zone
        CreateAreaSkill(
            "Asn",
            "Grass Knot",
            GameConfig.Skills.Asn.EffectValue,
            GameConfig.Skills.Asn.Duration,
            GameConfig.Skills.Asn.Radius,
            false,
            false,
            new Color(0f, 0.5f, 0.5f)
        );
        // Trp - Gravity Well (Purple)
        CreateAreaSkill(
            "Trp",
            "Gravity Well",
            GameConfig.Skills.Trp.EffectValue,
            GameConfig.Skills.Trp.Duration,
            GameConfig.Skills.Trp.Radius,
            true,
            false,
            new Color(0.5f, 0f, 0.5f)
        );
        // Arg - Tesla Coil (Yellow) - Follows Player
        CreateAreaSkill(
            "Arg",
            "Tesla Coil",
            GameConfig.Skills.Arg.EffectValue,
            GameConfig.Skills.Arg.Duration,
            GameConfig.Skills.Arg.Radius,
            true,
            true,
            Color.yellow
        );
        // Met - Methyl Trail (Green)
        CreateAreaSkill(
            "Met",
            "Methyl Trail",
            GameConfig.Skills.Met.EffectValue,
            GameConfig.Skills.Met.Duration,
            GameConfig.Skills.Met.Radius,
            true,
            false,
            Color.green
        );

        // 3. Buff Skills
        CreateBuffSkill(
            "Glu",
            "Synaptic Boost",
            BuffType.SpeedUp,
            GameConfig.Skills.Glu.Amount,
            GameConfig.Skills.Glu.Duration
        );
        CreateBuffSkill(
            "Leu",
            "Muscle Up",
            BuffType.AttackUp,
            GameConfig.Skills.Leu.Amount,
            GameConfig.Skills.Leu.Duration
        );
        CreateBuffSkill(
            "Phe",
            "Orbital Shield",
            BuffType.Shield,
            GameConfig.Skills.Phe.Amount,
            GameConfig.Skills.Phe.Duration
        );
        CreateBuffSkill("Gln", "Heal", BuffType.Heal, GameConfig.Skills.Gln.Amount, 0f);

        // 4. Global Skills
        CreateGlobalSkill("His", "Anaphylaxis", GlobalEffectType.Damage, GameConfig.Skills.His.Value, 0f);
        CreateGlobalSkill("Ser", "Phospho Mark", GlobalEffectType.DefenseZero, 0f, GameConfig.Skills.Ser.Duration);
        CreateGlobalSkill(
            "Thr",
            "Alcohol Burn",
            GlobalEffectType.DoT,
            GameConfig.Skills.Thr.Value,
            GameConfig.Skills.Thr.Duration
        );
        CreateGlobalSkill(
            "Gly",
            "Synapse Shutdown",
            GlobalEffectType.Slow,
            GameConfig.Skills.Gly.Value,
            GameConfig.Skills.Gly.Duration
        );
        CreateGlobalSkill("Stop", "Unlimited Void", GlobalEffectType.Stun, 0f, GameConfig.Skills.Stop.Duration);

        // 5. Chain Skills
        CreateChainSkill("Lys", "Chain Lightning", GameConfig.Skills.Lys.Damage, GameConfig.Skills.Lys.MaxTargets);
        CreateChainSkill("Cys", "S-S Death Bond", GameConfig.Skills.Cys.Damage, GameConfig.Skills.Cys.MaxTargets);

        // 6. Summon Skills
        CreateSummonSkill("Ile", "Mirror Image", GameConfig.Skills.Ile.Duration);
    }

    // --- Helper Methods ---

    private void CreateProjectileSkill(
        string code,
        string name,
        float dmg,
        float spd,
        int count,
        float spread,
        bool homing,
        float knockback,
        bool boomerang
    )
    {
        var skill = new ProjectileSkill
        {
            skillName = name,
            damage = dmg,
            speed = spd,
            projectileCount = count,
            spreadAngle = spread,
            isHoming = homing,
            knockback = knockback,
            isBoomerang = boomerang,
            projectilePrefab = projectilePrefab,
        };
        skills[code] = skill;
    }

    private void CreateAreaSkill(
        string code,
        string name,
        float effectVal,
        float dur,
        float radius,
        bool spawnOnPlayer,
        bool attachToPlayer,
        Color visualColor
    )
    {
        var skill = new AreaSkill
        {
            skillName = name,
            effectValue = effectVal,
            duration = dur,
            radius = radius,
            spawnOnPlayer = spawnOnPlayer,
            attachToPlayer = attachToPlayer,
            visualColor = visualColor,
            areaPrefab = areaPrefab,
        };
        skills[code] = skill;
    }

    private void CreateBuffSkill(string code, string name, BuffType type, float amount, float dur)
    {
        var skill = new BuffSkill
        {
            skillName = name,
            buffType = type,
            amount = amount,
            duration = dur,
        };
        skills[code] = skill;
    }

    private void CreateGlobalSkill(string code, string name, GlobalEffectType type, float value, float dur)
    {
        var skill = new GlobalEffectSkill
        {
            skillName = name,
            effectType = type,
            value = value,
            duration = dur,
        };
        skills[code] = skill;
    }

    private void CreateChainSkill(string code, string name, float dmg, int count)
    {
        var skill = new ChainSkill
        {
            skillName = name,
            damage = dmg,
            maxTargets = count,
            linkPrefab = linkPrefab,
        };
        skills[code] = skill;
    }

    private void CreateSummonSkill(string code, string name, float dur)
    {
        var skill = new SummonSkill
        {
            skillName = name,
            duration = dur,
            petPrefab = petPrefab,
        };
        skills[code] = skill;
    }
}
