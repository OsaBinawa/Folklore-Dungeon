using UnityEngine;
using UnityEngine.UI;

public class WeaponSelect : MonoBehaviour
{
    public GameObject Root; 
    private WeaponSO selectedChoice;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Slots slots;

    private void Awake()
    {
        playerInventory = FindFirstObjectByType<Inventory>();
        slots = FindFirstObjectByType<Slots>();
        
    }
    public void SelectChoice(WeaponSO choice)
    {
        playerInventory.AddWeapon(choice);
        slots.autoEquip(choice);
        Debug.Log("Selected: " + choice.WeaponName);
        Destroy(Root);
    }
}
