using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using TMPro;
using System.Collections.Generic;

public class ArcadeManager : MonoBehaviour
{
    [Header("Settings")]
    public int targetAminoAcids = 100;
    public GameObject aminoAcidPrefab; // Actually spawns GeneticBase now
    public Transform spawnArea;
    public float spawnRadius = 10f;
    public float spawnInterval = 0.5f;

    [Header("UI")]
    public TextMeshProUGUI progressText;
    public GameObject winPanel;
    public GameObject losePanel; // New: Game Over UI

    [Header("Managers")]
    public SkillManager skillManager; // New
    public EnemySpawner enemySpawner; // New
    public CodonRingController codonRing; // New: Optional, if pre-assigned

    private int currentSessionAminoAcids = 0;
    private float spawnTimer;
    private bool isGameActive = true;

    private Transform playerTransform;
    private PlayerStats playerStats;
    
    // Track current sequence of bases (max 3)
    private List<BaseType> currentCodon = new List<BaseType>();

    // Object Pooling
    private Queue<GeneticBase> pool = new Queue<GeneticBase>();

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public void ReturnToPool(GeneticBase item)
    {
        item.gameObject.SetActive(false);
        pool.Enqueue(item);
    }

    private void Start()
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
            playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.OnDeath += HandlePlayerDeath;
            }
            
            // Try to find components if not assigned
            if (skillManager == null) skillManager = FindFirstObjectByType<SkillManager>();
            if (enemySpawner == null) enemySpawner = FindFirstObjectByType<EnemySpawner>();
            if (codonRing == null) codonRing = player.GetComponentInChildren<CodonRingController>();
            
            // If CodonRing is not on player, maybe on manager or separate? 
            // Plan said "attached to player", so GetComponentInChildren is correct if I add it there.
            // If not found, we might need to spawn it? For now assume user/prefab setup or I'll ensure it exists.
            if (codonRing == null)
            {
                // Auto-add if missing for convenience in this task
                GameObject ringObj = new GameObject("CodonRing");
                ringObj.transform.SetParent(playerTransform);
                ringObj.transform.localPosition = Vector3.zero;
                codonRing = ringObj.AddComponent<CodonRingController>();
            }
            
            // Ensure SkillManager exists
            if (skillManager == null)
            {
                GameObject smObj = new GameObject("SkillManager");
                skillManager = smObj.AddComponent<SkillManager>();
            }
            
            // Ensure EnemySpawner exists
            if (enemySpawner == null)
            {
                GameObject esObj = new GameObject("EnemySpawner");
                enemySpawner = esObj.AddComponent<EnemySpawner>();
            }
        }

        UpdateUI();
        UpdateCodonRing();
    }

    private void Update()
    {
        if (!isGameActive) return;

        if (playerTransform == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) 
            {
                playerTransform = player.transform;
                // Re-init if player was just found (e.g. respawned - though not supported yet)
            }
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnGeneticBase();
            spawnTimer = 0f;
        }
    }

    private void SpawnGeneticBase()
    {
        if (aminoAcidPrefab == null) return;

        Vector3 center = Vector3.zero;
        if (playerTransform != null)
        {
            center = playerTransform.position;
        }
        else if (spawnArea != null)
        {
            center = spawnArea.position;
        }

        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = center + new Vector3(randomPos.x, randomPos.y, 0);
        
        GeneticBase geneticBase = null;
        if (pool.Count > 0)
        {
            geneticBase = pool.Dequeue();
            geneticBase.transform.position = spawnPos;
            geneticBase.transform.rotation = Quaternion.identity;
            geneticBase.gameObject.SetActive(true);
        }
        else
        {
            GameObject obj = Instantiate(aminoAcidPrefab, spawnPos, Quaternion.identity);
            geneticBase = obj.GetComponent<GeneticBase>();
        }
        
        if (geneticBase != null)
        {
            // Assign random Base Type
            BaseType randomType = (BaseType)Random.Range(0, 4);
            geneticBase.Initialize(randomType, this);
        }
    }

    public void CollectBase(BaseType type)
    {
        if (!isGameActive) return;

        currentCodon.Add(type);
        UpdateCodonRing();

        if (currentCodon.Count >= 3)
        {
            // Form Amino Acid
            string aminoAcidName = CodonTable.GetAminoAcid(currentCodon[0], currentCodon[1], currentCodon[2]);
            Debug.Log($"formed {aminoAcidName} from {currentCodon[0]}{currentCodon[1]}{currentCodon[2]}");
            
            // Trigger Skill
            if (skillManager != null)
            {
                skillManager.ActivateSkill(aminoAcidName);
            }
            
            currentSessionAminoAcids++;
            currentCodon.Clear();
            UpdateCodonRing();
            
            if (currentSessionAminoAcids >= targetAminoAcids)
            {
                EndGame(true);
            }
        }

        UpdateUI();
    }

    private void UpdateCodonRing()
    {
        if (codonRing != null)
        {
            codonRing.UpdateVisuals(currentCodon);
        }
    }

    public void CollectAminoAcid()
    {
        // Legacy
        currentSessionAminoAcids++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (progressText != null)
        {
            string codonRequest = "";
            foreach(var b in currentCodon) codonRequest += b.ToString() + " ";
            
            // Maybe show Health too if possible?
            string healthInfo = "";
            if (playerStats != null)
            {
                healthInfo = $"HP: {playerStats.currentHealth}/{playerStats.maxHealth}";
            }

            progressText.text = $"Amino Acids: {currentSessionAminoAcids} / {targetAminoAcids}\nSequence: {codonRequest}\n{healthInfo}";
        }
    }

    private void HandlePlayerDeath()
    {
        EndGame(false);
    }

    private void EndGame(bool win)
    {
        isGameActive = false;
        
        if (enemySpawner != null) enemySpawner.StopSpawning();

        if (win)
        {
            Debug.Log("Arcade Mode Cleared!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddAminoAcids(currentSessionAminoAcids);
            }
            if (winPanel != null) winPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Game Over!");
            if (losePanel != null) losePanel.SetActive(true);
        }
        
        Invoke(nameof(ReturnToBase), 3f);
    }

    private void ReturnToBase()
    {
        SceneManager.LoadScene("BaseScene");
    }
    
    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnDeath -= HandlePlayerDeath;
        }
    }
}
