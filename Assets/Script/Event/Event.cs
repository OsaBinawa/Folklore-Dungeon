public enum EventType
{
    Fortune, Misfortune, Neutral, Luck
}
public class Event
{
    private readonly EventData eventData;
    public string eventName;
    public EventType type;
    public string[] dialogue;
    public EventEffect effect;

    public EventChoice[] choices;
    public Event(EventData data)
    {
        eventData = data;
        eventName = eventData.Name;
        type = eventData.Type;
        dialogue = eventData.Dialogue;
        effect = eventData.Effect;
        choices = eventData.Choices;
    }
    
}