using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    [Header("Event Data")]
    [SerializeField] private EventData[] eventData;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button nextButton;

    [Header("Choices UI")]
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("References")]
    [SerializeField] private Slots slots;
    [SerializeField] private Inventory inventory;

    private bool usingChoiceDialogue = false;
    private string[] currentChoiceDialogue;
    private int choiceDialogueIndex = 0;

    private Event eventInstance;
    private int dialogueIndex = 0;
    public bool useRandom;
    public int eventIndex;

    private void Awake()
    {
        slots = FindFirstObjectByType<Slots>();
        inventory = FindFirstObjectByType<Inventory>();
    }
    private void Start()
    {
        int index = useRandom
        ? Random.Range(0, eventData.Length)
        : eventIndex;

        eventInstance = new Event(eventData[index]);
        StartEvent();
    }

    private void StartEvent()
    {
        dialogueIndex = 0;
        panel.SetActive(true);

        nextButton.gameObject.SetActive(true);

        ShowDialogue();

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
    }

    private void ShowDialogue()
    {
        
        if (usingChoiceDialogue)
        {
            if (choiceDialogueIndex >= currentChoiceDialogue.Length)
            {
                usingChoiceDialogue = false;
                EndEvent();
                return;
            }

            dialogueText.text = currentChoiceDialogue[choiceDialogueIndex];
            nextButton.gameObject.SetActive(true);
            return;
        }

       
        if (dialogueIndex >= eventInstance.dialogue.Length)
        {
            EndEvent();
            return;
        }

        ClearChoices();

        dialogueText.text = eventInstance.dialogue[dialogueIndex];

        bool hasChoice = false;

        foreach (var choice in eventInstance.choices)
        {
            if (choice.triggerIndex == dialogueIndex)
            {
                hasChoice = true;
                break;
            }
        }

        if (hasChoice)
        {
            ShowChoicesAtIndex(dialogueIndex);
            return;
        }

        nextButton.gameObject.SetActive(true);
    }

    private void ShowChoicesAtIndex(int index)
    {
        nextButton.gameObject.SetActive(false);

        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        foreach (var choice in eventInstance.choices)
        {
            if (choice.triggerIndex != index)
                continue;

            GameObject btn = Instantiate(choiceButtonPrefab, choicesContainer);

            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.text = choice.text;

            Button button = btn.GetComponent<Button>();
            button.onClick.AddListener(() => SelectChoice(choice));
        }
    }

    private void NextDialogue()
    {
        if (usingChoiceDialogue)
        {
            choiceDialogueIndex++;
            ShowDialogue();
            return;
        }

        dialogueIndex++;
        ShowDialogue();
    }


    private void ShowChoices()
    {
        nextButton.gameObject.SetActive(false);

        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        foreach (var choice in eventInstance.choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choicesContainer);

            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.text = choice.text;

            Button button = btn.GetComponent<Button>();
            button.onClick.AddListener(() => SelectChoice(choice));
        }
    }

    private void SelectChoice(EventChoice choice)
    {
        ClearChoices();

    
        if (choice.buff != null)
        {
            slots.AddBuff(choice.buff);
        }

     
        switch (choice.effect)
        {
            case EventEffectType.GainOwnedBuffChoice:
                ShowOwnedBuffChoices();
                return;

            case EventEffectType.GiveAmountOffMoney:
                GainMoney(choice);
                return;

            case EventEffectType.RerollAllBuffs:
                RerollAllBuffs();
                return;

            case EventEffectType.GainAllBuffs:
                GainAllBuffs();
                return;

        }


        if (choice.resultDialogue != null && choice.resultDialogue.Length > 0)
        {
            usingChoiceDialogue = true;
            currentChoiceDialogue = choice.resultDialogue;
            choiceDialogueIndex = 0;

            ShowDialogue();
            return;
        }

        EndEvent();
    }

    private void GainAllBuffs()
    {
        var allBuffs = RunManager.Instance.AllAvailableBuff;

        if (allBuffs == null || allBuffs.Count == 0)
        {
            Debug.Log("No buffs available.");
            EndEvent();
            return;
        }

        foreach (var buff in allBuffs)
        {
            slots.AddBuff(buff);
        }

        Debug.Log("Gained all available buffs!");

        EndEvent();
    }

    private void RerollAllBuffs()
    {
        var allBuffs = RunManager.Instance.AllAvailableBuff;

        if (allBuffs == null || allBuffs.Count == 0)
        {
            Debug.Log("No available buffs to roll.");
            EndEvent();
            return;
        }

        var currentBuffs = slots.OwnedBuffs;

        if (currentBuffs.Count == 0)
        {
            Debug.Log("Player has no buffs.");
            EndEvent();
            return;
        }

        
        List<BuffSO> newBuffs = new List<BuffSO>();

        foreach (var buff in currentBuffs)
        {
            BuffSO randomBuff = allBuffs[Random.Range(0, allBuffs.Count)];
            newBuffs.Add(randomBuff);
        }

        
        ClearAllBuffs();

        foreach (var buff in newBuffs)
        {
            slots.AddBuff(buff);
        }

        Debug.Log("Buffs rerolled!");

        EndEvent();
    }
    private void ClearAllBuffs()
    {
        slots.ClearBuffs();
    }

    public void ShowOwnedBuffChoices()
    {
        nextButton.gameObject.SetActive(false);

      
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        if (slots.OwnedBuffs.Count == 0)
        {
            Debug.Log("No buffs owned");
            EndEvent();
            return;
        }

     
        bool hasStackable = false;

        foreach (var buff in slots.OwnedBuffs)
        {
            if (buff.stackable)
            {
                hasStackable = true;
                break;
            }
        }

        if (!hasStackable)
        {
            Debug.Log("No stackable buffs available");
            EndEvent();
            return;
        }

        
        foreach (var buff in slots.OwnedBuffs)
        {
            if (!buff.stackable)
                continue;

            GameObject btn = Instantiate(choiceButtonPrefab, choicesContainer);

            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.text = buff.name;

            Button button = btn.GetComponent<Button>();
            button.onClick.AddListener(() => SelectOwnedBuff(buff));
        }
    }

    private void GainMoney(EventChoice choice)
    {
        bool win = Random.value > 0.5f;

        int baseMoney = Mathf.Max(inventory.money, 1);
        float percent = Random.Range(0.5f, 1f);
        int amount = Mathf.RoundToInt(baseMoney * percent);

        if (win)
        {
            inventory.money += amount;
            Debug.Log("WIN");

            
            dialogueIndex++;
            ShowDialogue();
        }
        else
        {
            inventory.money -= amount;
            if (inventory.money < 0)
                inventory.money = 0;

            Debug.Log("LOSE");

            
            if (choice.resultDialogue != null && choice.resultDialogue.Length > 0)
            {
                usingChoiceDialogue = true;
                currentChoiceDialogue = choice.resultDialogue;
                choiceDialogueIndex = 0;

                ShowDialogue();
            }
            else
            {
                EndEvent();
            }
        }
    }
    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);
    }

    private void SelectOwnedBuff(BuffSO buff)
    {
        slots.AddBuff(buff);
        dialogueIndex++;
        nextButton.gameObject.SetActive(true);
        ShowDialogue();
    }

    private void EndEvent()
    {
        panel.SetActive(false);
        nextButton.gameObject.SetActive(true);
    }
}
