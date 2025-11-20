using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("Skill Prefabs")]
    public GameObject toxicCloudPrefab; // Assign a prefab with ToxicCloud script
    // Effect prefabs for visuals could be added here

    private PlayerController player;
    private PlayerStats playerStats;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
    }

    public void ActivateSkill(string aminoAcid)
    {
        Debug.Log($"Activating Skill for: {aminoAcid}");

        switch (aminoAcid)
        {
            case "Phe": // UUU
                ActivateToxicCloud();
                break;
            case "Gly": // GGG
                ActivateShieldSpeed();
                break;
            case "Met": // AUG
                ActivateNuke();
                break;
            default:
                Debug.Log("No special skill for this amino acid.");
                break;
        }
    }

    private void ActivateToxicCloud()
    {
        if (player == null) return;

        // Spawn Cloud
        if (toxicCloudPrefab != null)
        {
            Instantiate(toxicCloudPrefab, player.transform.position, Quaternion.identity);
        }
        else
        {
            // Fallback generation
            GameObject go = new GameObject("ToxicCloud_Generated");
            go.transform.position = player.transform.position;
            var sprite = go.AddComponent<SpriteRenderer>();
            // Should load a sprite, but we'll just make it a color square for now if no sprite
            sprite.color = new Color(0.5f, 0f, 0.5f, 0.5f); // Purple
            go.AddComponent<ToxicCloud>();
        }
    }

    private void ActivateShieldSpeed()
    {
        if (playerStats == null) return;

        // Speed boost + Shield
        playerStats.EnableShield(10f); // 10 seconds shield or until hit
        playerStats.SetSpeedMultiplier(2.0f, 5f); // 2x speed for 5 seconds
        Debug.Log("Shield + Speed Activated!");
    }

    private void ActivateNuke()
    {
        // Damage all enemies
        var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            enemy.TakeDamage(999f);
        }
        // Reset cooldowns? (Not implemented yet)
        Debug.Log("Nuke Activated! Cleared Screen.");
    }
}

