using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestNodes : MonoBehaviour
{
    [SerializeField] private PlayerStats player;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject Root;
    //[SerializeField] private List<ItemSO> items;

    //[Header("UI")]
    /*[SerializeField] private Transform contentParent;
    [SerializeField] private GameObject buttonPrefab;*/

    private void Start()
    {
        player = FindFirstObjectByType<PlayerStats>();
        inventory = FindFirstObjectByType<Inventory>();
        //BuildUI();
    }

    public void Heal(int healAmt)
    {
        player.Heal(healAmt);
        Root.SetActive(false);
    }

    /*public void BuildUI()
    {
        // Clear old buttons
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            GameObject btnObj = Instantiate(buttonPrefab, contentParent);
            Button button = btnObj.GetComponent<Button>();

            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = item.Name;

            ItemSO localItem = item;

            button.onClick.AddListener(() =>
            {
                UseItem(localItem);
            });
        }
    }*/

    public void UseItem(ItemSO item)
    {
        inventory.AddHeld(item);
        Root.SetActive(false);
        Debug.Log($"Used {item.Name} at Rest Node");
    }
}
