using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TextMeshProUGUI aminoAcidText;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private TextMeshProUGUI attackText;

    [SerializeField]
    private Button upgradeButton;

    [SerializeField]
    private Button startArcadeButton;

    [Header("Settings UI")]
    [SerializeField]
    private Button settingsButton;

    [SerializeField]
    private GameObject settingsPanel;

    [SerializeField]
    private Button closeSettingsButton;

    [SerializeField]
    private Toggle virtualControlsToggle;

    private void Start()
    {
        UpdateUI();

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeClicked);

        if (startArcadeButton != null)
            startArcadeButton.onClick.AddListener(OnStartArcadeClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => SetSettingsPanelActive(true));

        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(() => SetSettingsPanelActive(false));

        if (virtualControlsToggle != null)
        {
            if (GameManager.Instance != null)
            {
                virtualControlsToggle.isOn = GameManager.Instance.ShowVirtualControls;
                virtualControlsToggle.onValueChanged.AddListener(OnVirtualControlsToggleChanged);
            }
        }

        // 초기엔 패널 닫기
        SetSettingsPanelActive(false);
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

    private void SetSettingsPanelActive(bool isActive)
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(isActive);
    }

    private void OnVirtualControlsToggleChanged(bool isOn)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowVirtualControls = isOn;
        }
    }
}
