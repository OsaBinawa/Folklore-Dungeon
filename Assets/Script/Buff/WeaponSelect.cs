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

    private WeaponSO selectedChoice;
    [SerializeField] private Inventory playerInventory;

    private void Awake()
    {
        playerInventory = FindFirstObjectByType<Inventory>();
        
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
            
            // Get UI components directly
            Text text = obj.GetComponentInChildren<Text>();
            Image image = obj.GetComponentInChildren<Image>();
            Button button = obj.GetComponentInChildren<Button>();

            // Set data
            if (text != null)
                text.text = choice.WeaponName;

            if (image != null && choice.icon != null)
                image = choice.icon;

            // Button click
            button.onClick.AddListener(() => SelectChoice(choice));
        }
    }

    void SelectChoice(WeaponSO choice)
    {
        playerInventory.AddWeapon(choice);
        Debug.Log("Selected: " + choice.WeaponName);
        Destroy(Container);
    }
}
