using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MalinKundangMain : EnemyUnit
{
    public static event Action<string> OnTypeChanged;
    public static event Action<string> OnExplodeCountdown;

    [Header("References")]
    [SerializeField] private MalinKundangHand leftHand;
    [SerializeField] private MalinKundangHand rightHand;
    [SerializeField] private TMP_Text typeText;


    [Header("State")]
    private bool leftDead;
    private bool rightDead;

    private int stunTurns = 0;

    // NEW: final phase timer
    private int finalPhaseTurns = -1;

    protected override void Setup()
    {
        base.Setup();
    }

    public override void Act(PlayerUnit player)
    {
        if (finalPhaseTurns >= 0)
        {
            Debug.Log($"{name} is preparing self-destruct... {finalPhaseTurns}");
            OnExplodeCountdown?.Invoke(name + " is preparing self-destruct In "+ finalPhaseTurns + " Turn");
            finalPhaseTurns--;

            if (finalPhaseTurns <= 0)
            {
                SelfDestruct(player);
            }

            OnActionFinished();
            return;
        }

        // STUN
        if (stunTurns > 0)
        {
            stunTurns--;
            Debug.Log($"{name} is stunned");
            OnActionFinished();
            return;
        }

        // NORMAL ATTACK
        //base.Act(player);
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

            Debug.Log("Right hand destroyed → Missing type");
            OnTypeChanged?.Invoke("Right hand destroyed, type changed to Missing");

            typeText.text = "Missing";
            SetMissType();
        }

        // optional stun on hand break
        if (stunTurns <= 0)
            stunTurns = 2;

        // NEW RULE: both hands dead → start final phase
        if (leftDead && rightDead)
        {
            Debug.Log("Both hands destroyed → FINAL SELF-DESTRUCT PHASE STARTED!");

            finalPhaseTurns = 10;
        }
    }
    public override bool CanBeTargeted()
    {
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

    public bool IsStunned()
    {
        return stunTurns > 0;
    }
}