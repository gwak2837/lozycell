using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Composite Skill")]
public class CompositeSkill : SkillStrategy
{
    public List<SkillStrategy> subSkills;

    public override void Activate(PlayerSkillController controller)
    {
        foreach (var skill in subSkills)
        {
            if (skill != null)
            {
                skill.Activate(controller);
            }
        }
    }
}
