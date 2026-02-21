using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class AttackBase 
{
    public string Name;

    [Header("Damage")]
    public int BaseDamage;
    public ElementType Element;
    public int HitCount = 1;

    [Header("Effects")]
    public List<AttackEffect> Effects = new List<AttackEffect>();
}
