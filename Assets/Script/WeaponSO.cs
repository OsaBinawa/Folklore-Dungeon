using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public string WeaponName;
    public int AttackBonus;
    public int skillCost;
    public ElementType Element;
    public AnimationClip AttackAnimation;
    public AttackEffect[] Effects;
}
