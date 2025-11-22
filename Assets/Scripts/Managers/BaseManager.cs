using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI aminoAcidText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI attackText;
    public Button upgradeButton;
    public Button startArcadeButton;

    private void Start()
    {
        UpdateUI();

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeClicked);

        if (startArcadeButton != null)
            startArcadeButton.onClick.AddListener(OnStartArcadeClicked);
    }

    private void UpdateUI()
    {
        if (GameManager.Instance == null)
            return;

        if (aminoAcidText != null)
            aminoAcidText.text = $"Amino Acids: {GameManager.Instance.AminoAcids}";

        if (levelText != null)
            levelText.text = $"Mitochondria Lv: {GameManager.Instance.MitochondriaLevel}";

        if (attackText != null)
            attackText.text = $"T-Cell Attack: {GameManager.Instance.TCellAttackPower}";
    }

    private void OnUpgradeClicked()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.TryUpgradeMitochondria())
            {
                UpdateUI();
            }
        }
    }

    private void OnStartArcadeClicked()
    {
        SceneManager.LoadScene("ArcadeScene");
    }
}
