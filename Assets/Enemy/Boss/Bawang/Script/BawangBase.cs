using System;
using UnityEngine;

public class BawangBase : EnemyUnit
{
    public static event Action<string> OnSiblingDefeated,
    OnFusion,
    OnHealing,
    OnBuffExpired;

    [Header("Bawang Shared")]
    [SerializeField] protected BawangBase sibling;
    [SerializeField] protected GameObject fusionPrefab;

    protected bool defeated;
    protected bool fusionTriggered;

    private int attackBuffTurns;
    private int speedBuffTurns;

    public bool IsDefeated => defeated;

    public override bool CanBeTargeted()
    {
        return !defeated;
    }

    public override void TakeDamage(int damage, ElementType element)
    {
        if (defeated)
            return;

        bool isWeak = runtimeWeaknesses.Contains(element);
        float multiplier = isWeak ? 1.5f : 1f;

        int finalDamage = Mathf.RoundToInt(damage * multiplier);

        currentHP -= finalDamage;

        StartCoroutine(TakingDamageSpriteChange());

        HpBarUpdate();

        if (currentHP <= 0)
        {
            EnterDefeatedState();
        }
    }

    protected virtual void EnterDefeatedState()
    {
        defeated = true;

        currentHP = 1;

        Debug.Log(name + " defeated");

        OnSiblingDefeated?.Invoke(name + " is defeated");

        SetTargeted(false);

        if (sr != null)
            sr.color = Color.gray;

        CheckFusion();
    }

    protected void CheckFusion()
    {
        if (fusionTriggered)
            return;

        if (sibling != null && sibling.IsDefeated)
        {
            fusionTriggered = true;
            sibling.fusionTriggered = true;

            Debug.Log("Fusion phase starts!");

            OnFusion?.Invoke("Bawang Merah fused with Bawang Putih");

            Instantiate(
                fusionPrefab,
                transform.position,
                Quaternion.identity,
                transform.parent
            );

            Destroy(sibling.gameObject);
            Destroy(gameObject);
        }
    }

    public override void Act(PlayerUnit player)
    {
        UpdateBuffs();

        if (defeated)
        {
            OnActionFinished();
            return;
        }

        base.Act(player);
    }

    private void UpdateBuffs()
    {
        if (attackBuffTurns > 0)
        {
            attackBuffTurns--;

            if (attackBuffTurns <= 0)
            {
                attackMultiplier = 1f;

                Debug.Log(name + " ATK buff expired");

                OnBuffExpired?.Invoke("ATK buff expired");
            }
        }

        if (speedBuffTurns > 0)
        {
            speedBuffTurns--;

            if (speedBuffTurns <= 0)
            {
                speedMultiplier = 1f;

                Debug.Log(name + " SPD buff expired");

                OnBuffExpired?.Invoke("SPD buff expired");
            }
        }
    }

    public virtual void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > data.MaxHP)
            currentHP = data.MaxHP;

        HpBarUpdate();

        Debug.Log(name + " healed " + amount);
        OnHealing?.Invoke("Bawang Merah is healing " + name);
    }

    public virtual void ApplyAttackBuff(float percent, int duration)
    {
        attackMultiplier = 1f + percent;
        attackBuffTurns = duration;

        Debug.Log(name + " ATK buff applied");
    }

    public virtual void ApplySpeedBuff(float percent, int duration)
    {
        speedMultiplier = 1f + percent;
        speedBuffTurns = duration;

        Debug.Log(name + " SPD buff applied");
    }
}