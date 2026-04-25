using UnityEngine;

public abstract class WeaponEffect : ScriptableObject
{
    public abstract void Apply(PlayerUnit attacker, EnemyUnit[] targets);
}