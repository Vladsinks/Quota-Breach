using UnityEngine;
using System.Collections.Generic;

public class RoomData : MonoBehaviour
{
    public Transform entrancePoint;
    public Transform exitPoint;

    [Header("Rotation")]
    public float rotationOffset = 0f;

    [Header("Spawn Chance")]
    [Range(0, 100)]
    public int spawnChance = 100;

    [Header("Events")]
    public List<RoomEvent> events;
}
