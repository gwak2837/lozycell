using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [System.Serializable]
    public struct SkillEntry
    {
        public string aminoAcidCode;
        public SkillStrategy skillData;
    }

    [Header("Skill Database")]
    public List<SkillEntry> skillEntries;
    private Dictionary<string, SkillStrategy> skillMap;

    // State Management
    public GameObject ActivePet { get; private set; }

    public PlayerController Player { get; private set; }
    public PlayerStats Stats { get; private set; }

    private void Awake()
    {
        Player = GetComponent<PlayerController>();
        Stats = GetComponent<PlayerStats>();

        // Build Dictionary
        skillMap = new Dictionary<string, SkillStrategy>();
        foreach (var entry in skillEntries)
        {
            if (!skillMap.ContainsKey(entry.aminoAcidCode) && entry.skillData != null)
            {
                skillMap.Add(entry.aminoAcidCode, entry.skillData);
            }
        }
        Debug.Log($"PlayerSkillController Loaded. Skills: {skillMap.Count}");
    }

    public void ActivateSkill(string aminoAcid)
    {
        if (skillMap.TryGetValue(aminoAcid, out SkillStrategy skill))
        {
            Debug.Log($"Activating Skill: {skill.skillName}");
            
            // Get Color
            Color c = CodonTable.GetAminoAcidData(aminoAcid).Color;
            
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
}
