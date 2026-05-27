using UnityEngine;

public class EliteTypoEnemy : EnemyUnit
{
    [Header("Typo Settings")]
    [SerializeField] private int untargetableDuration = 3;
    [SerializeField] private int attackBuff = 10;
    [SerializeField] private float counterChance = 0.3f;

    [SerializeField]private bool isUntargetable;
    [SerializeField]private int turnsRemaining;
    private int bonusAttack;

    public override void Act(PlayerUnit player)
    {
        
    }

    public override void TakeDamage(int damage, ElementType element)
    {
        // Ignore if untargetable (single target protection)
        if (isUntargetable)
            return;

        base.TakeDamage(damage, element);

        // Counterattack chance
        if (Random.value <= counterChance)
        {
            Debug.Log($"{name} counterattacks!");

            PlayerUnit player = FindFirstObjectByType<PlayerUnit>();
            if (player != null)
            {
                player.TakeDamage(data.BaseDamage + bonusAttack, ElementType.None);
            }
        }
    }

    
}
