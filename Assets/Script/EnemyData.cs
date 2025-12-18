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
    [SerializeField] private int maxToughness;

    [Header("Attacks")]
    [SerializeField] private List<EnemyAttack> attacks;
    public bool IsUnique => isUnique;
    public int MaxHP => maxHP;
    public int Speed => speed;
    public int BaseDamage => baseDamage;
    public int MaxToughness => maxToughness;

    public IReadOnlyList<ElementType> Weaknesses => weaknesses;
    public IReadOnlyList<EnemyAttack> Attacks => attacks;
}
