using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSelectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BuffRandomise randomizer;
    [SerializeField] private Slots playerSlots;
    [SerializeField] private NodeView nodeView;
    

    [Header("UI")]
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private Transform container;

    [Header("Settings")]
    [SerializeField] private int numberOfChoices = 3;
    [SerializeField] private int minRarity = 1;
    [SerializeField] private int maxRarity = 3;
    [SerializeField] private int picksRemaining;

    private List<GameObject> activeChoices = new();

    private void Awake()
    {
        playerSlots = FindFirstObjectByType<Slots>();
        //mapManager = FindFirstObjectByType<MapManager>();
        Show();
    }
    public void Show()
    {
        Debug.Log("Show called");
        Clear();

        picksRemaining = playerSlots.ApplyQuickread();

        List<BuffSO> buffs = randomizer.GetChoices(numberOfChoices, minRarity, maxRarity);

        foreach (var buff in buffs)
        {
            GameObject obj = Instantiate(choicePrefab, container);

            Button btn = obj.GetComponentInChildren<Button>();
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            //TextMeshProUGUI textDesc = obj.GetComponent<TextMeshProUGUI>();
            

            text.text = buff.Description;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => Select(buff, obj));

            activeChoices.Add(obj);
        }

        gameObject.SetActive(true);
    }

    void Select(BuffSO buff, GameObject obj)
    {
        playerSlots.AddBuff(buff);

        activeChoices.Remove(obj);
        Destroy(obj);

        picksRemaining--;

        if (picksRemaining <= 0)
        {
            Clear();
            gameObject.SetActive(false);
            nodeView.ResolveNode();
        }
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
