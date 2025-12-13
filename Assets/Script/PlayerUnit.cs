using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Equipment weapon;
    [SerializeField] private Equipment armor;

    [Header("Runtime")]
    [SerializeField] private int currentHP;
    [SerializeField] private EnemyUnit currentTarget;
    [SerializeField] private TurnManager turnManager;

    public PlayerStats Stats => stats;

    public ElementType CurrentElement =>
        weapon != null ? weapon.Element : ElementType.Physical;

    private void Start()
    {
        UpdateStats();

        if (turnManager == null)
            turnManager = FindFirstObjectByType<TurnManager>();
    }

    // ======================
    // STATS
    // ======================

    public void UpdateStats()
    {
        stats.Recalculate(weapon, armor);
        currentHP = stats.FinalHP;
    }

    // ======================
    // PLAYER ACTION
    // ======================

    public void BasicAttack()
    {
        if (currentTarget == null) return;

        currentTarget.TakeDamage(stats.FinalATK, CurrentElement);

        turnManager.NotifyPlayerActionComplete();
    }

    public void SetTarget(EnemyUnit target)
    {
        currentTarget = target;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
    }
}
