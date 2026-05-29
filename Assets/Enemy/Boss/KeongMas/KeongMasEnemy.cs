using System.Collections.Generic;
using UnityEngine;

public class KeongMasEnemy : EnemyUnit
{
    [Header("Summon Settings")]
    [SerializeField] private List<EnemyUnit> summonPool;
    [SerializeField] private GameObject summonParent;
    [SerializeField] private int summonCount = 3;

    [Header("Weakness States")]
    [SerializeField] private List<ElementType> shieldWeaknesses;
    [SerializeField] private List<ElementType> exposedWeaknesses;

    [Header("Shield")]
    [SerializeField] private bool shieldActive;

    [Header("Stun")]
    [SerializeField] private int stunDuration = 3;

    private readonly List<EnemyUnit> activeSummons = new();

    private bool stunned;
    private int stunTurnsRemaining;

    protected override void Setup()
    {
        base.Setup();

        shieldActive = false;
        stunned = false;
        stunTurnsRemaining = 0;

        runtimeWeaknesses = new List<ElementType>(exposedWeaknesses);
    }
    public override void Act(PlayerUnit player)
    {
        if (stunned)
        {
            Debug.Log(name + " is stunned");

            stunTurnsRemaining--;

            if (stunTurnsRemaining <= 0)
            {
                stunned = false;

                runtimeWeaknesses =
                    new List<ElementType>(exposedWeaknesses);
            }

            OnActionFinished();
            return;
        }

        if (shieldActive)
        {
            OnActionFinished();
            return;
        }

        base.Act(player);
    }

    public void AnimationEvent_ActivateShield()
    {
        ActivateShield();
    }

    public void AnimationEvent_SummonEnemies()
    {
        SummonEnemies();
    }

    private void ActivateShield()
    {
        shieldActive = true;

        runtimeWeaknesses =
            new List<ElementType>(shieldWeaknesses);
    }

    private void BreakShield()
    {
        shieldActive = false;

        stunned = true;
        stunTurnsRemaining = stunDuration;

        runtimeWeaknesses =
            new List<ElementType>(exposedWeaknesses);

        activeSummons.Clear();

        Debug.Log(name + " shield broken and stunned.");
    }

    private void SummonEnemies()
    {
        if (summonPool == null || summonPool.Count == 0)
        {
            Debug.LogWarning("Summon pool empty on " + name);
            return;
        }

        activeSummons.Clear();

        Vector3 spawnPosition =
            summonParent != null
            ? summonParent.transform.position
            : transform.position;

        for (int i = 0; i < summonCount; i++)
        {
            EnemyUnit prefab =
                summonPool[Random.Range(0, summonPool.Count)];

            if (prefab == null)
                continue;

            EnemyUnit summonedEnemy = Instantiate(
                prefab,
                spawnPosition,
                Quaternion.identity,
                summonParent != null
                    ? summonParent.transform
                    : null
            );

            summonedEnemy.Died += HandleSummonDeath;

            activeSummons.Add(summonedEnemy);
        }

        Debug.Log(name + " summoned " + activeSummons.Count + " enemies.");
    }

    private void HandleSummonDeath(EnemyUnit deadEnemy)
    {
        Debug.Log("SUMMON DIED: " + deadEnemy.name);

        activeSummons.Remove(deadEnemy);

        Debug.Log("Remaining summons: " + activeSummons.Count);

        if (shieldActive && activeSummons.Count == 0)
        {
            Debug.Log("ALL SUMMONS DEAD");
            BreakShield();
        }
    }

    public override void TakeDamage(int damage, ElementType element)
    {
        if (shieldActive)
        {
            Debug.Log(name + " blocked damage because shield is active.");
            return;
        }

        base.TakeDamage(damage, element);
    }

    public override bool CanBeTargeted()
    {
        return true;
    }
}