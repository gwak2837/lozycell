using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    [Header("Skill Prefabs")]
    public GameObject toxicCloudPrefab; 
    public GameObject projectilePrefab;
    
    // Pet for Met (Start Codon)
    private GameObject activePet = null;
    
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
            // Group A: Non-polar/Physical
            case "Gly": // Minigun
                StartCoroutine(ActivateMinigun());
                break;
            case "Ala": // Standard Shot
                ActivateStandardShot();
                break;
            case "Val": // Muscle Up - Power Shot
            case "Leu": 
            case "Ile":
                ActivateMuscleUp();
                break;
            case "Pro": // Boomerang
                ActivateBoomerang();
                break;

            // Group B: Polar/Water
            case "Ser": // Slow Field
                ActivateSlowField();
                break;
            case "Thr": // Freeze
                ActivateFreeze();
                break;
            case "Asn": // Wave
                ActivateWave();
                break;
            case "Gln": // Tidal
                ActivateTidal();
                break;

            // Group C: Basic/Lightning
            case "Lys": // Chain Lightning
                ActivateLightning();
                break;
            case "Arg": // Thunder Smash
                ActivateThunderSmash();
                break;
            case "His": // Overcharge
                ActivateOvercharge();
                break;

            // Group D: Acidic
            case "Asp": // Poison Pool
                ActivatePoisonPool();
                break;
            case "Glu": // Explosion
                ActivateExplosion();
                break;

            // Group E: Special
            case "Phe": // Homing Missiles
                ActivateHomingMissiles();
                break;
            case "Tyr": // Critical
                ActivateCritical();
                break;
            case "Trp": // Meteor
                ActivateMeteor();
                break;
            case "Cys": // Laser Link
                ActivateLaserLink();
                break;

            // Start Codon
            case "Met":
                ActivateShieldAndPet();
                break;

            // Stop Codon
            case "Stop":
                ActivateSelfDestruct();
                break;

            default:
                Debug.Log($"No specific skill for {aminoAcid}");
                break;
        }
    }

    // ========== GROUP A: NON-POLAR (PHYSICAL) ==========
    
    // Gly: Minigun
    private IEnumerator ActivateMinigun()
    {
        if (player == null) yield break;
        Debug.Log("Skill: Minigun Activated");
        
        for (int i = 0; i < 10; i++)
        {
            Vector3 targetDir = GetClosestEnemyDir();
            float spreadAngle = Random.Range(-15f, 15f);
            targetDir = Quaternion.Euler(0, 0, spreadAngle) * targetDir;
            
            SpawnProjectile(player.transform.position, targetDir, 10f, 15f, 2f, Color.gray, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Ala: Standard Shot
    private void ActivateStandardShot()
    {
        if (player == null) return;
        Debug.Log("Skill: Standard Shot Activated");
        
        Vector3 targetDir = GetClosestEnemyDir();
        SpawnProjectile(player.transform.position, targetDir, 20f, 12f, 3f, Color.gray, 0.5f);
    }

    // Val/Leu/Ile: Muscle Up (BCAA)
    private void ActivateMuscleUp()
    {
        if (player == null) return;
        Debug.Log("Skill: Muscle Up Activated");
        
        // Large knockback projectile
        Vector3 targetDir = GetClosestEnemyDir();
        // damage 40, speed 8, knockback 10
        SpawnProjectile(player.transform.position, targetDir, 40f, 8f, 2f, new Color(0.8f, 0.5f, 0.2f), 1.2f, 10f);
    }

    // Pro: Boomerang
    private void ActivateBoomerang()
    {
        if (player == null) return;
        Debug.Log("Skill: Boomerang Activated");
        
        StartCoroutine(BoomerangCoroutine());
    }

    private IEnumerator BoomerangCoroutine()
    {
        Vector3 startPos = player.transform.position;
        Vector3 targetDir = GetClosestEnemyDir();
        
        GameObject proj = SpawnProjectile(startPos, targetDir, 15f, 10f, 10f, Color.gray, 0.7f);
        if (proj == null) yield break;
        
        // Override movement to return
        float outTime = 1f;
        float returnTime = 1f;
        float elapsed = 0;
        
        // Going out
        while (elapsed < outTime && proj != null)
        {
            elapsed += Time.deltaTime;
            proj.transform.position += targetDir * 10f * Time.deltaTime;
            yield return null;
        }
        
        // Coming back
        elapsed = 0;
        while (elapsed < returnTime && proj != null && player != null)
        {
            elapsed += Time.deltaTime;
            Vector3 returnDir = (player.transform.position - proj.transform.position).normalized;
            proj.transform.position += returnDir * 15f * Time.deltaTime;
            yield return null;
        }
        
        if (proj != null) Destroy(proj);
    }

    // ========== GROUP B: POLAR (WATER/ICE) ==========
    
    // Ser: Slow Field
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
                enemy.ApplySlow(0.5f, 4f);
            }
        }
        
        // Visual: Cyan circle effect
        CreateVisualRing(player.transform.position, radius, Color.cyan);
    }

    // Thr: Freeze
    private void ActivateFreeze()
    {
        if (player == null) return;
        Debug.Log("Skill: Freeze Activated");
        
        // Freeze closest enemy completely
        EnemyController closest = GetClosestEnemy();
        if (closest != null)
        {
            closest.ApplySlow(0f, 3f); // 0% speed = frozen
            // Visual: Ice effect on enemy
            var sr = closest.GetComponent<SpriteRenderer>();
            if (sr != null) 
            {
                Color original = sr.color;
                sr.color = new Color(0.5f, 0.8f, 1f); // Ice blue
                StartCoroutine(RestoreColorAfter(sr, original, 3f));
            }
        }
    }

    // Asn: Wave
    private void ActivateWave()
    {
        if (player == null) return;
        Debug.Log("Skill: Wave Activated");
        
        Vector3 baseDir = GetClosestEnemyDir();
        
        // Fan-shaped projectiles
        for (int i = -2; i <= 2; i++)
        {
            float angle = i * 15f;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * baseDir;
            SpawnProjectile(player.transform.position, dir, 15f, 8f, 2f, new Color(0.2f, 0.6f, 1f), 0.6f);
        }
    }

    // Gln: Tidal
    private void ActivateTidal()
    {
        if (player == null) return;
        Debug.Log("Skill: Tidal Activated");
        
        // Large wave that pushes enemies back
        float radius = 8f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, radius);
        
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Push back
                Vector3 pushDir = (enemy.transform.position - player.transform.position).normalized;
                enemy.transform.position += pushDir * 3f;
                enemy.TakeDamage(20f);
            }
        }
        
        CreateVisualRing(player.transform.position, radius, new Color(0.2f, 0.6f, 1f));
    }

    // ========== GROUP C: BASIC (LIGHTNING) ==========
    
    // Lys: Chain Lightning
    private void ActivateLightning()
    {
        if (player == null) return;
        Debug.Log("Skill: Chain Lightning Activated");

        List<EnemyController> enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
        enemies.Sort((a, b) => Vector3.Distance(player.transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(player.transform.position, b.transform.position)));

        int chains = Mathf.Min(5, enemies.Count);
        for (int i = 0; i < chains; i++)
        {
            enemies[i].TakeDamage(25f);
            
            // Visual: Lightning line
            if (i > 0)
            {
                CreateLightningLine(enemies[i-1].transform.position, enemies[i].transform.position);
            }
            else
            {
                CreateLightningLine(player.transform.position, enemies[i].transform.position);
            }
        }
    }

    // Arg: Thunder Smash
    private void ActivateThunderSmash()
    {
        if (player == null) return;
        Debug.Log("Skill: Thunder Smash Activated");
        
        EnemyController closest = GetClosestEnemy();
        if (closest != null)
        {
            // Massive damage + stun
            closest.TakeDamage(50f);
            closest.ApplySlow(0f, 1f); // Stun for 1 second
            
            // Visual: Big lightning strike
            CreateLightningStrike(closest.transform.position);
        }
    }

    // His: Overcharge
    private void ActivateOvercharge()
    {
        if (player == null) return;
        Debug.Log("Skill: Overcharge Activated");
        
        // Speed boost for player
        if (playerStats != null)
        {
            playerStats.SetSpeedMultiplier(2f, 5f); // 2x speed for 5 seconds
        }
        
        // Visual: Electric aura around player
        CreateElectricAura(player.transform);
    }

    // ========== GROUP D: ACIDIC ==========
    
    // Asp: Poison Pool
    private void ActivatePoisonPool()
    {
        if (player == null) return;
        Debug.Log("Skill: Poison Pool Activated");

        float damage = 15f; // DPS increased for effectiveness
        float duration = 5f;
        float radius = 3f;
        
        GameObject cloudObj = null;
        
        if (toxicCloudPrefab != null)
        {
            cloudObj = Instantiate(toxicCloudPrefab, player.transform.position, Quaternion.identity);
        }
        else
        {
            cloudObj = new GameObject("PoisonPool");
            cloudObj.transform.position = player.transform.position;
            var sprite = cloudObj.AddComponent<SpriteRenderer>();
            sprite.color = new Color(0.3f, 1f, 0.3f, 0.5f);
            // Use a simple circle sprite if available, or default square
            
            // Create circle texture for visual
            Texture2D tex = new Texture2D(64, 64);
            Color[] colors = new Color[64 * 64];
            Vector2 center = new Vector2(32, 32);
            for (int x = 0; x < 64; x++) {
                for (int y = 0; y < 64; y++) {
                    float d = Vector2.Distance(new Vector2(x,y), center);
                    colors[y*64+x] = (d < 30) ? new Color(1,1,1,0.8f) : Color.clear;
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            sprite.sprite = Sprite.Create(tex, new Rect(0,0,64,64), new Vector2(0.5f,0.5f), 32);

            cloudObj.AddComponent<ToxicCloud>();
        }

        if (cloudObj != null)
        {
            var toxicCloud = cloudObj.GetComponent<ToxicCloud>();
            if (toxicCloud != null) 
            {
                toxicCloud.Initialize(damage, duration, radius);
            }
            
            var sr = cloudObj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(0.2f, 0.9f, 0.2f, 0.6f); 
        }
    }

    // Glu: Explosion
    private void ActivateExplosion()
    {
        if (player == null) return;
        Debug.Log("Skill: Explosion Activated");

        float radius = 5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, radius);
        
        // Visual
        GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        explosion.transform.position = player.transform.position;
        explosion.transform.localScale = Vector3.one * radius * 2;
        Destroy(explosion.GetComponent<Collider>());
        var renderer = explosion.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        renderer.material = mat;
        Destroy(explosion, 0.2f);

        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(50f);
            }
        }
    }

    // ========== GROUP E: SPECIAL ==========
    
    // Phe: Homing Missiles
    private void ActivateHomingMissiles()
    {
        if (player == null) return;
        Debug.Log("Skill: Homing Missiles Activated");
        
        for (int i = 0; i < 3; i++)
        {
            float angle = i * 120f;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;
            EnemyController target = GetClosestEnemy();
            
            SpawnProjectile(player.transform.position, dir, 15f, 8f, 4f, Color.magenta, 0.5f, 0f, target?.transform);
        }
    }

    // Tyr: Critical Hit
    private void ActivateCritical()
    {
        if (player == null) return;
        Debug.Log("Skill: Critical Hit Activated");
        
        // Single powerful shot with high crit chance
        Vector3 targetDir = GetClosestEnemyDir();
        float critDamage = Random.Range(0f, 1f) < 0.5f ? 100f : 30f; // 50% chance for massive damage
        
        GameObject proj = SpawnProjectile(player.transform.position, targetDir, critDamage, 20f, 3f, 
            new Color(0.8f, 0f, 0.8f), critDamage > 50f ? 1f : 0.6f);
        
        if (critDamage > 50f)
        {
            Debug.Log("CRITICAL HIT!");
        }
    }

    // Trp: Meteor
    private void ActivateMeteor()
    {
        if (player == null) return;
        Debug.Log("Skill: Meteor Activated");

        // Multiple meteors fall from above
        StartCoroutine(MeteorShower());
    }

    private IEnumerator MeteorShower()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = player.transform.position + new Vector3(Random.Range(-3f, 3f), 5f, 0);
            GameObject meteor = SpawnProjectile(spawnPos, Vector3.down, 60f, 8f, 3f, 
                new Color(0.6f, 0f, 0.8f), 1.5f);
            
            yield return new WaitForSeconds(0.2f);
        }
    }

    // Cys: Laser Link
    private void ActivateLaserLink()
    {
        if (player == null) return;
        Debug.Log("Skill: Laser Link Activated");
        
        // Link two closest enemies with damaging laser
        var enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
        enemies.Sort((a, b) => Vector3.Distance(player.transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(player.transform.position, b.transform.position)));
        
        if (enemies.Count >= 2)
        {
            StartCoroutine(LaserLinkCoroutine(enemies[0], enemies[1]));
        }
        else if (enemies.Count == 1)
        {
            // Link player to enemy
            StartCoroutine(LaserLinkPlayerCoroutine(enemies[0]));
        }
    }

    private IEnumerator LaserLinkCoroutine(EnemyController e1, EnemyController e2)
    {
        float duration = 2f;
        float elapsed = 0;
        float tickRate = 0.2f;
        float lastTick = 0;
        
        GameObject laserLine = CreateLaserLine(e1.transform.position, e2.transform.position);
        LineRenderer lr = laserLine.GetComponent<LineRenderer>();
        
        while (elapsed < duration && e1 != null && e2 != null)
        {
            elapsed += Time.deltaTime;
            
            // Update line position
            lr.SetPosition(0, e1.transform.position);
            lr.SetPosition(1, e2.transform.position);
            
            // Damage tick
            if (elapsed - lastTick > tickRate)
            {
                lastTick = elapsed;
                e1.TakeDamage(10f);
                e2.TakeDamage(10f);
            }
            
            yield return null;
        }
        
        Destroy(laserLine);
    }

    private IEnumerator LaserLinkPlayerCoroutine(EnemyController enemy)
    {
        float duration = 2f;
        float elapsed = 0;
        float tickRate = 0.2f;
        float lastTick = 0;
        
        GameObject laserLine = CreateLaserLine(player.transform.position, enemy.transform.position);
        LineRenderer lr = laserLine.GetComponent<LineRenderer>();
        
        while (elapsed < duration && enemy != null && player != null)
        {
            elapsed += Time.deltaTime;
            
            lr.SetPosition(0, player.transform.position);
            lr.SetPosition(1, enemy.transform.position);
            
            if (elapsed - lastTick > tickRate)
            {
                lastTick = elapsed;
                enemy.TakeDamage(15f);
            }
            
            yield return null;
        }
        
        Destroy(laserLine);
    }

    // ========== START/STOP CODONS ==========
    
    // Met: Shield + Pet
    private void ActivateShieldAndPet()
    {
        if (playerStats == null) return;
        Debug.Log("Skill: Start (Shield + Pet) Activated");

        playerStats.EnableShield(5f);
        
        // Summon pet
        if (activePet != null) Destroy(activePet);
        
        activePet = new GameObject("Pet_Ribosome");
        activePet.transform.position = player.transform.position + Vector3.right;
        
        // Visual
        var sr = activePet.AddComponent<SpriteRenderer>();
        sr.color = Color.green;
        sr.sortingOrder = 10;
        
        // Make it a simple square for now
        Texture2D tex = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        for (int i = 0; i < colors.Length; i++) colors[i] = Color.white;
        tex.SetPixels(colors);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        
        activePet.transform.localScale = Vector3.one * 0.5f;
        
        // Pet behavior
        StartCoroutine(PetBehavior());
    }

    private IEnumerator PetBehavior()
    {
        float petDuration = 10f;
        float elapsed = 0;
        float shootInterval = 1f;
        float lastShot = 0;
        
        while (elapsed < petDuration && activePet != null && player != null)
        {
            elapsed += Time.deltaTime;
            
            // Follow player
            Vector3 targetPos = player.transform.position + new Vector3(1.5f, 0, 0);
            activePet.transform.position = Vector3.Lerp(activePet.transform.position, targetPos, Time.deltaTime * 5f);
            
            // Shoot at enemies
            if (elapsed - lastShot > shootInterval)
            {
                lastShot = elapsed;
                Vector3 enemyDir = GetClosestEnemyDir();
                SpawnProjectile(activePet.transform.position, enemyDir, 10f, 12f, 2f, Color.green, 0.3f);
            }
            
            yield return null;
        }
        
        if (activePet != null) Destroy(activePet);
        activePet = null;
    }

    // Stop: Renewal & Regeneration
    private void ActivateSelfDestruct()
    {
        if (player == null) return;
        Debug.Log("Skill: STOP - Renewal Activated");
        
        // Heal player (30% of max health)
        if (playerStats != null)
        {
            float healAmount = playerStats.maxHealth * 0.3f;
            playerStats.Heal(healAmount);
        }
        
        // Small AOE damage to nearby enemies (not massive, just clearing)
        float radius = 5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, radius);
        
        // Visual: Red pulse that transitions to green (death → renewal)
        GameObject pulse = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pulse.transform.position = player.transform.position;
        pulse.transform.localScale = Vector3.one * radius * 2;
        Destroy(pulse.GetComponent<Collider>());
        var renderer = pulse.GetComponent<Renderer>();
        
        // Color transition: Red (stop) → Green (renewal)
        StartCoroutine(ColorTransition(renderer, Color.red, Color.green, 0.5f));
        Destroy(pulse, 0.5f);
        
        // Moderate damage to nearby enemies
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(30f); // Moderate damage
            }
        }
        
        // Brief invulnerability after "rebirth"
        if (playerStats != null)
        {
            playerStats.EnableInvulnerability(1.5f); // 1.5 seconds of invulnerability
        }
    }
    
    private IEnumerator ColorTransition(Renderer rend, Color from, Color to, float duration)
    {
        if (rend == null) yield break;
        
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = from;
        rend.material = mat;
        
        float elapsed = 0;
        while (elapsed < duration && rend != null && mat != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            mat.color = Color.Lerp(from, to, t);
            yield return null;
        }
        
        if (mat != null) Destroy(mat);
    }

    // ========== HELPER METHODS ==========
    
    private GameObject SpawnProjectile(Vector3 pos, Vector3 dir, float damage, float speed, float lifetime, Color color, float scale, float knockback = 0f, Transform homingTarget = null)
    {
        GameObject proj = null;
        if (projectilePrefab != null)
        {
            proj = Instantiate(projectilePrefab, pos, Quaternion.identity);
        }
        else
        {
            proj = new GameObject("Projectile");
            proj.transform.position = pos;
            var sr = proj.AddComponent<SpriteRenderer>();
            
            Texture2D tex = new Texture2D(16, 16);
            Color[] colors = new Color[16 * 16];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.white;
            tex.SetPixels(colors);
            tex.Apply();
            
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            sr.color = color;
            
            var col = proj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            
            var rb = proj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.isKinematic = true;

            proj.AddComponent<ProjectileController>();
        }

        if (proj != null)
        {
            var sr = proj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
            
            proj.transform.localScale = Vector3.one * scale;

            var pc = proj.GetComponent<ProjectileController>();
            if (pc == null) pc = proj.AddComponent<ProjectileController>();
            
            pc.Initialize(dir, damage, speed, lifetime, knockback, homingTarget);
        }
        
        return proj;
    }

    private void CreateVisualRing(Vector3 position, float radius, Color color)
    {
        GameObject ring = new GameObject("VisualRing");
        ring.transform.position = position;
        
        var sr = ring.AddComponent<SpriteRenderer>();
        sr.color = new Color(color.r, color.g, color.b, 0.3f);
        
        // Create ring texture (simple)
        Texture2D tex = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        Vector2 center = new Vector2(32, 32);
        
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist < 32 && dist > 28)
                    pixels[y * 64 + x] = Color.white;
                else
                    pixels[y * 64 + x] = Color.clear;
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 32);
        
        ring.transform.localScale = Vector3.one * (radius / 2f);
        Destroy(ring, 0.5f);
    }

    private void CreateLightningLine(Vector3 from, Vector3 to)
    {
        GameObject line = new GameObject("LightningLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.yellow;
        lr.endColor = Color.yellow;
        
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        
        Destroy(line, 0.2f);
    }

    private void CreateLightningStrike(Vector3 position)
    {
        GameObject strike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        strike.transform.position = position;
        strike.transform.localScale = new Vector3(0.5f, 5f, 0.5f);
        Destroy(strike.GetComponent<Collider>());
        
        var renderer = strike.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.yellow;
        renderer.material = mat;
        
        Destroy(strike, 0.3f);
    }

    private void CreateElectricAura(Transform target)
    {
        GameObject aura = new GameObject("ElectricAura");
        aura.transform.SetParent(target);
        aura.transform.localPosition = Vector3.zero;
        
        var sr = aura.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 1f, 0f, 0.3f);
        
        // Simple circle
        Texture2D tex = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        Vector2 center = new Vector2(32, 32);
        
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist < 30)
                    pixels[y * 64 + x] = Color.white;
                else
                    pixels[y * 64 + x] = Color.clear;
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 32);
        
        aura.transform.localScale = Vector3.one * 2f;
        Destroy(aura, 5f); // Match overcharge duration
    }

    private GameObject CreateLaserLine(Vector3 from, Vector3 to)
    {
        GameObject line = new GameObject("LaserLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0.8f, 0.8f, 0f);
        lr.endColor = new Color(0.8f, 0.8f, 0f);
        
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        
        return line;
    }

    private IEnumerator RestoreColorAfter(SpriteRenderer sr, Color original, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sr != null) sr.color = original;
    }

    private Vector3 GetClosestEnemyDir()
    {
        EnemyController closest = GetClosestEnemy();
        if (closest != null)
        {
            return (closest.transform.position - player.transform.position).normalized;
        }
        return player.transform.up;
    }
    
    private EnemyController GetClosestEnemy()
    {
        var enemies = FindObjectsOfType<EnemyController>();
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