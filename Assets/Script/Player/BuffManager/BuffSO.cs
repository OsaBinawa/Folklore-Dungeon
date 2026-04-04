using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Stat Buff")]
public class BuffSO : ScriptableObject
{
    [Header("Stat Modifiers (%)")]
    public float hpPercent;
    public float atkPercent;
    public float spdPercent;

    [Header("Special Effects")]
    public bool counter;
    public bool slowChance;
    public bool adjacentHit;
    public bool extraTurnOnKill;
    public bool quickRead;

    [Header("Stacking")]
    public bool stackable = true;

    public int rarity;
}
