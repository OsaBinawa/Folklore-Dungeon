using UnityEngine;

[CreateAssetMenu(menuName = "Create Equipment")]
public class Equipment : ScriptableObject
{
    [SerializeField] private int hpBonus;
    [SerializeField] private int atkBonus;
    [SerializeField] private int speedBonus;

    [Header("Element")]
    [SerializeField] private ElementType element;

    public int HPBonus => hpBonus;
    public int ATKBonus => atkBonus;
    public int SpeedBonus => speedBonus;
    public ElementType Element => element;
}
