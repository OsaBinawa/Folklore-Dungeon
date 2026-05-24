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
        // If already in untargetable mode
        if (this.isUntargetable)
        {
            turnsRemaining--;

            base.Act(player);

            if (turnsRemaining <= 0)
                ExitUntargetable();

            return;
        }
        //EnterUntargetable();

        anim.ResetTrigger("Skill");
        anim.SetTrigger("Skill");
    }

    private void EnterUntargetable()
    {
        isUntargetable = true;
        turnsRemaining = untargetableDuration;
        bonusAttack = attackBuff;

        Debug.Log($"{name} becomes untargetable and gains ATK");

        if (sr != null)
            sr.color = Color.gray;

        SetTargeted(false);
    }

    private void ExitUntargetable()
    {
        isUntargetable = false;
        bonusAttack = 0;

        Debug.Log($"{name} returns to normal");

        if (sr != null)
            sr.color = Color.white;
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

    public bool _CanBeTargeted()
    {
        return !isUntargetable;
    }
}
