using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    [Header("Skill Prefabs")]
    public GameObject toxicCloudPrefab; 
    public GameObject projectilePrefab; 
    
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
            // Group A: Non-polar/Physical (Minigun)
            case "Gly": 
            case "Ala": 
            case "Val": 
            case "Leu": 
            case "Ile": 
            case "Pro":
                StartCoroutine(ActivateMinigun());
                break;

            // Group B: Polar/Water (Slow)
            case "Ser": 
            case "Thr": 
            case "Asn": 
            case "Gln":
                ActivateSlowField();
                break;

            // Group C: Basic/Lightning (Chain Lightning)
            case "Lys": 
            case "Arg": 
            case "His":
                ActivateLightning();
                break;

            // Group D: Acidic/Fire (Toxic Cloud)
            case "Asp": 
            case "Glu":
                ActivateToxicCloud();
                break;

            // Group E: Special (Meteor/Homing)
            case "Trp": 
            case "Phe": 
            case "Tyr": 
            case "Cys":
                if (aminoAcid == "Phe") 
                    ActivateHomingMissiles();
                else 
                    ActivateMeteor();
                break;

            // Start Codon
            case "Met":
                ActivateShieldAndPet();
                break;

            // Stop Codon
            case "Stop":
                ActivateExplosion();
                break;

            default:
                Debug.Log($"No specific skill for {aminoAcid}");
                break;
        }
    }

    // Group A: Minigun
    private IEnumerator ActivateMinigun()
    {
        if (player == null) yield break;

        Debug.Log("Skill: Minigun Activated");
        // 10 shots over 0.5 seconds
        for (int i = 0; i < 10; i++)
        {
            Vector3 targetDir = GetClosestEnemyDir();
            // Add some spread
            float spreadAngle = Random.Range(-15f, 15f);
            targetDir = Quaternion.Euler(0, 0, spreadAngle) * targetDir;
            
            SpawnProjectile(player.transform.position, targetDir, 10f, 15f, 2f, Color.gray, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Group B: Slow
    private void ActivateSlowField()
    {
        if (player == null) return;
        Debug.Log("Skill: Slow Field Activated");

        float radius = 6f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, radius);
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.ApplySlow(0.5f, 4f); // 50% speed for 4 sec
            }
        }
        
        // Visual: Blue Ring (Simulated by a short lived large projectile or effect)
        // For now just debug or simple effect if possible
    }

    // Group C: Lightning
    private void ActivateLightning()
    {
        if (player == null) return;
        Debug.Log("Skill: Lightning Activated");

        float radius = 7f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, radius);
        int count = 0;
        foreach (var hit in hits)
        {
            if (count >= 5) break; // Max 5 targets
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(25f);
                // Visual: Yellow line (omitted for now, maybe flash enemy color)
                count++;
            }
        }
    }

    // Group D: Toxic Cloud
    private void ActivateToxicCloud()
    {
        if (player == null) return;
        Debug.Log("Skill: Toxic Cloud Activated");

        float damage = 5f; // DPS
        
        GameObject cloudObj = null;
        if (toxicCloudPrefab != null)
        {
            cloudObj = Instantiate(toxicCloudPrefab, player.transform.position, Quaternion.identity);
        }
        else
        {
            cloudObj = new GameObject("ToxicCloud_Generated");
            cloudObj.transform.position = player.transform.position;
            var sprite = cloudObj.AddComponent<SpriteRenderer>();
            sprite.color = new Color(0.5f, 0f, 0.5f, 0.5f); // Purple
            cloudObj.AddComponent<ToxicCloud>();
        }

        if (cloudObj != null)
        {
            var toxicCloud = cloudObj.GetComponent<ToxicCloud>();
            if (toxicCloud != null)
            {
                toxicCloud.SetDamage(damage);
            }
        }
    }

    // Group E: Meteor
    private void ActivateMeteor()
    {
        if (player == null) return;
        Debug.Log("Skill: Meteor Activated");

        // Spawn large projectile that moves slowly or just hits
        // Let's make it a large projectile that passes through
        Vector3 dir = Random.insideUnitCircle.normalized;
        SpawnProjectile(player.transform.position, dir, 50f, 5f, 3f, new Color(0.6f, 0f, 0.8f), 1.5f);
    }
    
    // Group E: Homing (Phe)
    private void ActivateHomingMissiles()
    {
         if (player == null) return;
         Debug.Log("Skill: Homing Missiles Activated");
         
         // Spawn 3 missiles
         for (int i = 0; i < 3; i++)
         {
             float angle = i * 120f;
             Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;
             EnemyController target = GetClosestEnemy();
             
             SpawnProjectile(player.transform.position, dir, 15f, 8f, 4f, Color.magenta, 0.5f, target != null ? target.transform : null);
         }
    }

    // Start: Shield + Pet
    private void ActivateShieldAndPet()
    {
        if (playerStats == null) return;
        Debug.Log("Skill: Start (Shield) Activated");

        playerStats.EnableShield(5f);
        // Pet logic: maybe just a visual or permanent small shooter (omitted for simplicity in this pass)
    }

    // Stop: Explosion
    private void ActivateExplosion()
    {
        if (player == null) return;
        Debug.Log("Skill: Stop (Explosion) Activated");

        float radius = 5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, radius);
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(50f);
            }
        }
    }

    // Helpers
    private void SpawnProjectile(Vector3 pos, Vector3 dir, float damage, float speed, float lifetime, Color color, float scale, Transform homingTarget = null)
    {
        GameObject proj = null;
        if (projectilePrefab != null)
        {
            proj = Instantiate(projectilePrefab, pos, Quaternion.identity);
        }
        else
        {
            proj = new GameObject("Projectile_Generated");
            proj.transform.position = pos;
            var sr = proj.AddComponent<SpriteRenderer>();
            
            // Generate a simple white texture so it's visible
            Texture2D tex = new Texture2D(16, 16);
            Color[] colors = new Color[16 * 16];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.white;
            tex.SetPixels(colors);
            tex.Apply();
            
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            sr.color = color; // Apply requested color tint
            
            var col = proj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            
            // Add Rigidbody for reliable trigger detection if needed, though kinematic/trigger works
            var rb = proj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.isKinematic = true;

            proj.AddComponent<ProjectileController>();
            
            // Note: Generated texture might leak memory if not destroyed, 
            // but for a prototype/fallback it's acceptable.
        }

        if (proj != null)
        {
            // Set color if we have a SR
            var sr = proj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
            
            proj.transform.localScale = Vector3.one * scale;

            var pc = proj.GetComponent<ProjectileController>();
            if (pc == null) pc = proj.AddComponent<ProjectileController>();
            
            pc.Initialize(dir, damage, speed, lifetime, homingTarget);
        }
    }

    private Vector3 GetClosestEnemyDir()
    {
        EnemyController closest = GetClosestEnemy();
        if (closest != null)
        {
            return (closest.transform.position - player.transform.position).normalized;
        }
        return player.transform.up; // Default forward
    }
    
    private EnemyController GetClosestEnemy()
    {
        var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        EnemyController closest = null;
        float minDist = float.MaxValue;
        
        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
    }
}
