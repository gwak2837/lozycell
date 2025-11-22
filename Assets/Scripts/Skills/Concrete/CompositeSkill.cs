using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Composite Skill")]
public class CompositeSkill : SkillStrategy
{
    public List<SkillStrategy> subSkills;

    public override void Activate(PlayerSkillController controller, Color skillColor = default)
    {
        if (subSkills == null)
            return;

        foreach (var skill in subSkills)
        {
            if (skill != null)
            {
                skill.Activate(controller, skillColor);
            }
        }
    }
}
