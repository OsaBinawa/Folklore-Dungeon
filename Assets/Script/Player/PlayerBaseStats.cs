using UnityEngine;
[CreateAssetMenu(menuName = "Player Base Stats")]
public class PlayerBaseStats : ScriptableObject
{
    public int maxHP = 80;
    public int baseAttack = 6;
    public int baseSpeed = 70;

    public Equipment[] startingEquipment;
}
