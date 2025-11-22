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

        // Load Prefabs
        GameObject projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Prefabs/Skills/ProjectileBase.prefab"
        );
        GameObject areaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Prefabs/Skills/ToxicCloudBase.prefab"
        );
        // Placeholders for others if they don't exist, or null
        GameObject petPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Skills/PetBase.prefab");
        GameObject linkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Skills/LinkBase.prefab");

        // 1. Projectile Skills
        CreateProjectileSkill(path, "Ala", "Multishot", 10f, 10f, 5, 30f, false, 0f, false, projectilePrefab);
        CreateProjectileSkill(path, "Val", "Power Shot", 30f, 15f, 1, 0f, false, 5f, false, projectilePrefab); // Knockback
        CreateProjectileSkill(path, "Tyr", "Homing Missiles", 15f, 8f, 3, 45f, true, 0f, false, projectilePrefab);
        CreateProjectileSkill(path, "Pro", "Boomerang", 15f, 10f, 1, 0f, false, 0f, true, projectilePrefab);

        // 2. Area Skills
        // Asp - Acid Pool (Green)
        CreateAreaSkill(path, "Asp", "Acid Pool", 5f, 5f, 2f, true, false, Color.green, areaPrefab);
        // Asn - Grass Knot (Blue/Green) - Slow Zone
        CreateAreaSkill(path, "Asn", "Grass Knot", 0f, 5f, 3f, true, false, new Color(0f, 0.5f, 0.5f), areaPrefab);
        // Trp - Gravity Well (Purple)
        CreateAreaSkill(path, "Trp", "Gravity Well", 0f, 5f, 4f, true, false, new Color(0.5f, 0f, 0.5f), areaPrefab);
        // Arg - Tesla Coil (Yellow) - Follows Player
        CreateAreaSkill(path, "Arg", "Tesla Coil", 10f, 5f, 3f, true, true, Color.yellow, areaPrefab);
        // Met - Methyl Trail (Green)
        CreateAreaSkill(path, "Met", "Methyl Trail", 10f, 5f, 1f, true, false, Color.green, areaPrefab);

        // 3. Buff Skills
        CreateBuffSkill(path, "Glu", "Synaptic Boost", BuffType.SpeedUp, 1.5f, 10f);
        CreateBuffSkill(path, "Leu", "Muscle Up", BuffType.AttackUp, 1.5f, 10f);
        CreateBuffSkill(path, "Phe", "Orbital Shield", BuffType.Shield, 1f, 3f);
        CreateBuffSkill(path, "Gln", "Heal", BuffType.Heal, 33f, 0f);

        // 4. Global Skills
        CreateGlobalSkill(path, "His", "Anaphylaxis", GlobalEffectType.Damage, 50f, 0f);
        CreateGlobalSkill(path, "Ser", "Phospho Mark", GlobalEffectType.DefenseZero, 0f, 10f);
        CreateGlobalSkill(path, "Thr", "Alcohol Burn", GlobalEffectType.DoT, 10f, 5f);
        CreateGlobalSkill(path, "Gly", "Synapse Shutdown", GlobalEffectType.Slow, 0.5f, 10f);
        CreateGlobalSkill(path, "Stop", "Unlimited Void", GlobalEffectType.Stun, 0f, 5f);

        // 5. Chain Skills
        CreateChainSkill(path, "Lys", "Chain Lightning", 20f, 4, linkPrefab);
        CreateChainSkill(path, "Cys", "S-S Death Bond", 15f, 2, linkPrefab);

        // 6. Summon Skills
        CreateSummonSkill(path, "Ile", "Mirror Image", 5f, petPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Skills Generated and Prefabs Assigned (where available) in Assets/Resources/Skills.");

        AssignSkillsToManager(path);
    }

    static void AssignSkillsToManager(string path)
    {
        PlayerSkillController controller = Object.FindFirstObjectByType<PlayerSkillController>();
        if (controller == null)
            return;

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

                if (!fileName.StartsWith("Skill_"))
                    continue;

                controller.skillEntries.Add(
                    new PlayerSkillController.SkillEntry { aminoAcidCode = code, skillData = skill }
                );
            }
        }
        EditorUtility.SetDirty(controller);
    }

    static void CreateProjectileSkill(
        string path,
        string code,
        string name,
        float dmg,
        float spd,
        int count,
        float spread,
        bool homing = false,
        float knockback = 0f,
        bool boomerang = false,
        GameObject prefab = null
    )
    {
        ProjectileSkill skill = ScriptableObject.CreateInstance<ProjectileSkill>();
        skill.name = $"Skill_{code}";
        skill.skillName = name;
        skill.damage = dmg;
        skill.speed = spd;
        skill.projectileCount = count;
        skill.spreadAngle = spread;
        skill.isHoming = homing;
        skill.knockback = knockback;
        skill.isBoomerang = boomerang;
        skill.projectilePrefab = prefab;
        SaveAsset(skill, path, code);
    }

    static void CreateAreaSkill(
        string path,
        string code,
        string name,
        float val,
        float dur,
        float rad,
        bool onPlayer,
        bool attach,
        Color color,
        GameObject prefab = null
    )
    {
        AreaSkill skill = ScriptableObject.CreateInstance<AreaSkill>();
        skill.name = $"Skill_{code}";
        skill.skillName = name;
        skill.effectValue = val;
        skill.duration = dur;
        skill.radius = rad;
        skill.spawnOnPlayer = onPlayer;
        skill.attachToPlayer = attach;
        skill.visualColor = color;
        skill.areaPrefab = prefab;
        SaveAsset(skill, path, code);
    }

    static void CreateBuffSkill(string path, string code, string name, BuffType type, float val, float dur)
    {
        BuffSkill skill = ScriptableObject.CreateInstance<BuffSkill>();
        skill.name = $"Skill_{code}";
        skill.skillName = name;
        skill.buffType = type;
        skill.amount = val;
        skill.duration = dur;
        SaveAsset(skill, path, code);
    }

    static void CreateGlobalSkill(string path, string code, string name, GlobalEffectType type, float val, float dur)
    {
        GlobalEffectSkill skill = ScriptableObject.CreateInstance<GlobalEffectSkill>();
        skill.name = $"Skill_{code}";
        skill.skillName = name;
        skill.effectType = type;
        skill.value = val;
        skill.duration = dur;
        SaveAsset(skill, path, code);
    }

    static void CreateChainSkill(
        string path,
        string code,
        string name,
        float dmg,
        int targets,
        GameObject prefab = null
    )
    {
        ChainSkill skill = ScriptableObject.CreateInstance<ChainSkill>();
        skill.name = $"Skill_{code}";
        skill.skillName = name;
        skill.damage = dmg;
        skill.maxTargets = targets;
        skill.linkPrefab = prefab;
        SaveAsset(skill, path, code);
    }

    static void CreateSummonSkill(string path, string code, string name, float dur, GameObject prefab = null)
    {
        SummonSkill skill = ScriptableObject.CreateInstance<SummonSkill>();
        skill.name = $"Skill_{code}";
        skill.skillName = name;
        skill.duration = dur;
        skill.petPrefab = prefab;
        SaveAsset(skill, path, code);
    }

    static void SaveAsset(ScriptableObject so, string path, string code)
    {
        string fullPath = $"{path}/Skill_{code}.asset";
        var existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(fullPath);

        if (existing == null)
        {
            AssetDatabase.CreateAsset(so, fullPath);
        }
        else
        {
            if (existing.GetType() != so.GetType())
            {
                AssetDatabase.DeleteAsset(fullPath);
                AssetDatabase.CreateAsset(so, fullPath);
            }
            else
            {
                EditorUtility.CopySerialized(so, existing);
                existing.name = so.name;
                EditorUtility.SetDirty(existing);
            }
        }
    }
}
#endif
