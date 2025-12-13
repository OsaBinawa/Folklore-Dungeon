using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private int maxHP;
    [SerializeField] private int speed;
    [SerializeField] private int damage;

    [Header("Weakness")]
    [SerializeField] private List<ElementType> weaknesses;
    [SerializeField] private int maxToughness;

    public int MaxHP => maxHP;
    public int Speed => speed;
    public int Damage => damage;
    public IReadOnlyList<ElementType> Weaknesses => weaknesses;
    public int MaxToughness => maxToughness;
}
