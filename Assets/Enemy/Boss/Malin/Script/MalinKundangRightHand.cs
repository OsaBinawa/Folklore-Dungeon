using UnityEngine;

public class MalinKundangRightHand : MalinKundangHand
{
    [Header("Miss Settings")]
    [SerializeField] private float delayChance = 0.5f;
    [SerializeField] private float delayAmount = 500f;

    [Header("Buff")]
    [SerializeField] private int buffEveryTurns = 3;
    [SerializeField] private int attackBuff = 10;

    private int turnCounter = 0;
    private int bonusAttack = 0;

    public override void Act(PlayerUnit player)
    {
        if (isDeadHand || IsBossStunned())
        {
            OnActionFinished();
            return;
        }
            
        turnCounter++;

        // Buff every 3 turns
        if (turnCounter % buffEveryTurns == 0)
        {
            bonusAttack += attackBuff;
            Debug.Log($"{name} gains permanent ATK!");
        }

        base.Act(player);
    }

    public void TryApplyDelay()
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player == null) return;

        if (Random.value <= delayChance)
        {
            Debug.Log($"{name} applies slow!");
            turnManager?.ModifyAV(player, delayAmount);
        }
    }

    public override void TakeDamage(int damage, ElementType element)
    {
        base.TakeDamage(damage, element);
    }
}