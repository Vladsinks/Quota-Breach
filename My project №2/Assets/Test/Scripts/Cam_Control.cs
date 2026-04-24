using UnityEngine;

public class Cam_Control : MonoBehaviour
{
    [Header("Wave Settings")]
    public float pulseSpeed = 20f;          // скорость расширения
    public float maxRadius = 50f;           // максимальный радиус
    public float highlightDuration = 1.5f;  // 1–2 секунды

    [Header("Wave Visual")]
    public Transform waveSphere;            // ссылка на сферу (модель волны), можно пусто

    private bool isPulsing = false;
    private float currentRadius = 0f;

    public float speed = 10.0f;
    public float sensitivity = 2.0f;


    void Start()
    {
        // Инициализируем глобальные параметры
        Shader.SetGlobalFloat("_PulseRadius", 0f);
        Shader.SetGlobalFloat("_PulseSpeed", pulseSpeed);
        Shader.SetGlobalFloat("_HighlightDuration", highlightDuration);
    }

    void Update()
    {       
        float moveX = Input.GetAxis("Horizontal"); // A, D
        float moveZ = Input.GetAxis("Vertical");   // W, S
        
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move * speed * Time.deltaTime;

        // 2. Поворот мышью (при зажатой правой кнопке или постоянно)
        
        float rotX = Input.GetAxis("Mouse X") * sensitivity;
        float rotY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up, rotX, Space.World);
        transform.Rotate(Vector3.left, rotY);


        // ПКМ запускает волну
        if (Input.GetMouseButtonDown(1))
        {
            StartPulse();
        }

        if (isPulsing)
        {
            currentRadius += pulseSpeed * Time.deltaTime;
            Shader.SetGlobalFloat("_PulseRadius", currentRadius);

            // Обновляем позицию центра волны
            Vector3 origin = transform.position;
            Shader.SetGlobalVector("_PulseOrigin", origin);

            // Обновляем визуальную сферу, если есть
            if (waveSphere != null)
            {
                waveSphere.position = origin;
                float scale = currentRadius * 2f; // диаметр
                waveSphere.localScale = new Vector3(scale, scale, scale);
            }

            if (currentRadius >= maxRadius)
            {
                isPulsing = false;
                // Можно обнулить радиус или оставить — подсветка сама затухнет
            }
        }

        Shader.SetGlobalFloat("_PulseRadius", currentRadius);
        Shader.SetGlobalVector("_PulseOrigin", transform.position);
        Shader.SetGlobalFloat("_PulseSpeed", pulseSpeed);
        Shader.SetGlobalFloat("_HighlightDuration", highlightDuration);

        
    }

    void StartPulse()
    {
        isPulsing = true;
        currentRadius = 0f;
        Shader.SetGlobalFloat("_PulseRadius", currentRadius);
        Shader.SetGlobalFloat("_PulseSpeed", pulseSpeed);
        Shader.SetGlobalFloat("_HighlightDuration", highlightDuration);
        Shader.SetGlobalVector("_PulseOrigin", transform.position);
    }
}

