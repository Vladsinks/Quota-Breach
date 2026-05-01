using UnityEngine;

public abstract class RoomEvent : ScriptableObject
{
    public string eventName;
    public abstract void ExecuteEvent(RoomData room);
}
