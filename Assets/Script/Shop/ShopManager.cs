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
    public BuffSO buff;
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
        Clear();

        // 🔹 Build combined pool
        List<ShopItem> pool = new List<ShopItem>();

        foreach (var weapon in weapons)
        {
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


        // 🔹 Get random items
        var randomItems = GetRandomItems(pool, totalDisplayCount);

        // 🔹 Spawn UI
        foreach (var item in randomItems)
        {
            GameObject obj = Instantiate(itemPrefab, container);

            Button btn = obj.GetComponentInChildren<Button>();
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();

            btn.onClick.RemoveAllListeners();

            if (item.type == ShopItemType.Weapon)
            {
                text.text = item.weaponData.weapon.name + " - " + item.weaponData.price;
                btn.onClick.AddListener(() => BuyWeapon(item.weaponData, obj));
            }
            else
            {
                text.text = item.buffData.buff.name + " - " + item.buffData.price;
                btn.onClick.AddListener(() => BuyBuff(item.buffData, obj));
            }

            activeItems.Add(obj);
        }

        gameObject.SetActive(true);
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
                    // stackable → always allowed
                    validPool.Add(item);
                }
                else
                {
                    // non-stackable → only if not picked yet
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
        if (inventory.money >= buff.price)
        {
            inventory.money -= buff.price;
            inventorySlots.AddBuff(buff.buff);

            Debug.Log("Bought Weapon: " + buff.buff.name);

            Destroy(obj);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}
