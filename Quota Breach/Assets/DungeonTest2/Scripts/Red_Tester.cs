using UnityEngine;

public class Red_Tester : MonoBehaviour
{
    public bool CanRoomSpawn;

    void OnTriggerEnter(Collider other) 
{
    if (other.CompareTag("Green")) 
    {
        CanRoomSpawn = true;
        Destroy(other.gameObject); 
    }

    else if (other.CompareTag("Pink"))
    {
         CanRoomSpawn = false;   
    }
}
}
