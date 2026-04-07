using UnityEngine;

public class Green_Tester : MonoBehaviour
{
    void OnTriggerEnter(Collider other) 
{
    if (other.CompareTag("Pink")) 
    {
        
        Destroy(this.gameObject); 
    }
}
}
