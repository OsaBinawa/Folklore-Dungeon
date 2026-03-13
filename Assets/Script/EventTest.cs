using UnityEngine;

public class EventTest : MonoBehaviour
{
    [SerializeField] private EventData[] eventData;
    [SerializeField] private int EventIndex = 0;
    private Event eventInstance;

    private void Awake()
    {
        eventInstance = new Event(eventData[EventIndex]);

        Debug.Log("Event Name = " + eventInstance.eventName);

        Debug.Log("Event Type = " + eventInstance.type.ToString());

        foreach (var x in eventInstance.dialogue)
        {
            Debug.Log(x.ToString());
        }
    }
}
