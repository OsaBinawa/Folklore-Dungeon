using System;
using UnityEngine;

public enum EffectType
{
    Damage,
    DelayAV,
    Slow,
    Shield,
 
}

[Serializable]
public class AttackEffect
{
    public EffectType Type;

    [Tooltip("Damage / AV delay / Slow amount")]
    public int Value;

    
}

