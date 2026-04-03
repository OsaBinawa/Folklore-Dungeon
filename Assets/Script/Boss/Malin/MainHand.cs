using UnityEngine;

public class MainHand : EnemyUnit
{
    protected MalinController controller;

    protected override void Start()
    {
        base.Start();
        controller = FindFirstObjectByType<MalinController>();
    }

    protected override void Die()
    {
        controller.HandDestroyed(this);
        base.Die();
    }
}
