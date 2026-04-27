using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private PlayerRunData runData;

    public int FinalAttack { get; private set; }
    public int FinalSpeed { get; private set; }

    public int CurrentHP => runData.CurrentHP;
    public int MaxHP => runData.MaxHP;
    private int consumableAtkBonus;
    private int consumableSpdBonus;

    private void Start()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogError("RunManager not found!");
            return;
        }

        Initialize(RunManager.Instance.Player);
    }

    public void SetConsumableBonus(int atk, int spd)
    {
        consumableAtkBonus = atk;
        consumableSpdBonus = spd;

        RecalculateStats();
    }
    
    public void Initialize(PlayerRunData data)
    {
        Debug.Log("PlayerStats initialized with runData: " + data);
        runData = data;
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        FinalAttack = runData.BaseAttack;
        FinalSpeed = runData.BaseSpeed;

        foreach (var eq in runData.EquippedItems)
        {
            FinalAttack += eq.ATKBonus;
            FinalSpeed += eq.SpeedBonus;
        }
        FinalAttack += consumableAtkBonus;
        FinalSpeed += consumableSpdBonus;
    }
    public void RecalculateStatBuffs(Slots slots)
    {
        if (slots == null)
        {
            Debug.LogError("Slots is NULL");
            return;
        }

        if (slots.OwnedBuffs == null)
        {
            Debug.LogError("OwnedBuffs list is NULL");
            return;
        }

        float atkPercent = 0f;
        float spdPercent = 0f;

        foreach (var buff in slots.OwnedBuffs)
        {
            if (buff == null)
                continue;

            atkPercent += buff.atkPercent;
            spdPercent += buff.spdPercent;
        }

        FinalAttack = Mathf.RoundToInt(FinalAttack * (1 + atkPercent / 100f));
        FinalSpeed = Mathf.RoundToInt(FinalSpeed * (1 + spdPercent / 100f));
    }

    public void TakesDamage(int amount, Slots slots)
    {
        float reduction = 0f;

        foreach (var buff in slots.OwnedBuffs)
        {
            reduction += buff.damageTakenPercent;
        }

        float finalMultiplier = 1 + reduction / 100f;
        int finalDamage = Mathf.RoundToInt(amount * finalMultiplier);

        runData.TakesDamage(finalDamage);
    }


    public void Heal(int amount)
    {
        if (runData == null)
        {
            Debug.LogError("runData is NULL in Heal()");
            return;
        }

        runData.Heal(amount);
    }

}
