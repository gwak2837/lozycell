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

    private int currentSessionAminoAcids = 0;
    private float spawnTimer;
    private bool isGameActive = true;

    private Transform playerTransform;
    
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
        }

        UpdateUI();
    }

    private void Update()
    {
        if (!isGameActive) return;

        if (playerTransform == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) playerTransform = player.transform;
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

        if (currentCodon.Count >= 3)
        {
            // Form Amino Acid
            string aminoAcidName = CodonTable.GetAminoAcid(currentCodon[0], currentCodon[1], currentCodon[2]);
            Debug.Log($"formed {aminoAcidName} from {currentCodon[0]}{currentCodon[1]}{currentCodon[2]}");
            
            // Only count as success if not Stop? PRD doesn't specify lose condition on Stop, but usually Stop ends translation.
            // For this arcade mode, we'll just count it as 1 collected "Thing" or maybe handle Stop differently?
            // The user didn't specify Stop logic, just "make amino acids".
            // We will treat all as valid +1 for now.
            
            currentSessionAminoAcids++;
            currentCodon.Clear();
            
            if (currentSessionAminoAcids >= targetAminoAcids)
            {
                EndGame();
            }
        }

        UpdateUI();
    }

    // Kept for backward compatibility or reference, but not used by GeneticBase
    public void CollectAminoAcid()
    {
        // Legacy or generic collect
        currentSessionAminoAcids++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (progressText != null)
        {
            string codonRequest = "";
            foreach(var b in currentCodon) codonRequest += b.ToString() + " ";
            
            progressText.text = $"Amino Acids: {currentSessionAminoAcids} / {targetAminoAcids}\nSequence: {codonRequest}";
        }
    }

    private void EndGame()
    {
        isGameActive = false;
        Debug.Log("Arcade Mode Cleared!");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddAminoAcids(currentSessionAminoAcids);
        }

        if (winPanel != null) winPanel.SetActive(true);
        
        Invoke(nameof(ReturnToBase), 2f);
    }

    private void ReturnToBase()
    {
        SceneManager.LoadScene("BaseScene");
    }
}
