using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArcadeManager : MonoBehaviour
{
    [Header("Settings")]
    private int targetAminoAcids;

    [Header("UI")]
    [SerializeField]
    private RibosomeUI ribosomeUIPrefab;

    [SerializeField]
    private TextMeshProUGUI progressText;

    [SerializeField]
    private TextMeshProUGUI comboPopupText;

    [SerializeField]
    private GameObject winPanel;

    [SerializeField]
    private GameObject losePanel;

    public delegate void CodonUpdateHandler(List<NucleobaseType> currentCodon);
    public event CodonUpdateHandler OnCodonUpdated;

    [Header("Managers")]
    [SerializeField]
    private PlayerSkillController skillController;

    [SerializeField]
    private EnemySpawner enemySpawner;

    [SerializeField]
    private NucleobaseSpawner nucleobaseSpawner;

    [SerializeField]
    private PeptideChainController peptideChainController;

    private int currentSessionAminoAcids = 0;
    private bool isGameActive = true;
    private bool isProcessingSequence = false;

    private Transform playerTransform;
    private PlayerStats playerStats;

    // Track current sequence of bases (max 3)
    private List<NucleobaseType> currentCodon = new List<NucleobaseType>();

    // [Protein Synthesis]
    private List<AminoAcidData> peptideChain = new List<AminoAcidData>();
    private bool isSynthesizing = false;

    public static ArcadeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        targetAminoAcids = GameConfig.Arcade.TargetAminoAcids;
    }

    public List<NucleobaseType> GetCurrentCodon()
    {
        return new List<NucleobaseType>(currentCodon);
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    private void Start()
    {
        EnsureRibosomeUI();
        InitializePlayer();

        if (!enemySpawner)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();

        if (!nucleobaseSpawner)
            nucleobaseSpawner = FindFirstObjectByType<NucleobaseSpawner>();

        nucleobaseSpawner.OnBaseCollected += CollectBase;

        UpdateUI();
    }

    private void EnsureRibosomeUI()
    {
        if (FindFirstObjectByType<RibosomeUI>())
            return;

        var canvas = FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            var obj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = obj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // Fail Fast: Assume prefab is assigned
        Instantiate(ribosomeUIPrefab, canvas.transform, false);
    }

    private void InitializePlayer()
    {
        var player = FindFirstObjectByType<PlayerController>();
        playerTransform = player.transform;

        playerStats = player.GetComponent<PlayerStats>();
        playerStats.OnDeath += HandlePlayerDeath;
        playerStats.OnHealthChanged += HandleHealthChanged;

        if (!skillController)
            skillController = player.GetComponent<PlayerSkillController>();

        if (!peptideChainController)
            peptideChainController = FindFirstObjectByType<PeptideChainController>();
    }

    private void Update()
    {
        if (!isGameActive)
            return;

        if (playerTransform == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
    }

    public void CollectBase(NucleobaseType type)
    {
        if (!isGameActive || isProcessingSequence)
            return;

        currentCodon.Add(type);
        OnCodonUpdated?.Invoke(new List<NucleobaseType>(currentCodon));

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

        // 1. 코돈 -> 아미노산 이름 변환 (CodonTable)
        string aminoAcidShortName = CodonTable.GetAminoAcid(currentCodon[0], currentCodon[1], currentCodon[2]);

        // 2. 아미노산 이름 -> 데이터 변환 (AminoAcidDefinitions)
        AminoAcidData data = AminoAcidDefinitions.GetData(aminoAcidShortName);

        Debug.Log($"formed {data.ShortName} from {currentCodon[0]}{currentCodon[1]}{currentCodon[2]}");

        ShowComboVisuals(data);

        // Trigger Skill - Fail Fast
        Debug.Log($"ArcadeManager invoking skill: {data.ShortName}");
        skillController.ActivateSkill(data.ShortName);

        // [Protein Synthesis Logic]
        HandleProteinSynthesis(data);

        currentSessionAminoAcids++;

        currentCodon.Clear();
        OnCodonUpdated?.Invoke(new List<NucleobaseType>(currentCodon));

        if (currentSessionAminoAcids >= targetAminoAcids)
        {
            EndGame(true);
        }

        UpdateUI();
        isProcessingSequence = false;
    }

    private void HandleProteinSynthesis(AminoAcidData data)
    {
        if (data.ShortName == "Met") // Start Codon
        {
            if (!isSynthesizing)
            {
                StartSynthesis(data);
            }
            else
            {
                // Met can also be an internal amino acid
                AddToChain(data);
            }
        }
        else if (data.ShortName == "Stop") // Stop Codon
        {
            if (isSynthesizing)
            {
                FinishSynthesis();
            }
            // If not synthesizing, "Stop" just does its skill (Unlimited Void)
            else
            {
                UpdatePlayerWeight(); // Ensure weight is updated (reset) if needed
            }
        }
        else // Regular Amino Acid
        {
            if (isSynthesizing)
            {
                AddToChain(data);
            }
        }
    }

    private void UpdatePlayerWeight()
    {
        if (playerStats == null)
            return;

        int count = peptideChain.Count;
        // Formula: 1.0 - (count * penalty)
        float penalty = count * GameConfig.Player.SpeedPenaltyPerAminoAcid;
        float multiplier = Mathf.Clamp(1f - penalty, GameConfig.Player.MinSpeedMultiplier, 1f);

        playerStats.SetWeightMultiplier(multiplier);
    }

    private void StartSynthesis(AminoAcidData startCodon)
    {
        isSynthesizing = true;
        peptideChain.Clear();
        peptideChain.Add(startCodon);
        UpdatePlayerWeight();

        Debug.Log("<color=cyan>[Ribosome]</color> Synthesis Started! (Start Codon)");

        peptideChainController.StartSynthesis(playerTransform);
    }

    private void AddToChain(AminoAcidData data)
    {
        peptideChain.Add(data);
        UpdatePlayerWeight();
        Debug.Log($"<color=cyan>[Ribosome]</color> Chain Elongation: {peptideChain.Count} AAs. Added {data.ShortName}");

        peptideChainController.AddAminoAcid(data);
    }

    private void FinishSynthesis()
    {
        isSynthesizing = false;

        // Calculate result BEFORE clearing, but clear chain visually first or after?
        // Logic: Check recipe -> Show popup -> Clear chain logic -> Reset Weight

        ProteinData result = ProteinDatabase.CheckRecipe(peptideChain);

        if (result != null)
        {
            Debug.Log(
                $"<color=yellow>[Protein Synthesized]</color> <b>{result.ProteinName}</b> ({result.Tier}) - {result.Description}"
            );
            // TODO: Grant Reward (Item/Stat)
            ShowProteinPopup(result);
        }
        else
        {
            Debug.Log("<color=grey>[Ribosome]</color> Synthesis Failed or Unknown Protein.");
        }

        peptideChain.Clear();
        UpdatePlayerWeight(); // Reset weight

        peptideChainController.FinishSynthesis();
    }

    private void ShowProteinPopup(ProteinData protein)
    {
        // Reuse or extend the combo popup for now
        // Fail Fast: Assume comboPopupText is assigned
        comboPopupText.text = $"<size=120%>{protein.ProteinName}</size>\n{protein.Description}";
        comboPopupText.color = protein.Color;
        StartCoroutine(AnimatePopup(comboPopupText));
    }

    private void ShowComboVisuals(AminoAcidData data)
    {
        // Fail Fast
        comboPopupText.text = $"{data.FullName}\n<size=80%>{data.SkillDescription}</size>";
        comboPopupText.color = data.Color;
        StartCoroutine(AnimatePopup(comboPopupText));
    }

    private IEnumerator AnimatePopup(TextMeshProUGUI textComp)
    {
        textComp.gameObject.SetActive(true);
        textComp.transform.localScale = Vector3.zero;

        float duration = AppConfig.UI.Popup.AnimateDuration;
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

        yield return new WaitForSeconds(AppConfig.UI.Popup.DisplayDuration);

        time = 0;
        duration = AppConfig.UI.Popup.FadeOutDuration;
        Vector3 startPos = textComp.transform.localPosition;
        Color startColor = textComp.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            textComp.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            textComp.transform.localPosition = startPos + Vector3.up * (AppConfig.UI.Popup.FloatDistance * t);
            yield return null;
        }

        textComp.gameObject.SetActive(false);
        textComp.transform.localPosition = Vector3.zero;
    }

    public void SpawnDrop(Vector3 position)
    {
        nucleobaseSpawner.SpawnDrop(position);
    }

    public void CollectAminoAcid()
    {
        currentSessionAminoAcids++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Fail Fast: progressText is required
        string healthInfo = "";
        if (playerStats != null)
        {
            healthInfo = $"HP: {playerStats.currentHealth}/{playerStats.MaxHealth}";
        }

        progressText.text = $"Amino Acids: {currentSessionAminoAcids} / {targetAminoAcids}\n{healthInfo}";
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

        enemySpawner.StopSpawning();

        if (win)
        {
            Debug.Log("Arcade Mode Cleared!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddAminoAcids(currentSessionAminoAcids);
            }
            winPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Game Over!");
            losePanel.SetActive(true);
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
