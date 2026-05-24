using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Weapon
{
    public WeaponSO weapon;
    public int price;
    public bool stackable;
}

[System.Serializable]
public class Buff
{
    public ItemSO Item;
    public int price;
    public bool stackable;
}

public enum ShopItemType
{
    Weapon,
    Buff
}

[System.Serializable]
public class ShopItem
{
    public ShopItemType type;
    public Weapon weaponData;
    public Buff buffData;
}

public class ShopManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform container;

    public ShopUICard slot1;
    public ShopUICard slot2;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Slots inventorySlots;
    public List<Weapon> weapons;
    public List<Buff> buffs;

    public int totalDisplayCount = 5;

    private List<GameObject> activeItems = new List<GameObject>();

    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        inventorySlots = FindFirstObjectByType<Slots>();
        Show();
    }

    public void Show()
    {
        Debug.Log("Shop Show");

        List<ShopItem> pool = new List<ShopItem>();

        foreach (var weapon in weapons)
        {
            // Skip if player already owns this weapon
            if (inventory.HasWeapon(weapon.weapon))
                continue;

            int count = weapon.stackable ? UnityEngine.Random.Range(1, 4) : 1;

            for (int i = 0; i < count; i++)
            {
                pool.Add(new ShopItem
                {
                    type = ShopItemType.Weapon,
                    weaponData = weapon
                });
            }
        }

        foreach (var buff in buffs)
        {
            int count = buff.stackable ? UnityEngine.Random.Range(1, 4) : 1;

            for (int i = 0; i < count; i++)
            {
                pool.Add(new ShopItem
                {
                    type = ShopItemType.Buff,
                    buffData = buff
                });
            }
        }

        var randomItems = GetRandomItems(pool, 2); // only 2 now

        // SLOT 1
        SetupSlot(slot1, randomItems[0]);

        // SLOT 2
        if (randomItems.Count > 1)
            SetupSlot(slot2, randomItems[1]);
    }

    void SetupSlot(ShopUICard slot, ShopItem item)
    {
        if (item.type == ShopItemType.Weapon)
        {
            slot.SetupWeapon(item.weaponData, () =>
            {
                BuyWeapon(item.weaponData, slot.gameObject);
            });
        }
        else
        {
            slot.SetupBuff(item.buffData, () =>
            {
                BuyBuff(item.buffData, slot.gameObject);
            });
        }
    }


    private List<ShopItem> GetRandomItems(List<ShopItem> pool, int count)
    {
        List<ShopItem> result = new List<ShopItem>();
        HashSet<ShopItem> pickedNonStackable = new HashSet<ShopItem>();

        for (int i = 0; i < count; i++)
        {
            List<ShopItem> validPool = new List<ShopItem>();

            foreach (var item in pool)
            {
                if (IsStackable(item))
                {
                    
                    validPool.Add(item);
                }
                else
                {
                    
                    if (!pickedNonStackable.Contains(item))
                        validPool.Add(item);
                }
            }

            if (validPool.Count == 0)
                break;

            var picked = validPool[UnityEngine.Random.Range(0, validPool.Count)];
            result.Add(picked);

            if (!IsStackable(picked))
            {
                pickedNonStackable.Add(picked);
            }
        }

        return result;
    }

    private bool IsStackable(ShopItem item)
    {
        if (item.type == ShopItemType.Weapon)
            return item.weaponData.stackable;

        return item.buffData.stackable;
    }

    void Clear()
    {
        foreach (var obj in activeItems)
        {
            Destroy(obj);
        }
        activeItems.Clear();
    }

    private void BuyWeapon(Weapon weapon, GameObject obj)
    {
        if (inventory.money >= weapon.price)
        {
            inventory.money -= weapon.price;
            inventory.AddWeapon(weapon.weapon);

            Debug.Log("Bought Weapon: " + weapon.weapon.name);

            Destroy(obj); 
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    private void BuyBuff(Buff buff, GameObject obj)
    {
        bool useFree = inventory.freeShopItemCount > 0;

        if (useFree || inventory.money >= buff.price)
        {
            if (useFree)
            {
                inventory.freeShopItemCount--;
                Debug.Log("Used FREE coupon!");
            }
            else
            {
                inventory.money -= buff.price;
            }
            inventory.AddConsumable(buff.Item);
            

            Destroy(obj);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

}
