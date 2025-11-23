using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    // State Management
    public GameObject ActivePet { get; private set; }

    public PlayerController Player { get; private set; }
    public PlayerStats Stats { get; private set; }

    private void Awake()
    {
        Player = GetComponent<PlayerController>();
        Stats = GetComponent<PlayerStats>();
        Debug.Log($"PlayerSkillController Loaded.");
    }

    public void ActivateSkill(string aminoAcid)
    {
        if (SkillDatabase.Instance == null)
        {
            Debug.LogError("SkillDatabase Instance is null!");
            return;
        }

        SkillStrategy skill = SkillDatabase.Instance.GetSkill(aminoAcid);
        if (skill != null)
        {
            Debug.Log($"Activating Skill: {skill.skillName}");

            // Get Color from AminoAcidDefinitions (Code-based Config)
            Color c = AminoAcidDefinitions.GetData(aminoAcid).Color;

            skill.Activate(this, c);
        }
        else
        {
            Debug.LogWarning($"No skill found for: {aminoAcid}");
        }
    }

    public void SetActivePet(GameObject newPet)
    {
        if (ActivePet != null)
            Destroy(ActivePet);
        ActivePet = newPet;
    }

    // Used by RibosomeUI
    public Color GetSkillColor(string aminoAcid)
    {
        return AminoAcidDefinitions.GetData(aminoAcid).Color;
    }
}
