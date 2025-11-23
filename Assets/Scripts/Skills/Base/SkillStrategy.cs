using UnityEngine;

public abstract class SkillStrategy
{
    public string skillName;

    public string description;
    public float cooldown;
    public Sprite icon;

    public abstract void Activate(PlayerSkillController controller, Color skillColor = default);
}
