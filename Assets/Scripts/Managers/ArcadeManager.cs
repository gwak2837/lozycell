using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ArcadeManager : MonoBehaviour
{
    [Header("Settings")]
    public int targetAminoAcids = 100;
    public GameObject aminoAcidPrefab;
    public Transform spawnArea;
    public float spawnRadius = 10f;
    public float spawnInterval = 0.5f;

    [Header("UI")]
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI comboPopupText; // New: Flashy text
    public GameObject winPanel;
    public GameObject losePanel;

    public delegate void CodonUpdateHandler(List<BaseType> currentCodon);
    public event CodonUpdateHandler OnCodonUpdated;

    [Header("Managers")]
    public PlayerSkillController skillController; // Changed from SkillManager
    public EnemySpawner enemySpawner;
    public CodonRingController codonRing;

    private int currentSessionAminoAcids = 0;
    private float spawnTimer;
    private bool isGameActive = true;
    private bool isProcessingSequence = false;

    private Transform playerTransform;
    private PlayerStats playerStats;

    // Track current sequence of bases (max 3)
    private List<BaseType> currentCodon = new List<BaseType>();

    // Object Pooling
    private Queue<GeneticBase> pool = new Queue<GeneticBase>();

    public static ArcadeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public List<BaseType> GetCurrentCodon()
    {
        return new List<BaseType>(currentCodon);
    }

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
        // Check for Ribosome UI, create if missing
        if (FindFirstObjectByType<RibosomeUI>() == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject cObj = new GameObject("Canvas");
                canvas = cObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                cObj.AddComponent<CanvasScaler>();
                cObj.AddComponent<GraphicRaycaster>();
            }
            RibosomeUI.CreateDefaultUI(canvas.transform);
        }

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
            playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.OnDeath += HandlePlayerDeath;
                playerStats.OnHealthChanged += HandleHealthChanged;
            }

            // Try to find components if not assigned
            if (skillController == null) skillController = FindFirstObjectByType<PlayerSkillController>();
            if (enemySpawner == null) enemySpawner = FindFirstObjectByType<EnemySpawner>();
            if (codonRing == null) codonRing = player.GetComponentInChildren<CodonRingController>();

            if (codonRing == null)
            {
                GameObject ringObj = new GameObject("CodonRing");
                ringObj.transform.SetParent(playerTransform);
                ringObj.transform.localPosition = Vector3.zero;
                codonRing = ringObj.AddComponent<CodonRingController>();
            }

            // Ensure PlayerSkillController exists
            if (skillController == null)
            {
                // Usually on player
                skillController = player.GetComponent<PlayerSkillController>();
                if (skillController == null)
                    skillController = player.gameObject.AddComponent<PlayerSkillController>();
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
        if (!isGameActive || isProcessingSequence) return;

        currentCodon.Add(type);
        UpdateCodonRing();
        OnCodonUpdated?.Invoke(new List<BaseType>(currentCodon));

        if (currentCodon.Count >= 3)
        {
            StartCoroutine(ProcessCompleteCodon());
        }
        else
        {
            UpdateUI();
        }
    }

    private IEnumerator ProcessCompleteCodon()
    {
        isProcessingSequence = true;

        yield return new WaitForSeconds(0.5f);

        string aminoAcidName = CodonTable.GetAminoAcid(currentCodon[0], currentCodon[1], currentCodon[2]);
        Debug.Log($"formed {aminoAcidName} from {currentCodon[0]}{currentCodon[1]}{currentCodon[2]}");

        ShowComboVisuals(aminoAcidName);

        // Trigger Skill
        if (skillController != null)
        {
            Debug.Log($"ArcadeManager invoking skill: {aminoAcidName}");
            skillController.ActivateSkill(aminoAcidName);
        }
        else
        {
            Debug.LogError("SkillController is NULL! Cannot activate skill.");
            // Retry finding it
            skillController = FindFirstObjectByType<PlayerSkillController>();
            if (skillController != null) skillController.ActivateSkill(aminoAcidName);
        }

        currentSessionAminoAcids++;

        currentCodon.Clear();
        UpdateCodonRing();
        OnCodonUpdated?.Invoke(new List<BaseType>(currentCodon));

        if (currentSessionAminoAcids >= targetAminoAcids)
        {
            EndGame(true);
        }

        UpdateUI();
        isProcessingSequence = false;
    }

    private void ShowComboVisuals(string shortName)
    {
        if (comboPopupText == null)
        {
            if (progressText != null && progressText.transform.parent != null)
            {
                GameObject go = new GameObject("ComboPopupText");
                go.transform.SetParent(progressText.transform.parent, false);
                comboPopupText = go.AddComponent<TextMeshProUGUI>();
                comboPopupText.alignment = TextAlignmentOptions.Center;
                comboPopupText.fontSize = 50;
                comboPopupText.fontStyle = FontStyles.Bold;
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(800, 200);
            }
        }

        if (comboPopupText != null)
        {
            AminoAcidData data = CodonTable.GetData(shortName);
            comboPopupText.text = $"{data.FullName}\n<size=80%>{data.SkillDescription}</size>";
            comboPopupText.color = data.Color;
            StartCoroutine(AnimatePopup(comboPopupText));
        }
    }

    private IEnumerator AnimatePopup(TextMeshProUGUI textComp)
    {
        textComp.gameObject.SetActive(true);
        textComp.transform.localScale = Vector3.zero;

        float duration = 0.3f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.2f;
            textComp.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        textComp.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.8f);

        time = 0;
        duration = 0.5f;
        Vector3 startPos = textComp.transform.localPosition;
        Color startColor = textComp.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            textComp.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            textComp.transform.localPosition = startPos + Vector3.up * (100f * t);
            yield return null;
        }

        textComp.gameObject.SetActive(false);
        textComp.transform.localPosition = Vector3.zero;
    }

    private void UpdateCodonRing()
    {
        if (codonRing != null)
        {
            codonRing.UpdateVisuals(currentCodon);
        }
    }

    public void SpawnDrop(Vector3 position)
    {
        if (aminoAcidPrefab == null) return;

        GeneticBase geneticBase = null;
        if (pool.Count > 0)
        {
            geneticBase = pool.Dequeue();
            geneticBase.transform.position = position;
            geneticBase.transform.rotation = Quaternion.identity;
            geneticBase.gameObject.SetActive(true);
        }
        else
        {
            GameObject obj = Instantiate(aminoAcidPrefab, position, Quaternion.identity);
            geneticBase = obj.GetComponent<GeneticBase>();
        }

        if (geneticBase != null)
        {
            BaseType randomType = (BaseType)Random.Range(0, 4);
            geneticBase.Initialize(randomType, this);
        }
    }

    public void CollectAminoAcid()
    {
        currentSessionAminoAcids++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (progressText != null)
        {
            string codonRequest = "";
            foreach (var b in currentCodon) codonRequest += b.ToString() + " ";

            string healthInfo = "";
            if (playerStats != null)
            {
                healthInfo = $"HP: {playerStats.currentHealth}/{playerStats.maxHealth}";
            }

            progressText.text = $"Amino Acids: {currentSessionAminoAcids} / {targetAminoAcids}\nSequence: {codonRequest}\n{healthInfo}";
        }
    }

    private void HandleHealthChanged(float ratio)
    {
        UpdateUI();
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
            playerStats.OnHealthChanged -= HandleHealthChanged;
        }
    }
}
