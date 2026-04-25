using UnityEngine;
using UnityEngine.UI;

public class WeaponSelect : MonoBehaviour
{
    [Header("Data")]
    public WeaponSO[] choices;

    [Header("UI")]
    public GameObject choicePrefab;   
    public Transform parentContainer; 
    public GameObject Container; 
    public GameObject Root; 

    private WeaponSO selectedChoice;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Slots slots;

    private void Awake()
    {
        playerInventory = FindFirstObjectByType<Inventory>();
        slots = FindFirstObjectByType<Slots>();
        
    }

    void Start()
    {
        GenerateChoices();
    }

    void GenerateChoices()
    {
        Debug.Log("Total choices: " + choices.Length);
        foreach (var choice in choices)
        {
            Debug.Log("Spawning: " + choice.WeaponName);
            GameObject obj = Instantiate(choicePrefab, parentContainer);
            
            Text text = obj.GetComponentInChildren<Text>();
            Button button = obj.GetComponentInChildren<Button>();
            Image image = button.GetComponentInChildren<Image>();

            if (text != null)
                text.text = choice.WeaponName;

            if (image != null && choice.icon != null)
                image.sprite = choice.icon;

            button.onClick.AddListener(() => SelectChoice(choice));
        }
    }

    void SelectChoice(WeaponSO choice)
    {
        playerInventory.AddWeapon(choice);
        slots.autoEquip(choice);
        Debug.Log("Selected: " + choice.WeaponName);
        Destroy(Root);
    }
}
