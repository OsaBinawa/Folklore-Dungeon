using UnityEngine;

[CreateAssetMenu(menuName = "Event/Event Data")]
public class EventData : ScriptableObject
{
    public string Name;
    public EventType Type;
    public string[] Dialogue;
    public EventEffect Effect;
    public EventChoice[] Choices;
}

[System.Serializable]
public class EventChoice
{
    public string text;
    public BuffSO buff;
    public EventEffectType effect;
    public int triggerIndex;
    public string[] resultDialogue;
    
}