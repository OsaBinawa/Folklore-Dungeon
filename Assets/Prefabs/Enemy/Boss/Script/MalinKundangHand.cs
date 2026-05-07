using UnityEngine;

public class MalinKundangHand : EnemyUnit
{
    [SerializeField] protected MalinKundangMain boss;
    [SerializeField] protected bool isLeft;

    public bool IsLeft => isLeft;

    protected override void Die()
    {
        if (boss != null)
            boss.NotifyHandDead(this);

        base.Die();
    }
}