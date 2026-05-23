using TMPro;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    [SerializeField] private GameObject InventoryRoot;
    [SerializeField] private GameObject WeaponSelectRoot;
    [SerializeField] private GameObject MapInventoryButton;

    [Header("Reference")]
    [SerializeField] private PlayerStats playerStats;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Slots slots;

    [Header("UI")]
    [SerializeField] private Transform buffContainer;
    [SerializeField] private BuffCard buffPrefab;


    private int previousAttack;
    private int previousSpeed;
    private int previousHP;
    private void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found!");
            return;
        }
        RefreshBuffUI();
        RefreshStats(true);
        WeaponSelectRoot.SetActive(true);
        InventoryRoot.SetActive(false);
        slots = FindFirstObjectByType<Slots>();
    }
    public void openInv()
    {
        InventoryRoot.SetActive(true);
    }
    public void closeInv()
    {
        InventoryRoot.SetActive(false);
    }
    public void HideButton()
    {
        MapInventoryButton.SetActive(false);
    }
    public void ShowButton()
    {
        MapInventoryButton.SetActive(true);
    }

    public void RefreshStats(bool firstTime = false)
    {
        int currentAttack = playerStats.FinalAttack;
        int currentSpeed = playerStats.FinalSpeed;
        int currentHP = playerStats.CurrentHP;
        int maxHP = playerStats.MaxHP;

        if (firstTime)
        {
            previousAttack = currentAttack;
            previousSpeed = currentSpeed;
            previousHP = currentHP;
        }

        attackText.text = FormatStat(
            "",
            currentAttack,
            currentAttack - previousAttack
        );

        speedText.text = FormatStat(
            "",
            currentSpeed,
            currentSpeed - previousSpeed
        );

        hpText.text = FormatHPStat(
            currentHP,
            maxHP,
            currentHP - previousHP
        );

        previousAttack = currentAttack;
        previousSpeed = currentSpeed;
        previousHP = currentHP;
    }

    private string FormatStat(string statName, int value, int change)
    {
        string changeText = "";

        if (change > 0)
        {
            changeText = $" <color=green>(+{change})</color>";
        }
        else if (change < 0)
        {
            changeText = $" <color=red>({change})</color>";
        }

        return $"{statName} {value}{changeText}";
    }

    private string FormatHPStat(int currentHP, int maxHP, int change)
    {
        string changeText = "";

        if (change > 0)
        {
            changeText = $" <color=green>(+{change})</color>";
        }
        else if (change < 0)
        {
            changeText = $" <color=red>({change})</color>";
        }

        return $"{currentHP}/{maxHP}{changeText}";
    }

    public void RefreshBuffUI()
    {
        foreach (Transform child in buffContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var buff in slots.OwnedBuffs)
        {
            BuffCard entry = Instantiate(
                buffPrefab,
                buffContainer
            );

            entry.Setup(buff);
        }
    }
}
