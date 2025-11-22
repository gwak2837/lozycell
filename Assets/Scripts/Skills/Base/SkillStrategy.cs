using UnityEngine;

public abstract class SkillStrategy : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;
    public float cooldown;

    public abstract void Activate(PlayerSkillController controller);
}
