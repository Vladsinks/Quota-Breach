using UnityEngine;
using TMPro;

public class Count : MonoBehaviour
{
    public TextMeshPro _countText; 
    float Value;

    void Start()
    {
        Value = 0f;
        _countText.text = "Заработано:" + Value.ToString("F0");
        _countText.color = Color.green;
    }

    void Update()
    {
        
    }

        private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Can Pick Up"))
        {
            Value += other.GetComponent<ObjectScript>().Value;
            _countText.text = "Заработано:" + Value.ToString("F0");
            Destroy(other.gameObject);
        }
    }
}
