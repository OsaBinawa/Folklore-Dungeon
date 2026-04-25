using UnityEngine;

[CreateAssetMenu(menuName = "Effects/MultiHit")]
public class MultiHitEffect : WeaponEffect
{
    public int hitCount;
    public int damagePerHit;

    public override void Apply(PlayerUnit attacker, EnemyUnit[] targets)
    {
        for (int i = 0; i < hitCount; i++)
        {
            var target = targets[Random.Range(0, targets.Length)];

            int dmg = damagePerHit + attacker.Stats.FinalAttack;
            target.TakeDamage(dmg, attacker.EquippedWeapon.Element);
        }
    }
}
