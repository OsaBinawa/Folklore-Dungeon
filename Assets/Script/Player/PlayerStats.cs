using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private PlayerRunData runData;

    public int FinalAttack { get; private set; }
    public int FinalSpeed { get; private set; }

    public int CurrentHP => runData.CurrentHP;
    public int MaxHP => runData.MaxHP;

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
    }

    public void TakesDamage(int amount)
    {
        runData.TakesDamage(amount);
    }

    public void Heal(int amount)
    {
        runData.Heal(amount);
    }
}
