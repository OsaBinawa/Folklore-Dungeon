using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Equipment weapon;
    [SerializeField] private Equipment armor;

    [SerializeField] private int currentHP;
    [SerializeField] private float actionGauge;

    public PlayerStats Stats => stats;
    public int CurrentHP => currentHP;
    public float ActionGauge => actionGauge;

    public ElementType CurrentElement =>
        weapon != null ? weapon.Element : ElementType.Physical;

    private void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        stats.Recalculate(weapon, armor);
        currentHP = stats.FinalHP;
    }

    public void BasicAttack(EnemyUnit target, TurnManager turnManager)
    {
        target.TakeDamage(stats.FinalATK, CurrentElement);
        turnManager.EndPlayerTurn();
    }

    public void TakeDamage(int dmg) => currentHP -= dmg;
    public void AddGauge(float value) => actionGauge += value;
    public void ResetGauge() => actionGauge = 0;
}   
