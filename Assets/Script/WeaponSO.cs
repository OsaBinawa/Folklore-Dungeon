using UnityEngine;
using UnityEngine.UI;
public enum TargetType
{
    Single,
    Adjacent3,
    All,
    Random
}

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public string WeaponName;
    public ElementType Element;
    public Sprite icon;
    public Sprite _WeaponSprite;

    [Header("Skill")]
    public AnimationClip SkillAnimation;
    public TargetType SkillTargetType;
    public WeaponEffect[] SkillEffects;

    [Header("Ultimate")]
    public AnimationClip UltimateAnimation;
    public TargetType UltimateTargetType;
    public WeaponEffect[] UltimateEffects;
    public Sprite UltIcon;
    public int UltimateEnergyCost;
}
