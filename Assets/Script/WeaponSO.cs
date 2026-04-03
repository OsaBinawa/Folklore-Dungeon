using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public string WeaponName;
    public int AttackBonus;
    public int skillCost;
    public Image icon;
    public ElementType Element;
    public AnimationClip AttackAnimation;
    public AttackEffect[] Effects;
}
