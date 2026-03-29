using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    [SerializeField] private WeaponSO equippedWeapon;
    [SerializeField] private List<BuffSO> ownedBuffs = new();
    public IReadOnlyList<BuffSO> OwnedBuffs => ownedBuffs;
    public WeaponSO EquippedWeapon => equippedWeapon;
    public bool HasWeapon => equippedWeapon != null;

    public void Equip(WeaponSO weapon)
    {
        equippedWeapon = weapon;
    }
    public void AddBuff(BuffSO buff)
    {
        if (!buff.stackable && ownedBuffs.Contains(buff))
            return;

        ownedBuffs.Add(buff);
    }
    /*public void Clear()
    {
        equippedWeapon = null;
    }*/
}
