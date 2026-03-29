using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSelectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BuffRandomise randomizer;
    [SerializeField] private Slots playerSlots;

    [Header("UI")]
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private Transform container;

    [Header("Settings")]
    [SerializeField] private int numberOfChoices = 3;
    [SerializeField] private int minRarity = 1;
    [SerializeField] private int maxRarity = 3;

    private List<GameObject> activeChoices = new();

    private void Awake()
    {
        playerSlots = FindFirstObjectByType<Slots>();
        Show();
    }
    public void Show()
    {
        Debug.Log("Show called");
        Clear();

        List<BuffSO> buffs = randomizer.GetChoices(numberOfChoices, minRarity, maxRarity);

        foreach (var buff in buffs)
        {
            GameObject obj = Instantiate(choicePrefab, container);
            Debug.Log("Instantiated: " + obj.name);

            Button btn = obj.GetComponentInChildren<Button>();
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();

            if (btn == null)
                Debug.LogError("Button not found in prefab!");

            if (text == null)
                Debug.LogError("TMP text not found in prefab!");

            text.text = buff.name;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => Select(buff));

            activeChoices.Add(obj);
        }


        gameObject.SetActive(true);
    }

    void Select(BuffSO buff)
    {
        playerSlots.AddBuff(buff);
        Clear();
        gameObject.SetActive(false);
    }

    void Clear()
    {
        foreach (var obj in activeChoices)
        {
            Destroy(obj);
        }

        activeChoices.Clear();
    }
}
