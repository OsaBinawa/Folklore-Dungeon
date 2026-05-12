using UnityEngine;

public class BawangMerah : BawangBase
{
    [Header("Merah")]
    [SerializeField] private int rageDamage = 50;

    private int turnCounter;

    public override void Act(PlayerUnit player)
    {
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
        Debug.Log(name + " buffs ATK");
    }

    public void BuffSPD()
    {
        Debug.Log(name + " buffs SPD");
    }

    public void Heal()
    {
        Debug.Log(name + " heals");
    }
}