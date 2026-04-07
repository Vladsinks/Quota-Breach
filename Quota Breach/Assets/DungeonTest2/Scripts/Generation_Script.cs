using UnityEngine;

public class Generation_Script : MonoBehaviour
{
    public GameObject _roomPrefab;
    public Transform spawnPoint, _redTester, _count, _pinkTester, zero, _greenTester; 
    private bool tester;
    public float _roomSize; //Расстояние между комнатами
    private int i;
    private float timer, delay = 2f;

    void Start()
    {
        i = 0;

        //Спавним зелёные тестеры вокруг комнаты
        Instantiate(_greenTester, _pinkTester.position + new Vector3(0f,0f,_roomSize), spawnPoint.rotation);
        Instantiate(_greenTester, _pinkTester.position + new Vector3(_roomSize,0f,0f), spawnPoint.rotation);
        Instantiate(_greenTester, _pinkTester.position + new Vector3(0f,0f,-_roomSize), spawnPoint.rotation);
        Instantiate(_greenTester, _pinkTester.position + new Vector3(-_roomSize,0f,0f), spawnPoint.rotation);
    }


    void Update()
    {
        timer += Time.deltaTime; //таймер
        if (timer >= delay) 
        {
            RedTester(); 
            timer = 0f;
        }
        RedTester(); 

        if(_count.GetComponent<Count>().i < _count.GetComponent<Count>()._roomCount){ //если построено комнат меньше чем надо
            tester = _redTester.GetComponent<Red_Tester>().CanRoomSpawn; //берём переменную из другого скрипта

            if (tester) //если есть место для комнаты - спавним
            {
                SpawnRoom();
                tester = false;
            }     
        }

        if(_count.GetComponent<Count>().i >= _count.GetComponent<Count>()._roomCount) //если уже построили достаточно комнат
        {         
            Destroy(_redTester.gameObject); //удалить красного
            Destroy(this); //удалить этот скрипт
        }
    }

    void SpawnRoom()
    {
        _redTester.position = _pinkTester.position + new Vector3(0f,10f,0f); //красный уходит вверх что бы не мешать
        Instantiate(_roomPrefab, spawnPoint.position, spawnPoint.rotation); //спавн комнаты
        _count.GetComponent<Count>().i += 1; //говорим что создали 1 комнату

        //Спавним зелёные тестеры вокруг комнаты
        Instantiate(_greenTester, _pinkTester.position + new Vector3(0f,0f,_roomSize), spawnPoint.rotation);
        Instantiate(_greenTester, _pinkTester.position + new Vector3(_roomSize,0f,0f), spawnPoint.rotation);
        Instantiate(_greenTester, _pinkTester.position + new Vector3(0f,0f,-_roomSize), spawnPoint.rotation);
        Instantiate(_greenTester, _pinkTester.position + new Vector3(-_roomSize,0f,0f), spawnPoint.rotation);


        Destroy(_redTester.gameObject); //уничтожение красного
        Destroy(this); //уничтожение этого скрипта
    }

    void RedTester()
    {
        int randomValue = Random.Range(0, 4);
        
        switch (randomValue)
        {
            case 0:
                _redTester.position = _pinkTester.position + new Vector3(0f,0f,_roomSize);
                spawnPoint.position = zero.position + new Vector3(0f,0f,_roomSize);
                Debug.Log("1");
                break;

            case 1:
                _redTester.position = _pinkTester.position + new Vector3(_roomSize,0f,0f);
                spawnPoint.position = zero.position + new Vector3(_roomSize,0f,0f);
                Debug.Log("2");
                break;

            case 2:
                _redTester.position = _pinkTester.position + new Vector3(0f,0f,-_roomSize);
                spawnPoint.position = zero.position + new Vector3(0f,0f,-_roomSize);
                Debug.Log("3");
                break;

            case 3:
                _redTester.position = _pinkTester.position + new Vector3(-_roomSize,0f,0f);
                spawnPoint.position = zero.position + new Vector3(-_roomSize,0f,0f);
                Debug.Log("4");
                break;

            default:
                Debug.Log("Что то пошло не так");
                break;
        }
    }
}
