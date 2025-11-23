using UnityEngine;

// Composite is optional but let's keep it POCO
public class CompositeSkill : SkillStrategy
{
    // Not heavily used but update if exists
    public SkillStrategy[] subSkills;

    public override void Activate(PlayerSkillController controller, Color skillColor = default)
    {
        if (subSkills == null)
            return;
        foreach (var s in subSkills)
        {
            if (s != null)
                s.Activate(controller, skillColor);
        }
    }
}
