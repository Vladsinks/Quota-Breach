using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour
{
    [SerializeField] private float Speed = 0.1f;
    [SerializeField] private float MaxTime = 50f;

    [SerializeField] private Color Wall;
    [SerializeField] private Color Scrap;
    [SerializeField] private Color Enemy;
    float tim = 0f;
    Renderer _renderer;
    
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
        else  Destroy(this.gameObject, 2.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //StartCoroutine(ChangeColorRoutine());
        _renderer = other.GetComponent<Renderer>();

        if (other.CompareTag("Wall")) 
        {
            StartCoroutine(ChangeColorRoutine(Wall));
        }
        else if (other.CompareTag("Scrap")) 
        {
            StartCoroutine(ChangeColorRoutine(Scrap));
        }
        else if (other.CompareTag("Enemy")) 
        {
            StartCoroutine(ChangeColorRoutine(Enemy));
        }
    }

    private IEnumerator ChangeColorRoutine(Color color)
    {
        _renderer.material.color = color;
        yield return new WaitForSeconds(1f);
        _renderer.material.color = Color.black;
    }

    
}
