using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MalinKundangMain : EnemyUnit
{
    public static event Action<string> OnTypeChanged;
    [Header("References")]
    [SerializeField] private MalinKundangHand leftHand;
    [SerializeField] private MalinKundangHand rightHand;
    [SerializeField] private TMP_Text typeText;

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
            OnTypeChanged?.Invoke("Left hand destroyed, type changed to Typo");
            typeText.text = "Typo";
            SetTypoType();
        }
        else
        {
            rightDead = true;
            Debug.Log("Right hand destroyed → Miss type");
            OnTypeChanged?.Invoke("Left hand destroyed, type changed to Missing");
            typeText.text = "Missing";
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

    public override bool CanBeTargeted()
    {
        if (permanentTargetable)
            return true;

        return leftDead && rightDead;
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
    private bool IsAttackAllowed(EnemyAttack attack)
    {
        string anim = attack.AnimationString.ToLower();

        // Example naming rules (adjust to your real animation names)
        if (anim.Contains("left") && leftDead)
            return false;

        if (anim.Contains("right") && rightDead)
            return false;

        return true;
    }
    protected override EnemyAttack ChooseAttack()
    {
        var pool = data.Actions;

        var valid = new List<EnemyAttack>();

        foreach (var action in pool)
        {
            if (IsAttackAllowed(action))
                valid.Add(action);
        }

        if (valid.Count == 0)
            return pool[UnityEngine.Random.Range(0, pool.Count)];

        foreach (var action in valid)
        {
            if (UnityEngine.Random.value <= action.Chance)
                return action;
        }

        return valid[UnityEngine.Random.Range(0, valid.Count)];
    }
    public bool IsStunned()
    {
        return stunTurns > 0;
    }
}