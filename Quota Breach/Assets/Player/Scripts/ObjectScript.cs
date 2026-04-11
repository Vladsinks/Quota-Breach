using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    [Range(1f, 1000f)]                
    public float _minValue;

    [Range(1f, 1000f)]                
    public float _maxValue;

    [Range(1f, 10f)]                
    public float Fragility;

    [HideInInspector]
    public float Value;

    void Start()
    {
        Value = Random.Range(_minValue, _maxValue);
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce > 3)
        {
            float loss = impactForce * Fragility;
            Value -= loss;

            if(Value <= 0)
            {
                Destroy(gameObject);
            }


            Debug.Log($"Удар силой {impactForce}. Потеряно: {loss}. Текущая цена: {Value}");
        }
    }
}
