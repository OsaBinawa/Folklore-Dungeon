using UnityEngine;

public class BawangMerah : BawangBase
{
    [Header("Merah")]
    [SerializeField] private int rageDamage = 50;
    [Range(0.0f, 1f)]
    [SerializeField] private float atkBuffPercent = 0.25f;
    [SerializeField] private int atkBuffDuration = 3;

    [Range(0.0f, 1f)]
    [SerializeField] private float spdBuffPercent = 0.25f;
    [SerializeField] private int spdBuffDuration = 3;

    [SerializeField] private int healAmount = 50;
    private int currentAtkTurns;
    private int currentSpdTurns;
    private int turnCounter;

    public override void Act(PlayerUnit player)
    {
        UpdateBuffs();
        if (defeated)
        {
            OnActionFinished();
            return;
        }

        // If Putih defeated
        if (sibling != null && sibling.IsDefeated)
        {
            turnCounter++;

            if (turnCounter % 3 == 0)
            {
                Debug.Log(name + " uses rage attack!");

                anim.SetTrigger("Skill");

                return;
            }
        }

        base.Act(player);
    }

    // Animation Event
    public void RageAttack()
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player != null)
        {
            player.TakeDamage(rageDamage, ElementType.Typo);
        }
    }

    // Optional support skills
    public void BuffATK()
    {
        if (sibling == null)
            return;

        if (sibling.IsDefeated)
            return;

        sibling.ApplyAttackBuff(
            atkBuffPercent,
            atkBuffDuration
        );

        Debug.Log(name + " buffs sibling ATK");
    }

    // Animation Event
    public void BuffSPD()
    {
        if (sibling == null)
            return;

        if (sibling.IsDefeated)
            return;

        sibling.ApplySpeedBuff(
            spdBuffPercent,
            spdBuffDuration
        );

        Debug.Log(name + " buffs sibling SPD");
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

    public void Heal()
    {
        if (sibling == null)
            return;

        if (sibling.IsDefeated)
            return;

        sibling.Heal(healAmount);

        Debug.Log(name + " heals " + sibling.name);
    }
}