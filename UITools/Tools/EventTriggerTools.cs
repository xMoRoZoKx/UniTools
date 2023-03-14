using System;
namespace UnityEngine.EventSystems
{
    public static class EventTriggerTools
    {
        public static void AddEvent(this EventTrigger trigger, EventTriggerType type, Action<BaseEventData> callBack)
        {
            var trig = trigger.triggers.Find(t => t.eventID == type);
            if (trig == null)
            {
                trig = new EventTrigger.Entry() { eventID = type, callback = new EventTrigger.TriggerEvent() };
                trigger.triggers.Add(trig);
            }
            trig.callback.AddListener((eventData) => callBack?.Invoke(eventData));
        }
        public static void ClearAllEvents(this EventTrigger trigger)
        {
            trigger.triggers.ForEach(t => t.callback.RemoveAllListeners());
        }
        public static void ClearEvents(this EventTrigger trigger, EventTriggerType type)
        {
            trigger.triggers.Find(t => t.eventID == type)?.callback.RemoveAllListeners();
        }
    }
}
