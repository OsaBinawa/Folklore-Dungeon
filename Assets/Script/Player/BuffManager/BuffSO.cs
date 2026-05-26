using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Stat Buff")]
public class BuffSO : ScriptableObject
{
    [Header("Stat Modifiers (%)")]
    public float hpPercent;
    public float atkPercent;
    public float spdPercent;
    public float damageTakenPercent;
    public float atkBonusVsNonWeakPercent;

    [Header("Special Effects")]
    public bool counter;
    public bool slowChance;
    public bool adjacentHit;
    public bool extraTurnOnKill;
    public bool quickRead;
    public bool bonusVsNonWeak;

    [Header("Condition")]
    public bool stackable = true;
    public bool removeAble = true;
    public bool displayable = true;
    public Sprite Icon;
    public int rarity;
    [TextArea(5,10)]
    public string Description;
}
