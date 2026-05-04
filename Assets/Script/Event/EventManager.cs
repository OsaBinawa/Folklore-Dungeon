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

    [Header("Runtime")]
    [SerializeField] private int IncreaseHp;
    [SerializeField] private int IncreaseAtk;
    [SerializeField] private int IncreaseSpd;
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

        List<EventChoice> validChoices = new List<EventChoice>();

        foreach (var choice in eventInstance.choices)
        {
            if (choice.triggerIndex == index)
                validChoices.Add(choice);
        }

        
        if (eventInstance.randomizeChoices)
        {
            for (int i = 0; i < validChoices.Count; i++)
            {
                int rand = Random.Range(i, validChoices.Count);
                (validChoices[i], validChoices[rand]) = (validChoices[rand], validChoices[i]);
            }

            validChoices = validChoices.GetRange(0, Mathf.Min(2, validChoices.Count));
        }

        
        foreach (var choice in validChoices)
        {
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
            case EventEffectType.FreeShopItem:
                GiveFreeShopItem();
                return;
            case EventEffectType.TheFortuneCookies:
                TheFortuneCookies(choice);
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

    private void TheFortuneCookies(EventChoice choice)
    {
        bool win = Random.value > 0.5f;

        var player = RunManager.Instance.Player;

        if (win)
        {
            player.IncreaseAttack(IncreaseAtk);

            dialogueIndex++;
            ShowDialogue();
        }
        else
        {
            
            if (slots.OwnedBuffs.Count > 0)
            {
                int index = Random.Range(0, slots.OwnedBuffs.Count);
                BuffSO removedBuff = slots.OwnedBuffs[index];

                slots.RemoveBuff(removedBuff);

                Debug.Log("LOSE: Removed " + removedBuff.name);
            }
            else
            {
                Debug.Log("LOSE: No buffs to remove");
            }

            
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

    private void GiveFreeShopItem()
    {
        inventory.freeShopItemCount++;

        Debug.Log("Gained 1 free shop item!");

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
            Debug.Log("No available buffs.");
            EndEvent();
            return;
        }

        if (slots.OwnedBuffs.Count == 0)
        {
            Debug.Log("No buffs to reroll.");
            EndEvent();
            return;
        }

        List<BuffSO> newBuffs = new List<BuffSO>();

        foreach (var buff in slots.OwnedBuffs)
        {
            if (!buff.removeAble)
            {
               
                newBuffs.Add(buff);
                continue;
            }

            
            BuffSO randomBuff = allBuffs[Random.Range(0, allBuffs.Count)];
            newBuffs.Add(randomBuff);
        }

        slots.ClearBuffs();

        foreach (var buff in newBuffs)
        {
            slots.AddBuff(buff);
        }

        Debug.Log("Rerolled removable buffs only.");

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
