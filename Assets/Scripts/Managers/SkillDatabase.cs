using System.Collections.Generic;
using UnityEngine;

public class SkillDatabase : MonoBehaviour
{
    public static SkillDatabase Instance { get; private set; }

    // Prefabs loaded from Resources
    private GameObject projectilePrefab;
    private GameObject areaPrefab;
    private GameObject petPrefab;
    private GameObject linkPrefab;

    private Dictionary<string, SkillStrategy> skills = new Dictionary<string, SkillStrategy>();

    // [RuntimeInitializeOnLoadMethod] allows this to run without being in the scene manually
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Create a hidden GameObject to host the Database
        GameObject go = new GameObject("SkillDatabase");
        Instance = go.AddComponent<SkillDatabase>();
        Object.DontDestroyOnLoad(go); // Persist across scenes
    }

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

        LoadResources();
        InitializeSkills();
        Debug.Log($"SkillDatabase Initialized via Code. Loaded {skills.Count} skills.");
    }

    private void LoadResources()
    {
        // Load prefabs from Assets/Resources/Prefabs/Skills/
        // Note: Do NOT include file extensions (.prefab) in the path
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Skills/ProjectileBase");
        areaPrefab = Resources.Load<GameObject>("Prefabs/Skills/ToxicCloudBase");
        petPrefab = Resources.Load<GameObject>("Prefabs/Skills/PetBase");
        linkPrefab = Resources.Load<GameObject>("Prefabs/Skills/LinkBase");

        if (projectilePrefab == null)
            Debug.LogError("Failed to load ProjectileBase from Resources!");
        if (areaPrefab == null)
            Debug.LogError("Failed to load ToxicCloudBase from Resources!");
        // Pet and Link might be optional depending on skills, but good to warn
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
        CreateProjectileSkill("Ala", "Multishot", 10f, 10f, 5, 30f, false, 0f, false);
        CreateProjectileSkill("Val", "Power Shot", 30f, 15f, 1, 0f, false, 5f, false);
        CreateProjectileSkill("Tyr", "Homing Missiles", 15f, 8f, 3, 45f, true, 0f, false);
        CreateProjectileSkill("Pro", "Boomerang", 15f, 10f, 1, 0f, false, 0f, true);

        // 2. Area Skills
        // Asp - Acid Pool (Green)
        CreateAreaSkill("Asp", "Acid Pool", 5f, 5f, 2f, true, false, new Color(0.2f, 0.8f, 0.2f));
        // Asn - Grass Knot (Blue/Green) - Slow Zone
        CreateAreaSkill("Asn", "Grass Knot", 0f, 5f, 3f, false, false, new Color(0f, 0.5f, 0.5f));
        // Trp - Gravity Well (Purple)
        CreateAreaSkill("Trp", "Gravity Well", 0f, 5f, 4f, true, false, new Color(0.5f, 0f, 0.5f));
        // Arg - Tesla Coil (Yellow) - Follows Player
        CreateAreaSkill("Arg", "Tesla Coil", 10f, 5f, 3f, true, true, Color.yellow);
        // Met - Methyl Trail (Green)
        CreateAreaSkill("Met", "Methyl Trail", 10f, 5f, 1f, true, false, Color.green);

        // 3. Buff Skills
        CreateBuffSkill("Glu", "Synaptic Boost", BuffType.SpeedUp, 1.5f, 10f);
        CreateBuffSkill("Leu", "Muscle Up", BuffType.AttackUp, 1.5f, 10f);
        CreateBuffSkill("Phe", "Orbital Shield", BuffType.Shield, 1f, 3f);
        CreateBuffSkill("Gln", "Heal", BuffType.Heal, 33f, 0f);

        // 4. Global Skills
        CreateGlobalSkill("His", "Anaphylaxis", GlobalEffectType.Damage, 50f, 0f);
        CreateGlobalSkill("Ser", "Phospho Mark", GlobalEffectType.DefenseZero, 0f, 10f);
        CreateGlobalSkill("Thr", "Alcohol Burn", GlobalEffectType.DoT, 10f, 5f);
        CreateGlobalSkill("Gly", "Synapse Shutdown", GlobalEffectType.Slow, 0.5f, 10f);
        CreateGlobalSkill("Stop", "Unlimited Void", GlobalEffectType.Stun, 0f, 5f);

        // 5. Chain Skills
        CreateChainSkill("Lys", "Chain Lightning", 20f, 4);
        CreateChainSkill("Cys", "S-S Death Bond", 15f, 2);

        // 6. Summon Skills
        CreateSummonSkill("Ile", "Mirror Image", 5f);
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
