using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Seed Settings")]
    public bool useCustomSeed = false;
    public int seed = 0;

    [Header("Rooms")]
    public List<RoomData> roomPrefabs;

    private RoomData currentRoom;
    private RoomData nextRoom;
    private RoomData bufferRoom;

    private RoomData prevRoom1;
    private RoomData prevRoom2;
    private RoomData prevRoom3;

    private float lastRotation = 0f;

    void Start()
    {
        if (useCustomSeed) Random.InitState(seed);

        else if (PlayerPrefs.HasKey("seed"))
        {
            seed = PlayerPrefs.GetInt("seed");
            Random.InitState(seed);
        }

        else seed = Random.Range(0, 999999);


        // 1. Создаём первую комнату
        currentRoom = Instantiate(roomPrefabs[0], Vector3.zero, Quaternion.identity);
        lastRotation = currentRoom.transform.rotation.eulerAngles.y;

        // 2. Создаём следующую
        nextRoom = SpawnRoomAfter(currentRoom);

        // 3. Создаём буферную
        bufferRoom = SpawnRoomAfter(nextRoom);

        // Подписываемся на дверь
        currentRoom.GetComponentInChildren<Door>().onDoorOpened += OnDoorOpened;
    }

    void OnDoorOpened()
    {
        // Сдвигаем историю назад
        if (prevRoom3 != null)
            Destroy(prevRoom3.gameObject);

        prevRoom3 = prevRoom2;
        prevRoom2 = prevRoom1;
        prevRoom1 = currentRoom;

        // Сдвигаем вперёд
        currentRoom = nextRoom;
        nextRoom = bufferRoom;

        // Создаём новую буферную
        bufferRoom = SpawnRoomAfter(nextRoom);

        // Подписываемся на дверь новой текущей комнаты
        currentRoom.GetComponentInChildren<Door>().onDoorOpened += OnDoorOpened;
    }

    RoomData SpawnRoomAfter(RoomData previous)
    {
        RoomData prefab = ChooseRoom();
        RoomData newRoom = Instantiate(prefab);

        // Поворот
        float newYRotation = lastRotation + prefab.rotationOffset;
        newRoom.transform.rotation = Quaternion.Euler(0, newYRotation, 0);

        // Позиция
        Vector3 offset = previous.exitPoint.position - newRoom.entrancePoint.position;
        newRoom.transform.position += offset;

        lastRotation = newYRotation;

        // События
        foreach (var e in newRoom.events)
            e.ExecuteEvent(newRoom);

        return newRoom;
    }

    RoomData ChooseRoom()
    {
        int totalWeight = 0;

        foreach (var room in roomPrefabs)
            totalWeight += room.spawnChance;

        int roll = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var room in roomPrefabs)
        {
            current += room.spawnChance;
            if (roll < current)
                return room;
        }

        return roomPrefabs[0];
    }
}
