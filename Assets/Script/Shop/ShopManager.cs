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
    
}
[System.Serializable]
public class Buff
{
    public BuffSO buff;
    public int price;
}

public class ShopManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform container;

    public List<Weapon> weapons;
    public List<Buff> buffs;

    private List<GameObject> activeItems = new List<GameObject>();


    private void Awake()
    {
        Show();
    }
    public void Show()
    {
        Debug.Log("Shop Show");
        Clear();

        // Show Weapons
        foreach (var weapon in weapons)
        {
            GameObject obj = Instantiate(itemPrefab, container);

            Button btn = obj.GetComponentInChildren<Button>();
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();

            text.text = weapon.weapon.name + " - " + weapon.price;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => BuyWeapon(weapon, obj));

            activeItems.Add(obj);
        }

        // Show Buffs
        foreach (var buff in buffs)
        {
            GameObject obj = Instantiate(itemPrefab, container);

            Button btn = obj.GetComponentInChildren<Button>();
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();

            text.text = buff.buff.name + " - " + buff.price;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => BuyBuff(buff, obj));

            activeItems.Add(obj);
        }

        gameObject.SetActive(true);
    }

    void Clear()
    {
        foreach (var obj in activeItems)
        {
            Destroy(obj);
        }
        activeItems.Clear();
    }


    private void BuyBuff(Buff buff, GameObject obj)
    {
        throw new NotImplementedException();
    }

    private void BuyWeapon(Weapon weapon, GameObject obj)
    {
        throw new NotImplementedException();
    }
}
