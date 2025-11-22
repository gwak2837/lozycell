using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class SkillAssetGenerator : Editor
{
    [MenuItem("Tools/Generate Default Skills")]
    public static void GenerateSkills()
    {
        string path = "Assets/Resources/Skills";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // 1. Group A: Projectile Skills
        CreateProjectileSkill(path, "Gly", "Minigun", 10f, 15f, 0.05f, 10, 15f);
        CreateProjectileSkill(path, "Ala", "Standard Shot", 20f, 12f, 0f, 1, 0f);
        CreateProjectileSkill(path, "Val", "Muscle Up", 40f, 8f, 0f, 1, 0f, 10f);
        CreateProjectileSkill(path, "Phe", "Homing Missiles", 15f, 8f, 0f, 3, 120f, 0f, true); // Homing
        CreateProjectileSkill(path, "Pro", "Boomerang", 15f, 10f, 0f, 1, 0f); // Simple version of Boomerang

        // 2. Group B: Area Skills
        CreateAreaSkill(path, "Ser", "Slow Field", AreaSkill.AreaType.Slow, 0.5f, 4f, Color.cyan);
        CreateAreaSkill(path, "Glu", "Explosion", AreaSkill.AreaType.InstantDamage, 50f, 0f, Color.red);
        CreateAreaSkill(path, "Gln", "Tidal", AreaSkill.AreaType.Push, 3f, 0f, Color.blue);
        CreateAreaSkill(path, "Asp", "Poison Pool", AreaSkill.AreaType.SpawnObject, 15f, 5f, Color.green); // Needs prefab assignment later if strictly following logic

        // 3. Group C: Chain Skills
        CreateChainSkill(path, "Lys", "Chain Lightning", ChainSkill.ChainType.Lightning, 25f, 5);
        CreateChainSkill(path, "Cys", "Laser Link", ChainSkill.ChainType.LaserLink, 10f, 2);

        // 4. Group D: Direct Target
        CreateDirectSkill(path, "Arg", "Thunder Smash", DirectTargetSkill.EffectType.Stun, 50f, 1f);
        CreateDirectSkill(path, "Thr", "Freeze", DirectTargetSkill.EffectType.Freeze, 0f, 3f);
        CreateDirectSkill(path, "Tyr", "Critical Hit", DirectTargetSkill.EffectType.Critical, 100f, 0f);

        // Missing Skills
        CreateProjectileSkill(path, "Trp", "Meteor", 60f, 10f, 0.1f, 3, 20f, 5f); // High damage burst
        CreateProjectileSkill(path, "Leu", "Muscle Up", 40f, 8f, 0f, 1, 0f, 10f);
        CreateProjectileSkill(path, "Ile", "Muscle Up", 40f, 8f, 0f, 1, 0f, 10f);
        CreateProjectileSkill(path, "Asn", "Wave", 15f, 10f, 0f, 5, 60f, 2f); // Fan shot

        // 5. Group E: Buffs
        CreateBuffSkill(path, "His", "Overcharge", BuffSkill.BuffType.Speed, 2f, 5f);

        // 6. Special: Met & Stop
        CreateMetSkill(path);
        CreateStopSkill(path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Skills Generated in Assets/Resources/Skills.");

        // Auto-assign to PlayerSkillController in scene
        AssignSkillsToManager(path);
    }

    static void AssignSkillsToManager(string path)
    {
        // Find PlayerSkillController instead
        PlayerSkillController controller = Object.FindFirstObjectByType<PlayerSkillController>();
        if (controller == null)
        {
            Debug.LogWarning("PlayerSkillController not found in the current scene. Skills created but not assigned.");
            return;
        }

        Undo.RecordObject(controller, "Auto Assign Skills");

        controller.skillEntries = new List<PlayerSkillController.SkillEntry>();
        string[] assetGuids = AssetDatabase.FindAssets("t:SkillStrategy", new[] { path });

        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            SkillStrategy skill = AssetDatabase.LoadAssetAtPath<SkillStrategy>(assetPath);

            if (skill != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                string code = fileName.Replace("Skill_", "");

                if (!assetPath.Contains(".asset"))
                    continue;
                if (!fileName.StartsWith("Skill_"))
                    continue;

                controller.skillEntries.Add(
                    new PlayerSkillController.SkillEntry { aminoAcidCode = code, skillData = skill }
                );
            }
        }

        Debug.Log($"Automatically assigned {controller.skillEntries.Count} skills to PlayerSkillController.");
        EditorUtility.SetDirty(controller);
    }

    static void CreateProjectileSkill(
        string path,
        string code,
        string name,
        float dmg,
        float spd,
        float delay,
        int count,
        float spread,
        float knockback = 0f,
        bool homing = false
    )
    {
        ProjectileSkill skill = ScriptableObject.CreateInstance<ProjectileSkill>();
        skill.skillName = name;
        skill.damage = dmg;
        skill.speed = spd;
        skill.delayBetweenShots = delay;
        skill.projectileCount = count;
        skill.spreadAngle = spread;
        skill.knockback = knockback;
        skill.isHoming = homing;
        skill.color = Color.white;

        SaveAsset(skill, path, code);
    }

    static void CreateAreaSkill(
        string path,
        string code,
        string name,
        AreaSkill.AreaType type,
        float val,
        float dur,
        Color color
    )
    {
        AreaSkill skill = ScriptableObject.CreateInstance<AreaSkill>();
        skill.skillName = name;
        skill.type = type;
        skill.value = val;
        skill.duration = dur;
        skill.visualColor = color;

        SaveAsset(skill, path, code);
    }

    static void CreateChainSkill(string path, string code, string name, ChainSkill.ChainType type, float dmg, int count)
    {
        ChainSkill skill = ScriptableObject.CreateInstance<ChainSkill>();
        skill.skillName = name;
        skill.chainType = type;
        skill.damage = dmg;
        skill.chainCount = count;

        SaveAsset(skill, path, code);
    }

    static void CreateDirectSkill(
        string path,
        string code,
        string name,
        DirectTargetSkill.EffectType type,
        float dmg,
        float dur
    )
    {
        DirectTargetSkill skill = ScriptableObject.CreateInstance<DirectTargetSkill>();
        skill.skillName = name;
        skill.effectType = type;
        skill.damage = dmg;
        skill.duration = dur;

        SaveAsset(skill, path, code);
    }

    static void CreateBuffSkill(string path, string code, string name, BuffSkill.BuffType type, float val, float dur)
    {
        BuffSkill skill = ScriptableObject.CreateInstance<BuffSkill>();
        skill.skillName = name;
        skill.buffType = type;
        skill.value = val;
        skill.duration = dur;

        SaveAsset(skill, path, code);
    }

    static void CreateMetSkill(string path)
    {
        // Met needs Shield + Pet
        BuffSkill shield = ScriptableObject.CreateInstance<BuffSkill>();
        shield.skillName = "Met Shield";
        shield.buffType = BuffSkill.BuffType.Shield;
        shield.duration = 5f;

        SummonSkill pet = ScriptableObject.CreateInstance<SummonSkill>();
        pet.skillName = "Met Pet";
        pet.duration = 10f;

        CompositeSkill met = ScriptableObject.CreateInstance<CompositeSkill>();
        met.skillName = "Met (Start)";
        met.subSkills = new List<SkillStrategy> { shield, pet };

        string assetPath = $"{path}/Skill_Met.asset";
        AssetDatabase.CreateAsset(met, assetPath);

        // Add sub-assets properly
        shield.name = "Met_Shield";
        pet.name = "Met_Pet";
        AssetDatabase.AddObjectToAsset(shield, assetPath);
        AssetDatabase.AddObjectToAsset(pet, assetPath);
    }

    static void CreateStopSkill(string path)
    {
        // Stop needs Heal + Invul + Area Damage
        BuffSkill heal = ScriptableObject.CreateInstance<BuffSkill>();
        heal.skillName = "Renewal Heal";
        heal.buffType = BuffSkill.BuffType.Heal;
        heal.value = 30f;
        heal.isPercentage = true;

        BuffSkill invul = ScriptableObject.CreateInstance<BuffSkill>();
        invul.skillName = "Renewal Invul";
        invul.buffType = BuffSkill.BuffType.Invulnerability;
        invul.duration = 1.5f;

        AreaSkill damage = ScriptableObject.CreateInstance<AreaSkill>();
        damage.skillName = "Renewal Blast";
        damage.type = AreaSkill.AreaType.InstantDamage;
        damage.value = 30f;
        damage.radius = 5f;
        damage.visualColor = Color.green;

        CompositeSkill stop = ScriptableObject.CreateInstance<CompositeSkill>();
        stop.skillName = "Stop (Renewal)";
        stop.subSkills = new List<SkillStrategy> { heal, invul, damage };

        string assetPath = $"{path}/Skill_Stop.asset";
        AssetDatabase.CreateAsset(stop, assetPath);

        heal.name = "Stop_Heal";
        invul.name = "Stop_Invul";
        damage.name = "Stop_Damage";
        AssetDatabase.AddObjectToAsset(heal, assetPath);
        AssetDatabase.AddObjectToAsset(invul, assetPath);
        AssetDatabase.AddObjectToAsset(damage, assetPath);
    }

    static void SaveAsset(ScriptableObject so, string path, string code)
    {
        string fullPath = $"{path}/Skill_{code}.asset";
        // Check if exists
        var existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(fullPath);
        if (existing == null)
        {
            AssetDatabase.CreateAsset(so, fullPath);
        }
        else
        {
            EditorUtility.CopySerialized(so, existing);
        }
    }
}
#endif
