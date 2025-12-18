using System;
using UnityEngine;

public enum EffectType
{
    Damage,
    DelayAV,
    Slow
    // future mechanics go here
}

[Serializable]
public class AttackEffect
{
    public EffectType Type;

    [Tooltip("Damage / AV delay / Slow amount")]
    public int Value;

    /*[Tooltip("Duration in turns (if applicable)")]
    public int Duration;*/
}
