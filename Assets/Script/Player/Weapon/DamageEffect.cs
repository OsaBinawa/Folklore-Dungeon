using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Damage")]
public class DamageEffect : WeaponEffect
{
    public int baseDamage;

    public override void Apply(PlayerUnit attacker, EnemyUnit[] targets)
    {
        foreach (var target in targets)
        {
            int dmg = baseDamage + attacker.Stats.FinalAttack;
            target.TakeDamage(dmg, attacker.EquippedWeapon.Element);
        }
    }
}
