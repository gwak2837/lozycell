using UnityEngine;

public abstract class SkillStrategy : ScriptableObject
{
    [Header("Base Settings")]
    public string skillName;

    [TextArea]
    public string description;
    public float cooldown;
    public Sprite icon;

    public abstract void Activate(PlayerSkillController controller, Color skillColor = default);
}
