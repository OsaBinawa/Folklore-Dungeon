using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunData
{
    public event Action<int, int> OnHPChanged;

    public int CurrentHP { get; private set; }

    // Permanent HP before equipment/buffs
    public int BaseMaxHP { get; private set; }

    // Current calculated max HP
    public int MaxHP { get; private set; }

    public int BaseAttack;
    public int BaseSpeed;

    public int CurrentEnergy;
    public int MaxEnergy = 100;

    public List<Equipment> EquippedItems = new();

    public PlayerRunData(int maxHp, int baseAttack, int baseSpeed)
    {
        BaseMaxHP = maxHp;

        MaxHP = maxHp;
        CurrentHP = maxHp;

        BaseAttack = baseAttack;
        BaseSpeed = baseSpeed;

        RecalculateMaxHP();
    }

    public void TakesDamage(int amount)
    {
        CurrentHP -= amount;

        if (CurrentHP < 0)
            CurrentHP = 0;

        OnHPChanged?.Invoke(CurrentHP, MaxHP);
    }

    public void Heal(int amount)
    {
        CurrentHP = Math.Min(MaxHP, CurrentHP + amount);

        OnHPChanged?.Invoke(CurrentHP, MaxHP);
    }

    /// <summary>
    /// Recalculate HP from base value + equipment.
    /// Prevents HP from stacking every recalculation.
    /// </summary>
    public void RecalculateMaxHP()
    {
        int hp = BaseMaxHP;

        foreach (var eq in EquippedItems)
        {
            hp += eq.HPBonus;
        }

        MaxHP = hp;

        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }

        OnHPChanged?.Invoke(CurrentHP, MaxHP);
    }

    public void SetMaxHP(int newMaxHP)
    {
        MaxHP = newMaxHP;

        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }

        OnHPChanged?.Invoke(CurrentHP, MaxHP);
    }
    public void IncreaseMaxHP(int amount)
    {
        MaxHP += amount;

        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }

        OnHPChanged?.Invoke(CurrentHP, MaxHP);
    }

    public void IncreaseAttack(int amount)
    {
        BaseAttack += amount;
    }

    public void IncreaseSpeed(int amount)
    {
        BaseSpeed += amount;
    }

    public void GainEnergy(int amount)
    {
        CurrentEnergy = Mathf.Clamp(
            CurrentEnergy + amount,
            0,
            MaxEnergy
        );
    }

    public void ConsumeEnergy(int amount)
    {
        CurrentEnergy = Mathf.Clamp(
            CurrentEnergy - amount,
            0,
            MaxEnergy
        );
    }
}