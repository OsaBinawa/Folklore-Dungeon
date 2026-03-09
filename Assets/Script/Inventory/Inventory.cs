using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Runtime")]
    [SerializeField] private Slots weaponSlot;
    [SerializeField] private List<WeaponSO> ownedWeapons = new();
    public IReadOnlyList<WeaponSO> OwnedWeapons => ownedWeapons;

    private void Start()
    {
        BuildUI();

        if (ownedWeapons.Count > 0 && !weaponSlot.HasWeapon)
            weaponSlot.Equip(ownedWeapons[0]);
    }

    public void AddWeapon(WeaponSO weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
            BuildUI();
        }
    }

    private void EquipWeapon(WeaponSO weapon)
    {
        if (!ownedWeapons.Contains(weapon))
            return;

        weaponSlot.Equip(weapon);
        Debug.Log($"[Inventory] Equipped {weapon.WeaponName}");
    }

    public void BuildUI()
    {
        /*foreach (Transform child in contentParent)
            Destroy(child.gameObject);*/

        foreach (var weapon in ownedWeapons)
        {
            GameObject btnObj = Instantiate(buttonPrefab, contentParent);
            Button button = btnObj.GetComponent<Button>();

            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = weapon.WeaponName;

            WeaponSO localWeapon = weapon;

            button.onClick.AddListener(() =>
            {
                EquipWeapon(localWeapon);
            });
        }
    }
    
}
