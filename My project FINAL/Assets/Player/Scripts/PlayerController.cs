using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float airControl = 0.5f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField] public float mouseSensitivity = 50f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private float cameraTiltAmount = 5f;
    [SerializeField] private float cameraTiltSpeed = 8f;

    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Vector3 currentVelocity;
    private float verticalRotation;
    private float currentCameraTilt;
    private bool isGrounded;
    private float verticalVelocity;
    private float targetTilt;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLookRotation();
        HandleCameraTilt();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump") && isGrounded;

        Vector3 targetDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
        Vector3 targetVelocity = targetDirection * moveSpeed;

        float currentAcceleration = isGrounded ? acceleration : acceleration * airControl;
        if (targetDirection.magnitude > 0.1f)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, currentAcceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        if (jumpPressed)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        Vector3 moveVelocity = currentVelocity;
        moveVelocity.y = verticalVelocity;
        controller.Move(moveVelocity * Time.deltaTime);
    }

    private void HandleLookRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity / 25 ;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity / 25 ;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        cameraHolder.localEulerAngles = Vector3.right * verticalRotation;
    }

    private void HandleCameraTilt()
    {
        if (Input.GetKey(KeyCode.A)) 
        {
            targetTilt = cameraTiltAmount;
        }
        else if (Input.GetKey(KeyCode.D)) 
        {
            targetTilt = -cameraTiltAmount;
        }
        else 
        {
            targetTilt = 0f;
        }

        currentCameraTilt = Mathf.Lerp(currentCameraTilt, targetTilt, cameraTiltSpeed * Time.deltaTime);
        cameraTransform.localEulerAngles = new Vector3(cameraTransform.localEulerAngles.x, 0, currentCameraTilt);
    }

    private void ApplyGravity()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}