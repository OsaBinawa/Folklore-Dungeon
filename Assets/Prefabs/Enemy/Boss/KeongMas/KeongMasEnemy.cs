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

    private List<EnemyUnit> activeSummons = new();

    private bool stunned;
    private int stunTurnsRemaining;

    protected override void Setup()
    {
        base.Setup();

        shieldActive = false;
        stunned = false;
    }

    public override void Act(PlayerUnit player)
    {
        CleanupSummons();

        if (stunned)
        {
            stunTurnsRemaining--;

            Debug.Log(name + " stunned. Remaining turns: " + stunTurnsRemaining);

            if (stunTurnsRemaining <= 0)
            {
                stunned = false;
            }

            OnActionFinished();
            return;
        }

        if (shieldActive && activeSummons.Count <= 0)
        {
            BreakShield();

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

    private void SummonEnemies()
    {
        if (summonPool == null || summonPool.Count == 0)
        {
            Debug.LogWarning("Summon pool empty on " + name);
            return;
        }

        activeSummons.Clear();

        for (int i = 0; i < summonCount; i++)
        {
            EnemyUnit randomEnemy =
                summonPool[Random.Range(0, summonPool.Count)];

            if (randomEnemy == null)
                continue;

            Vector3 spawnPosition =
                summonParent != null
                ? summonParent.transform.position
                : transform.position;

            EnemyUnit summonedEnemy = Instantiate(
                randomEnemy,
                spawnPosition,
                Quaternion.identity,
                summonParent != null
                    ? summonParent.transform
                    : null
            );

            activeSummons.Add(summonedEnemy);
        }

        Debug.Log(name + " summoned " + activeSummons.Count + " enemies.");
    }

    private void ActivateShield()
    {
        shieldActive = true;

        runtimeWeaknesses =
            new List<ElementType>(shieldWeaknesses);

        Debug.Log(name + " activated shield.");
    }

    private void BreakShield()
    {
        shieldActive = false;

        runtimeWeaknesses =
            new List<ElementType>(exposedWeaknesses);

        stunned = true;
        stunTurnsRemaining = stunDuration;

        Debug.Log(name + " shield broken and stunned.");
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

    private void CleanupSummons()
    {
        activeSummons.RemoveAll(enemy => enemy == null);
    }

    public override bool CanBeTargeted()
    {
        return true;
    }
}