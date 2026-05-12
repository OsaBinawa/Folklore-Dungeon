using UnityEngine;

public class BawangBase : EnemyUnit
{
    [Header("Bawang Shared")]
    [SerializeField] protected BawangBase sibling;
    [SerializeField] protected GameObject fusionPrefab;

    protected bool defeated;
    protected bool fusionTriggered;

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
        if (defeated)
        {
            OnActionFinished();
            return;
        }

        base.Act(player);
    }
}