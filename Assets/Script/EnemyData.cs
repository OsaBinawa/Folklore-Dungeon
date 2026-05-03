using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private bool isUnique;

    [Header("Stats")]
    [SerializeField] private int maxHP;
    [SerializeField] private int speed;
    [SerializeField] private int baseDamage;

    [Header("Break")]
    [SerializeField] private List<ElementType> weaknesses;

    [Header("Attacks")]
    [SerializeField] private List<EnemyAttack> attacks;
    [SerializeField] private EnemyUltimate ultimate;
    public EnemyUltimate Ultimate => ultimate;
    public IReadOnlyList<EnemyAttack> Actions => attacks;
    public bool IsUnique => isUnique;
    public int MaxHP => maxHP;
    public int Speed => speed;
    public int BaseDamage => baseDamage;
    

    public IReadOnlyList<ElementType> Weaknesses => weaknesses;
    
}
[Serializable]
public class EnemyUltimate
{
    public string AnimationString;
    public int EnergyRequired;
}