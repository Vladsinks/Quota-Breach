using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class Player_Controller : MonoBehaviour
{
    public float characterSpeed;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CinemachineCamera _cimCam;

    private Vector2 _move;

    public void OnMove(InputValue val)
    {
        _move = val.Get<Vector2>();
    }

    private void Update()
    {
        _characterController.Move((GetForward() * _move.y + GetRight() * _move.x) * Time.deltaTime * characterSpeed);
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
