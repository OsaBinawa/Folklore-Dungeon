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


    public void UpdateStats()
    {
        stats.Recalculate(weapon, armor);
        currentHP = stats.FinalHP;
    }

    public void BasicAttack()
    {
        if (currentTarget == null)
        {
            Debug.LogWarning("No target");
            return;
        }

        currentTarget.TakeDamage(stats.FinalATK, CurrentElement);
        Debug.Log("Player uses Basic Attack");
        turnManager.NotifyPlayerActionComplete();
    }

    public void TakeDamage(int amount, ElementType element)
    {
        currentHP -= amount;

        Debug.Log($"Player takes {amount} {element} damage. HP: {currentHP}");

        /*if (currentHP <= 0)
            Die();*/
    }


    // ================= TARGET =================

    public void SetTarget(EnemyUnit target)
    {
        currentTarget = target;
    }

}
