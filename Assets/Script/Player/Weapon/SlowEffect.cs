using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Slow")]
public class SlowEffect : WeaponEffect
{
    public float chance = 0.3f;
    public float delayAV = 10f;

    public override void Apply(PlayerUnit attacker, EnemyUnit[] targets)
    {
        TurnManager tm = GameObject.FindFirstObjectByType<TurnManager>();

        foreach (var target in targets)
        {
            if (Random.value < chance)
            {
                tm.ModifyAV(target, delayAV);
            }
        }
    }
}
