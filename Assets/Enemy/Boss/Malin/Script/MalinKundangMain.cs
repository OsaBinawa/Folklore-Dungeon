using UnityEngine;

public class MalinKundangMain : EnemyUnit
{
    [Header("References")]
    [SerializeField] private MalinKundangHand leftHand;
    [SerializeField] private MalinKundangHand rightHand;

    [Header("State")]
    private bool leftDead;
    private bool rightDead;

    private int stunTurns = 0;
    private int selfDestructTimer = -1;

    private bool permanentTargetable = false;

    protected override void Setup()
    {
        base.Setup();

        // Start untargetable
        permanentTargetable = false;
    }

    public override void Act(PlayerUnit player)
    {
        // Not targetable yet → do nothing
        if (!CanBeTargeted())
        {
            OnActionFinished();
            return;
        }

        // Stunned phase
        if (stunTurns > 0)
        {
            stunTurns--;
            Debug.Log($"{name} is stunned");
            OnActionFinished();
            return;
        }

        // Normal attack
        base.Act(player);

        // Self destruct countdown
        if (permanentTargetable)
        {
            selfDestructTimer--;

            if (selfDestructTimer <= 0)
            {
                SelfDestruct(player);
            }
        }
    }

    public void NotifyHandDead(MalinKundangHand hand)
    {
        if (hand.IsLeft)
        {
            leftDead = true;
            Debug.Log("Left hand destroyed → Typo type");
            SetTypoType();
        }
        else
        {
            rightDead = true;
            Debug.Log("Right hand destroyed → Miss type");
            SetMissType();
        }

        // Become targetable temporarily
        stunTurns = 3;

        // Both hands dead → final phase
        if (leftDead && rightDead)
        {
            permanentTargetable = true;
            runtimeWeaknesses.Clear();

            selfDestructTimer = 10;

            Debug.Log("Final phase started!");
        }
    }

    public bool _CanBeTargeted()
    {
        if (permanentTargetable)
            return true;

        return leftDead || rightDead;
    }

    private void SetTypoType()
    {
        runtimeWeaknesses.Clear();
        runtimeWeaknesses.Add(ElementType.Typo);
    }

    private void SetMissType()
    {
        runtimeWeaknesses.Clear();
        runtimeWeaknesses.Add(ElementType.Missing);
    }

    private void SelfDestruct(PlayerUnit player)
    {
        Debug.Log("SELF DESTRUCT!");

        if (player != null)
            player.TakeDamage(9999, ElementType.None);

        Die();
    }
}