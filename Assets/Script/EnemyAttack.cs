using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyAttack
{
    public string Name;

    [Header("Selection")]
    [Range(0f, 1f)]
    public float Chance = 1f;

    [Tooltip("Only used if enemy is Unique")]
    public int Cooldown;

    [Header("Element")]
    public ElementType Element;

    [Header("Visuals")]
    public string AnimationTrigger;

    [Header("Effects")]
    public List<AttackEffect> Effects;

    [Header("Energy")]
    public bool UsesEnergy;
    public int EnergyCost;

}
