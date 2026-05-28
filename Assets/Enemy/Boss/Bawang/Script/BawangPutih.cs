using System;
using UnityEngine;

public class BawangPutih : BawangBase
{
    public static event Action<string> OnSelfBuff,
    OnSlow,
    OnCharging,
    OnChargeAttack;

    [Header("Putih")]
    [SerializeField] private int chargeBonus = 30;

    private bool charged;

    public override void Act(PlayerUnit player)
    {
        if (defeated)
        {
            OnActionFinished();
            return;
        }

        // If Merah defeated
        if (sibling != null && sibling.IsDefeated)
        {
            Debug.Log(name + " buffs itself");

            OnSelfBuff?.Invoke(name + " is raging and buffs it's own ATK and SPD");

            attackMultiplier = 1.5f;
            speedMultiplier = 1.5f;
        }

        base.Act(player);
    }

    // Animation Event
    public void ApplySlow()
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player != null)
        {
            Debug.Log(name + " slows player");

            OnSlow?.Invoke(name + " inflicted slow to player");

            turnManager?.ModifyAV(player, 500f);
        }
    }

    // Animation Event
    public void Charge()
    {
        charged = true;

        Debug.Log(name + " is charging");

        OnCharging?.Invoke(name + " is charging, will deal more damage on the next turn");
    }

    public void ApplyChargeBonus()
    {
        if (!charged)
            return;

        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player == null)
            return;

        player.TakeDamage(chargeBonus, ElementType.Typo);

        charged = false;

        Debug.Log(name + " triggers charge bonus damage");
        OnChargeAttack?.Invoke(name + " deal charged attack");
    }
    // Animation Event
    public void HeavyAttack()
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player == null)
            return;

        int damage = data.BaseDamage;

        if (charged)
        {
            damage += chargeBonus;
            charged = false;
        }

        player.TakeDamage(damage, ElementType.Typo);
    }
    

}