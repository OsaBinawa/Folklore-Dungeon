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

    private void Awake()
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

        int baseAttack = FinalAttack;
        int baseSpeed = FinalSpeed;

        float atkPercent = 0f;
        float spdPercent = 0f;
        float hpPercent = 0f;

        Debug.Log("========== RECALCULATING BUFFS ==========");

        foreach (var buff in slots.OwnedBuffs)
        {
            if (buff == null)
                continue;

            atkPercent += buff.atkPercent;
            spdPercent += buff.spdPercent;
            hpPercent += buff.hpPercent;

            Debug.Log(
                $"[Buff Applied] {buff.name} | " +
                $"ATK: {buff.atkPercent}% | " +
                $"SPD: {buff.spdPercent}% | " +
                $"HP: {buff.hpPercent}%"
            );
        }

        FinalAttack = Mathf.RoundToInt(
            baseAttack * (1 + atkPercent / 100f)
        );

        FinalSpeed = Mathf.CeilToInt(
            baseSpeed * (1 + spdPercent / 100f)
        );

        // IMPORTANT:
        // Always calculate HP from BaseMaxHP,
        // never from current MaxHP.

        int baseHP = runData.BaseMaxHP;

        foreach (var eq in runData.EquippedItems)
        {
            baseHP += eq.HPBonus;
        }

        int oldMaxHP = runData.MaxHP;
        int oldCurrentHP = runData.CurrentHP;

        int boostedHP = Mathf.RoundToInt(
            baseHP * (1 + hpPercent / 100f)
        );

        float hpRatio = oldMaxHP > 0
            ? (float)oldCurrentHP / oldMaxHP
            : 1f;

        runData.SetMaxHP(boostedHP);

        int hpAfterBuff = Mathf.RoundToInt(boostedHP * hpRatio);

        if (hpAfterBuff > oldCurrentHP)
        {
            runData.Heal(hpAfterBuff - oldCurrentHP);
        }

        Debug.Log(
            $"[HP Buff Calculation] " +
            $"BaseHP={baseHP} | " +
            $"Buff={hpPercent}% | " +
            $"FinalHP={boostedHP}"
        );

        Debug.Log(
            $"[Final Stats] " +
            $"ATK={FinalAttack} | " +
            $"SPD={FinalSpeed} | " +
            $"MaxHP={runData.MaxHP}"
        );

        Debug.Log("=========================================");
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
