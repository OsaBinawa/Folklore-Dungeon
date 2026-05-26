using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TMP_Text MoneyText;
    [SerializeField] private MainUIManager mainUIManager;

    [Header("Consumable UI")]
    [SerializeField] private Transform consumableContentParent;
    [SerializeField] private GameObject consumableButtonPrefab;

    [Header("Runtime")]
    [SerializeField] private Slots weaponSlot;
    [SerializeField] private List<WeaponSO> ownedWeapons = new();
    [SerializeField] private List<ItemSO> ownedConsumables = new();
    [SerializeField] private List<ItemSO> heldConsumables = new();
    public int freeShopItemCount = 0;
    public IReadOnlyList<ItemSO> HeldConsumables => heldConsumables;
    public IReadOnlyList<ItemSO> OwnedConsumables => ownedConsumables;
    public int money;
    public IReadOnlyList<WeaponSO> OwnedWeapons => ownedWeapons;

    private void Start()
    {
        BuildUI();
        moneyUpdateUI();
        BuildConsumableUI();
       mainUIManager = FindAnyObjectByType<MainUIManager>();
        if (ownedWeapons.Count > 0 && !weaponSlot.HasWeapon)
            weaponSlot.Equip(ownedWeapons[0]);
    }

    private void Update()
    {
        moneyUpdateUI();
    }
    public bool HasWeapon(WeaponSO weapon)
    {
        foreach (var w in ownedWeapons)
        {
            if (w == weapon)
                return true;
        }

        return false;
    }
    public void AddConsumable(ItemSO item)
    {
        if (!ownedConsumables.Contains(item))
            ownedConsumables.Add(item);
    }
    public void UseConsumableOutsideCombat(ItemSO item)
    {
        if (!ownedConsumables.Contains(item))
            return;

        ownedConsumables.Remove(item); 

        if (!heldConsumables.Contains(item))
            heldConsumables.Add(item);

        BuildConsumableUI(); 
    }


    public void AddWeapon(WeaponSO weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
            BuildUI();
        }
    }

    public void BuildConsumableUI()
    {
        foreach (Transform child in consumableContentParent)
            Destroy(child.gameObject);

        Dictionary<ItemSO, int> grouped = new Dictionary<ItemSO, int>();

        foreach (var item in ownedConsumables)
        {
            if (!grouped.ContainsKey(item))
                grouped[item] = 0;

            grouped[item]++;
        }

        foreach (var pair in grouped)
        {
            ItemSO item = pair.Key;
            int count = pair.Value;

            GameObject btnObj = Instantiate(consumableButtonPrefab, consumableContentParent);
            Button button = btnObj.GetComponent<Button>();

            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();
            Image img = btnObj.GetComponentInChildren<Image>();
            if (label != null)
                label.text = $"{item.Name} x{count}";
            img.sprite = item.Sprite;

            button.onClick.AddListener(() =>
            {
                UseConsumableOutsideCombat(item);
                BuildConsumableUI(); 
            });

            if (heldConsumables.Contains(item))
                button.interactable = false;
        }
    }

    public void AddItem(ItemSO Item)
    {
        ownedConsumables.Add(Item);
    }
    public void AddHeld(ItemSO Item)
    {
        heldConsumables.Add(Item);
    }
    public void ClearHeldConsumables()
    {
        heldConsumables.Clear();
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
            Image img = btnObj.GetComponentInChildren<Image>();
            if (label != null)
                label.text = weapon.WeaponName;

            img.sprite = weapon.icon;
            WeaponSO localWeapon = weapon;

            button.onClick.AddListener(() =>
            {
                EquipWeapon(localWeapon);
            });
        }
    }

    void moneyUpdateUI()
    {
        MoneyText.text = money.ToString();
    }

    public void RemoveHeld(ItemSO item)
    {
        if (heldConsumables.Contains(item))
            heldConsumables.Remove(item);
    }

}
