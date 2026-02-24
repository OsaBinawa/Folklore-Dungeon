using UnityEngine;

public class Slots : MonoBehaviour
{
    [SerializeField] private WeaponSO equippedWeapon;

    public WeaponSO EquippedWeapon => equippedWeapon;
    public bool HasWeapon => equippedWeapon != null;

    public void Equip(WeaponSO weapon)
    {
        equippedWeapon = weapon;
    }

    /*public void Clear()
    {
        equippedWeapon = null;
    }*/
}
