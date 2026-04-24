using UnityEngine;

public class Controller67 : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float sensitivity = 2.0f;
    [SerializeField] private Transform Wave, cam;

    void Start()
    {
        
    }

    // Update is called once per frame
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

        if (Input.GetMouseButtonDown(1))
        {
            Instantiate(Wave, cam.position , Quaternion.identity);
        }
    }

}
