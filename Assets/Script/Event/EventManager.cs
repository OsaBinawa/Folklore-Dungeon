using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private Slots inventory;
    //[SerializeField] private Slots playerSlots;

    private Event eventInstance;
    private int dialogueIndex = 0;
    public bool useRandom;
    public int eventIndex;

    private void Awake()
    {
        inventory = FindFirstObjectByType<Slots>();
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
        if (dialogueIndex < eventInstance.dialogue.Length)
        {
            dialogueText.text = eventInstance.dialogue[dialogueIndex];
        }
        else
        {
            ShowChoices();
        }
    }

    private void NextDialogue()
    {
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
        // Buff path
        if (choice.buff != null)
        {
            inventory.AddBuff(choice.buff);
            EndEvent();
            return;
        }

        // Effect path
        switch (choice.effect)
        {
            case EventEffectType.GainOwnedBuffChoice:
                ShowOwnedBuffChoices();
                return;
        }

        EndEvent();
    }

    // ===== SPECIAL EFFECT =====

    public void ShowOwnedBuffChoices()
    {
        nextButton.gameObject.SetActive(false);

        // Clear old buttons
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        if (inventory.OwnedBuffs.Count == 0)
        {
            Debug.Log("No buffs owned");
            EndEvent();
            return;
        }

        // Check if there is at least one stackable buff
        bool hasStackable = false;

        foreach (var buff in inventory.OwnedBuffs)
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

        // Spawn only stackable buffs
        foreach (var buff in inventory.OwnedBuffs)
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


    private void SelectOwnedBuff(BuffSO buff)
    {
        inventory.AddBuff(buff);
        EndEvent();
    }

    private void EndEvent()
    {
        panel.SetActive(false);
        nextButton.gameObject.SetActive(true);
    }
}
