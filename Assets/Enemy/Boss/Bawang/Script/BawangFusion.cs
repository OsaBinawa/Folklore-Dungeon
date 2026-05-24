using UnityEngine;

public class BawangFusion : EnemyUnit
{
    [Header("Buff")]
    [Range(0.0f, 1f)]
    [SerializeField] private float atkBuffPercent = 0.25f;
    [SerializeField] private int atkBuffDuration = 3;

    [Range(0.0f, 1f)]
    [SerializeField] private float spdBuffPercent = 0.25f;
    [SerializeField] private int spdBuffDuration = 3;

    private int currentAtkTurns;
    private int currentSpdTurns;

    public override void Act(PlayerUnit player)
    {
        UpdateBuffs();

        base.Act(player);
    }

    // Animation Event
    public void BuffATK()
    {
        attackMultiplier = 1f + atkBuffPercent;
        currentAtkTurns = atkBuffDuration;

        Debug.Log(name + " buffs ATK");
    }

    // Animation Event
    public void BuffSPD()
    {
        speedMultiplier = 1f + spdBuffPercent;
        currentSpdTurns = spdBuffDuration;

        Debug.Log(name + " buffs SPD");
    }

    private void UpdateBuffs()
    {
        if (currentAtkTurns > 0)
        {
            currentAtkTurns--;

            if (currentAtkTurns <= 0)
            {
                attackMultiplier = 1f;

                Debug.Log(name + " ATK buff expired");
            }
        }

        if (currentSpdTurns > 0)
        {
            currentSpdTurns--;

            if (currentSpdTurns <= 0)
            {
                speedMultiplier = 1f;

                Debug.Log(name + " SPD buff expired");
            }
        }
    }
}