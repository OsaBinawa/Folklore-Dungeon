using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Slots : MonoBehaviour
{
    [SerializeField] private WeaponSO equippedWeapon;
    [SerializeField] private PlayerStats player;
    [SerializeField] private MainUIManager mainUIManager;
    [SerializeField] private List<BuffSO> ownedBuffs = new();
    public IReadOnlyList<BuffSO> OwnedBuffs => ownedBuffs;
    public WeaponSO EquippedWeapon => equippedWeapon;
    public bool HasWeapon => equippedWeapon != null;

    private void Awake()
    {
        mainUIManager = FindFirstObjectByType<MainUIManager>();
        player = FindAnyObjectByType<PlayerStats>();
    }
    public void Equip(WeaponSO weapon)
    {
        equippedWeapon = weapon;
    }
    public void AddBuff(BuffSO buff)
    {
        if (!buff.stackable && ownedBuffs.Contains(buff))
            return;

        ownedBuffs.Add(buff);
        player.RecalculateStats();
        player.RecalculateStatBuffs(this);
        mainUIManager.RefreshBuffUI();
        mainUIManager.RefreshStats();
    }
    public  int ApplyQuickread()
    {
        foreach (var buff in OwnedBuffs)
        {
            if (buff.quickRead)
                return 2;
        }

        return 1;
    }
    public void ClearBuffs()
    {
        ownedBuffs.Clear();
    }
    public void RemoveBuff(BuffSO buff)
    {
        if (ownedBuffs.Contains(buff))
            ownedBuffs.Remove(buff);
        mainUIManager.RefreshStats();
    }
    public void autoEquip(WeaponSO weap)
    {
        equippedWeapon = weap;
    }
    /*public void Clear()
    {
        equippedWeapon = null;
    }*/
}
