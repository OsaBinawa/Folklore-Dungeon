using UnityEngine;

[CreateAssetMenu(menuName = "Event/Event Data")]
public class EventData : ScriptableObject
{
    public string Name;
    public EventType Type;
    public string[] Dialogue;
    public EventEffect Effect;
}
