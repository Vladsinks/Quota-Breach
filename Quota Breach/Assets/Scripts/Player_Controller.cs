using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class Player_Controller : MonoBehaviour
{
    public float currentSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float staminaSpeed;
    public float stamina;
    public float maxStamina;
    public bool IsPlayerRun;
    
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CinemachineCamera _cimCam;

    private Vector2 _move;

    public void OnMove(InputValue val)
    {
        _move = val.Get<Vector2>();
    }

    public void OnSprint(InputValue val)
    {
        if(val.Get<float>() > 0.5f)
        {
                if(stamina > 0)
            {
                currentSpeed = sprintSpeed;
                IsPlayerRun = true;
            }
            else
            {
                currentSpeed = walkSpeed;
                IsPlayerRun = false;
            }
        }
        else
        {
            currentSpeed = walkSpeed;
            IsPlayerRun = false;
        }
    }

    private void Start()
    {
        currentSpeed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _characterController.Move((GetForward() * _move.y + GetRight() * _move.x) * Time.deltaTime * currentSpeed);

        if(IsPlayerRun)
        {
            stamina -= staminaSpeed;
            Debug.Log(stamina);
        }
        else
        {
            if(stamina < maxStamina)
            {
                stamina += staminaSpeed;
            }
        }

        if(stamina <= 0)
        {
            currentSpeed = walkSpeed;
        }
    }

    private Vector3 GetForward()
    {
        Vector3 forward = _cimCam.transform.forward;
        forward.y = 0;

        return forward.normalized;
    }

    private Vector3 GetRight()
    {
        Vector3 right = _cimCam.transform.right;
        right.y = 0;

        return right.normalized;
    }
  

}
