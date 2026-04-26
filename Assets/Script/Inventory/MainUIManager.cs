using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    [SerializeField] private GameObject InventoryRoot;
    [SerializeField] private GameObject WeaponSelectRoot;
    [SerializeField] private GameObject MapInventoryButton;
    private void Start()
    {
        WeaponSelectRoot.SetActive(true);
        InventoryRoot.SetActive(false);
    }
    public void openInv()
    {
        InventoryRoot.SetActive(true);
    }
    public void closeInv()
    {
        InventoryRoot.SetActive(false);
    }
    public void HideButton()
    {
        MapInventoryButton.SetActive(false);
    }
    public void ShowButton()
    {
        MapInventoryButton.SetActive(true);
    }
}
