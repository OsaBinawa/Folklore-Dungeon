using UnityEngine;
using System.Collections.Generic;

public class MainBody : EnemyUnit
{
    private MalinPhase phase = MalinPhase.Hidden;
    private MalinController controller;

    protected override void Start()
    {
        base.Start();
        controller = FindFirstObjectByType<MalinController>();
    }

    public void EnterStun()
    {
        phase = MalinPhase.Stunned;
    }

    public void ExitStun()
    {
        phase = MalinPhase.Active;
    }

    public void EnterFinalPhase()
    {
        phase = MalinPhase.FinalPhase;
    }

    public void TriggerSelfDestruct()
    {
        phase = MalinPhase.SelfDestruct;
    }

    public void SetWeakness(ElementType element)
    {
        runtimeWeaknesses.Clear();
        runtimeWeaknesses.Add(element);
    }

    public void RemoveWeakness()
    {
        runtimeWeaknesses.Clear();
    }


    public override void TakeDamage(int damage, ElementType element)
    {
        if (phase == MalinPhase.Hidden)
            return;

        base.TakeDamage(damage, element);
    }

    public override void Act(PlayerUnit player)
    {
        switch (phase)
        {
            case MalinPhase.Hidden:
                turnManager.NotifyEnemyActionComplete();
                return;

            case MalinPhase.Stunned:
                // Optional: play stunned animation
                // anim.SetTrigger("Stunned");
                turnManager.NotifyEnemyActionComplete();
                return;

            case MalinPhase.Active:
            case MalinPhase.FinalPhase:
                base.Act(player);
                return;

            case MalinPhase.SelfDestruct:
                anim.SetTrigger("SelfDestruct");
                return;
        }
    }

    // Called via animation event
    public void Explode()
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player != null)
            player.TakeDamage(9999, ElementType.Physical);

        OnActionFinished();
    }
}
