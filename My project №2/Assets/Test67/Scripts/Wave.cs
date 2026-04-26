using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour
{
    [SerializeField] private float Speed = 0.1f;
    [SerializeField] private float MaxTime = 50f;

    float tim = 0f;
    
    void Start()
    {
        tim = 0f;
    }

    void Update()
    {
        if (tim < MaxTime)
        {
            transform.localScale += new Vector3(Speed, Speed, Speed);
            tim++;
        }
        else  Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<ChangeColor>() == null)
        {
            collision.gameObject.AddComponent<ChangeColor>();
        }
    }   
}
