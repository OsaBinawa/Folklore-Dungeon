using System.Collections.Generic;
using UnityEngine;

public class KeongMasEnemy : EnemyUnit
{
    [Header("Shield")]
    [SerializeField] private int maxShieldHP = 300;
    [SerializeField] private GameObject shieldVisual;
    private int currentShieldHP;
    private bool shieldActive = true;
    [Header("Stun")]
    [SerializeField] private int stunDuration = 3;
    private int stunnedTurns;
    [Header("Summon")]
    [SerializeField] private List<EnemyUnit> summonEnemies;
    [SerializeField] private GameObject[] summonSlots;

    private List<EnemyUnit> activeSummons = new();

    protected override void Start()
    {
        base.Start();

        ActivateShield();
        SummonEnemies();
    }

    public override void Act(PlayerUnit player)
    {
        activeSummons.RemoveAll(x => x == null);

        // While summons alive, Keong cannot act
        if (activeSummons.Count > 0)
        {
            Debug.Log(name + " is protected by summons");

            OnActionFinished();
            return;
        }

        // Stunned phase
        if (!shieldActive)
        {
            stunnedTurns--;

            Debug.Log(name + " stunned turns: " + stunnedTurns);

            if (stunnedTurns <= 0)
            {
                ActivateShield();
                SummonEnemies();
            }

            OnActionFinished();
            return;
        }

        // Normal behavior
        base.Act(player);
    }

    public override void TakeDamage(int damage, ElementType element)
    {
        activeSummons.RemoveAll(x => x == null);

        // Cannot be damaged while summons alive
        if (activeSummons.Count > 0)
        {
            Debug.Log(name + " is protected by summons");
            return;
        }

        // Shield phase
        if (shieldActive)
        {
            // Only weak to Miss while shield active
            if (element != ElementType.Fix)
            {
                Debug.Log(name + " resisted damage");
                return;
            }

            currentShieldHP -= damage;

            Debug.Log(name + " shield HP: " + currentShieldHP);

            StartCoroutine(TakingDamageSpriteChange());

            if (currentShieldHP <= 0)
            {
                BreakShield();
            }

            return;
        }

        // Vulnerable phase
        // Only weak to Typo while shield broken
        if (element != ElementType.Typo)
        {
            Debug.Log(name + " resisted damage");
            return;
        }

        base.TakeDamage(damage, element);
    }

    public override bool CanBeTargeted()
    {
        activeSummons.RemoveAll(x => x == null);

        return activeSummons.Count == 0;
    }

    private void ActivateShield()
    {
        shieldActive = true;
        currentShieldHP = maxShieldHP;

        runtimeWeaknesses.Clear();
        runtimeWeaknesses.Add(ElementType.Fix);

        Debug.Log(name + " restored shield");

        if (shieldVisual != null)
            shieldVisual.SetActive(true);
    }

    private void BreakShield()
    {
        shieldActive = false;
        stunnedTurns = stunDuration;

        runtimeWeaknesses.Clear();
        runtimeWeaknesses.Add(ElementType.Typo);

        Debug.Log(name + " shield broken!");

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    private void SummonEnemies()
    {
        activeSummons.RemoveAll(x => x == null);

        
        if (activeSummons.Count > 0)
            return;

        for (int i = 0; i < summonSlots.Length; i++)
        {
            EnemyUnit randomEnemy =
                summonEnemies[Random.Range(0, summonEnemies.Count)];

            EnemyUnit spawned = Instantiate(
                randomEnemy,
                summonSlots[i].transform
            );

            // Reset UI transform
            spawned.transform.localPosition = Vector3.zero;
            spawned.transform.localRotation = Quaternion.identity;
            spawned.transform.localScale = Vector3.one;

            activeSummons.Add(spawned);
        }

        Debug.Log(name + " summoned enemies");
    }
}