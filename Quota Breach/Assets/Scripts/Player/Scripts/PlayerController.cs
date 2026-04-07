using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float crouchSpeed = 1.2f;
    public float gravity = -9.81f;

    [Header("Crouch")]
    public float crouchHeight = 1.0f;
    public float standHeight = 1.8f;
    public float crouchSmooth = 0.15f;

    [Header("Camera")]
    public Transform cameraPivot;
    public float crouchCameraOffset = -0.5f;
    public float headBobAmount = 0.02f;
    public float headBobSpeed = 8f;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public LayerMask interactMask;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode throwKey = KeyCode.Mouse1;
    public InteractionUI interactionUI;

    [Header("Hand")]
    public Transform handPoint;

    [Header("Animator")]
    public Animator animator;

    private CharacterController controller;
    private float verticalVelocity;
    private bool isCrouching = false;
    private float defaultCameraY;
    private float headBobTimer = 0f;

    private PickableItem currentLookItem;
    private PickableItem heldItem;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        defaultCameraY = cameraPivot.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        HandleCrouch();
        HandleHeadBob();
        HandleRaycast();
        HandleInput();
    }

    // ---------------- MOVEMENT ----------------

    private void HandleMovement()
    {
        float speed = walkSpeed;

        bool moving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;

        bool running = Input.GetKey(KeyCode.LeftShift) && !isCrouching && moving;
        if (running) speed = runSpeed;

        if (isCrouching) speed = crouchSpeed;

        Vector3 move = transform.right * Input.GetAxis("Horizontal") +
                       transform.forward * Input.GetAxis("Vertical");

        move *= speed;

        if (controller.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        animator.SetFloat("Speed", new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude);
        animator.SetBool("IsRunning", running);
        animator.SetBool("IsGrounded", controller.isGrounded);
    }

    // ---------------- CROUCH ----------------

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isCrouching = !isCrouching;

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        float targetCameraY = isCrouching ? defaultCameraY + crouchCameraOffset : defaultCameraY;

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * (1f / crouchSmooth));

        Vector3 camPos = cameraPivot.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * (1f / crouchSmooth));
        cameraPivot.localPosition = camPos;

        animator.SetBool("IsCrouching", isCrouching);
    }

    // ---------------- HEAD BOB ----------------

    private void HandleHeadBob()
    {
        bool moving = controller.velocity.magnitude > 0.1f && controller.isGrounded;

        if (moving)
        {
            headBobTimer += Time.deltaTime * headBobSpeed;
            float bob = Mathf.Sin(headBobTimer) * headBobAmount;

            Vector3 pos = cameraPivot.localPosition;
            pos.y = defaultCameraY + bob + (isCrouching ? crouchCameraOffset : 0);
            cameraPivot.localPosition = pos;
        }
        else
        {
            headBobTimer = 0f;

            Vector3 pos = cameraPivot.localPosition;
            pos.y = Mathf.Lerp(pos.y, defaultCameraY + (isCrouching ? crouchCameraOffset : 0), Time.deltaTime * 10f);
            cameraPivot.localPosition = pos;
        }
    }

    // ---------------- INTERACTION ----------------

    private void HandleRaycast()
    {
        if (currentLookItem != null)
        {
            currentLookItem.SetHighlight(false);
            currentLookItem = null;
        }

        interactionUI.HidePrompt();

        Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
        {
            PickableItem item = hit.collider.GetComponentInParent<PickableItem>();

            if (item != null)
            {
                currentLookItem = item;
                currentLookItem.SetHighlight(true);

                if (heldItem == null)
                    interactionUI.ShowPrompt("Press E to pick up");
                else
                    interactionUI.ShowPrompt("Slot is full");
            }
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (currentLookItem != null && heldItem == null)
            {
                PickUp(currentLookItem);
                interactionUI.HidePrompt();
            }
        }

        if (Input.GetKeyDown(throwKey))
        {
            if (heldItem != null)
                ThrowItem();
        }
    }

    private void PickUp(PickableItem item)
    {
        heldItem = item;
        heldItem.OnPickedUp(handPoint);
        animator.SetTrigger("DoPickUp");
    }

    private void ThrowItem()
    {
        Vector3 pos = handPoint.position;
        Vector3 dir = cameraPivot.forward;

        heldItem.OnThrown(pos, dir);
        heldItem = null;
    }
}
