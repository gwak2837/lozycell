using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int AminoAcids { get; private set; }
    public int MitochondriaLevel { get; private set; }

    // Configuration
    private const int UpgradeCost = 100;
    private const int BaseAttack = 10;
    private const int AttackPerLevel = 5;

    public int TCellAttackPower => BaseAttack + (MitochondriaLevel * AttackPerLevel);

    public event System.Action<bool> OnVirtualControlsSettingChanged;

    public bool ShowVirtualControls
    {
        get
        {
            // Default to 1 (true) if mobile, 0 (false) otherwise
            int defaultValue = Application.isMobilePlatform ? 1 : 0;
            return PlayerPrefs.GetInt(GameConfig.Settings.ShowVirtualControlsKey, defaultValue) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(GameConfig.Settings.ShowVirtualControlsKey, value ? 1 : 0);
            PlayerPrefs.Save();
            OnVirtualControlsSettingChanged?.Invoke(value);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddAminoAcids(int amount)
    {
        AminoAcids += amount;
        SaveData();
        Debug.Log($"Amino Acids added: {amount}. Total: {AminoAcids}");
    }

    public bool TryUpgradeMitochondria()
    {
        if (AminoAcids >= UpgradeCost)
        {
            AminoAcids -= UpgradeCost;
            MitochondriaLevel++;
            SaveData();
            Debug.Log($"Mitochondria Upgraded! Level: {MitochondriaLevel}, Attack: {TCellAttackPower}");
            return true;
        }

        Debug.Log("Not enough Amino Acids!");
        return false;
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("AminoAcids", AminoAcids);
        PlayerPrefs.SetInt("MitochondriaLevel", MitochondriaLevel);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        AminoAcids = PlayerPrefs.GetInt("AminoAcids", 0);
        MitochondriaLevel = PlayerPrefs.GetInt("MitochondriaLevel", 1);
    }
}
