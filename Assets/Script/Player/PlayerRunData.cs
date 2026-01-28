using System.Collections.Generic;

public class PlayerRunData
{
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }

    public int BaseAttack;
    public int BaseSpeed;

    public List<Equipment> EquippedItems = new();

    public PlayerRunData(int maxHp, int baseAttack, int baseSpeed)
    {
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
    }

    public void Heal(int amount)
    {
        CurrentHP = System.Math.Min(MaxHP, CurrentHP + amount);
    }

    public void RecalculateMaxHP()
    {
        int hp = MaxHP;

        foreach (var eq in EquippedItems)
            hp += eq.HPBonus;

        MaxHP = hp;

        if (CurrentHP > MaxHP)
            CurrentHP = MaxHP;
    }
}
