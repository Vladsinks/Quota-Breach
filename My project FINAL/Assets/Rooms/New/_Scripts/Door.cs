using UnityEngine;
using System;

public class Door : MonoBehaviour
{
    public Action onDoorOpened;

    public void OpenDoor()
    {
        onDoorOpened?.Invoke();
    }
}
