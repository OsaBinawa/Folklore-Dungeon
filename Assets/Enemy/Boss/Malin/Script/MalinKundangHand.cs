using UnityEngine;

public class MalinKundangHand : EnemyUnit
{
    [Header("Boss")]
    [SerializeField] protected MalinKundangMain boss;
    [SerializeField] protected bool isLeft;

    [Header("State")]
    [SerializeField] public bool isDeadHand;

    public bool IsLeft => isLeft;

    protected override void Die()
    {
        if (isDeadHand)
            return;
        isDeadHand = true;
        if (boss != null)
            boss.NotifyHandDead(this);
        currentHP = 0;

        if (anim != null)
            anim.SetTrigger("Dead");
        HpBarUpdate();
    }

    public override bool CanBeTargeted()
    {
        return !isDeadHand;
    }
    protected bool IsBossStunned()
    {
        return boss != null && boss.IsStunned();
    }

    public override void TakeDamage(int damage, ElementType element)
    {
        if (isDeadHand)
            return;

        base.TakeDamage(damage, element);
    }
}