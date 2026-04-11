using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 5;
        [SerializeField] private float _runSpeed = 8;
        [SerializeField] private float _rotateSpeed = 75;
        [SerializeField] private float _jumpForce = 5;
        [SerializeField] private float _gravity = -9.81f;

        [Header("Pick Up")]
        [SerializeField] private float _pickUpDistance = 5;
        [SerializeField] private LayerMask _canPickUpLayer;

        [Header("Hold Settings")]
        [SerializeField] private float _holdDistance = 3f;
        [SerializeField] private float _springStrength = 120f;   // мягкая пружина
        [SerializeField] private float _springDamping = 15f;     // сглаживание
        [SerializeField] private float _maxHoldDistance = 3f;
        [SerializeField] private float _maxObjectSpeed = 6f;

        [Header("Beam Power")]
        [SerializeField] private float _beamPower = 1f;
        [SerializeField] private float _beamPowerMultiplier = 1f;
        [SerializeField] private float _maxBeamPower = 10f;

        [Header("Mass Handling")]
        [SerializeField] private float _baseMaxLiftMass = 8f;
        [SerializeField] private float _massHardLimit = 25f;

        [Header("UI")]
        [SerializeField] private GameObject _hoverIndicator;
        [SerializeField] private GameObject _normalIndicator;

        private CharacterController _characterController;
        private Camera _playerCamera;
        private Collider _playerCollider;

        private Vector3 _velocity;
        private Vector2 _rotation;
        private Vector2 _direction;

        private Rigidbody _currentObject;
        private Collider _currentCollider;

        private Vector3 _lastPos;
        private Vector3 _trackedVelocity;
        private readonly Queue<Vector3> _velBuffer = new();
        [SerializeField] private int _velocityBufferSize = 5;

        private bool _isRotating = false;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _playerCamera = GetComponentInChildren<Camera>();
            _playerCollider = GetComponent<Collider>();

            if (_hoverIndicator != null)
                _hoverIndicator.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            HandleMovement();
            HandleLook();

            if (Input.GetMouseButtonDown(0)) TryPickUp();
            if (Input.GetMouseButtonUp(0)) Drop();

            if (Input.GetMouseButtonDown(1)) _isRotating = true;
            if (Input.GetMouseButtonUp(1)) _isRotating = false;

            UpdateHoverIndicator();
            TrackVelocity();
            AutoDropCheck();
        }

        private void FixedUpdate()
        {
            if (_currentObject != null)
            {
                if (_isRotating)
                    RotateHeldObject();
                else
                    HoldObjectSpring();
            }
        }

        // ───────────────────────────────────────────────────────────────
        // MOVEMENT
        // ───────────────────────────────────────────────────────────────

        private void HandleMovement()
        {
            _direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (_characterController.isGrounded)
                _velocity.y = Input.GetKeyDown(KeyCode.Space) ? _jumpForce : -0.1f;
            else
                _velocity.y += _gravity * Time.deltaTime;

            float speed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed;

            Vector3 move = Quaternion.Euler(0, _playerCamera.transform.eulerAngles.y, 0)
                * new Vector3(_direction.x * speed, 0, _direction.y * speed);

            _velocity = new Vector3(move.x, _velocity.y, move.z);

            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mouse *= _rotateSpeed * Time.deltaTime;

            _rotation.y += mouse.x;
            _rotation.x = Mathf.Clamp(_rotation.x - mouse.y, -90, 90);

            _playerCamera.transform.localEulerAngles = _rotation;
        }

        // ───────────────────────────────────────────────────────────────
        // UI
        // ───────────────────────────────────────────────────────────────

        private void UpdateHoverIndicator()
        {
            if (_hoverIndicator == null) return;

            if (_currentObject != null)
            {
                _hoverIndicator.SetActive(false);
                _normalIndicator.SetActive(true);
                return;
            }

            bool hit = Physics.Raycast(
                _playerCamera.transform.position,
                _playerCamera.transform.forward,
                _pickUpDistance,
                _canPickUpLayer);

            _hoverIndicator.SetActive(hit);
            _normalIndicator.SetActive(!hit);
        }

        // ───────────────────────────────────────────────────────────────
        // PICK UP WITH MASS CHECK
        // ───────────────────────────────────────────────────────────────

        private void TryPickUp()
        {
            if (_currentObject != null) return;

            if (!Physics.Raycast(
                _playerCamera.transform.position,
                _playerCamera.transform.forward,
                out RaycastHit hit,
                _pickUpDistance,
                _canPickUpLayer)) return;

            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb == null) return;

            if (rb.mass > _massHardLimit)
                return;

            float dynamicLimit = _baseMaxLiftMass * _beamPower;
            if (rb.mass > dynamicLimit)
                return;

            _currentObject = rb;
            _currentCollider = rb.GetComponent<Collider>();

            Physics.IgnoreCollision(_playerCollider, _currentCollider, true);

            _lastPos = rb.position;
            _velBuffer.Clear();
        }

        private void Drop()
        {
            if (_currentObject == null) return;

            Physics.IgnoreCollision(_playerCollider, _currentCollider, false);

            _currentObject.linearVelocity = _trackedVelocity;

            _currentObject = null;
            _velBuffer.Clear();
        }

        // ───────────────────────────────────────────────────────────────
        // SOFT SPRING HOLDING (stable, smooth)
        // ───────────────────────────────────────────────────────────────

        private void HoldObjectSpring()
        {
            float power = _beamPower * _beamPowerMultiplier;

            float massFactor = Mathf.Clamp01(1f / (_currentObject.mass * 0.25f));

            Vector3 holdPoint = _playerCamera.transform.position + _playerCamera.transform.forward * _holdDistance;
            Vector3 toTarget = holdPoint - _currentObject.position;

            Vector3 springForce =
                toTarget * (_springStrength * power * massFactor)
                - _currentObject.linearVelocity * _springDamping;

            _currentObject.AddForce(springForce, ForceMode.Acceleration);

            _currentObject.linearVelocity = Vector3.ClampMagnitude(
                _currentObject.linearVelocity,
                _maxObjectSpeed * power
            );
        }

        // ───────────────────────────────────────────────────────────────
        // ROTATION MODE (ПКМ)
        // ───────────────────────────────────────────────────────────────

        private void RotateHeldObject()
        {
            float rotX = Input.GetAxis("Mouse X") * 5f;
            float rotY = -Input.GetAxis("Mouse Y") * 5f;

            _currentObject.angularVelocity = Vector3.zero;

            _currentObject.transform.Rotate(_playerCamera.transform.up, rotX, Space.World);
            _currentObject.transform.Rotate(_playerCamera.transform.right, rotY, Space.World);
        }

        // ───────────────────────────────────────────────────────────────
        // VELOCITY TRACKING
        // ───────────────────────────────────────────────────────────────

        private void TrackVelocity()
        {
            if (_currentObject == null) return;

            Vector3 frameVel = (_currentObject.position - _lastPos) / Time.deltaTime;
            _lastPos = _currentObject.position;

            _velBuffer.Enqueue(frameVel);
            if (_velBuffer.Count > _velocityBufferSize)
                _velBuffer.Dequeue();

            Vector3 sum = Vector3.zero;
            foreach (var v in _velBuffer) sum += v;
            _trackedVelocity = sum / _velBuffer.Count;
        }

        // ───────────────────────────────────────────────────────────────
        // AUTO DROP
        // ───────────────────────────────────────────────────────────────

        private void AutoDropCheck()
        {
            if (_currentObject == null) return;

            Vector3 holdPoint = _playerCamera.transform.position + _playerCamera.transform.forward * _holdDistance;

            if (Vector3.Distance(_currentObject.position, holdPoint) > _maxHoldDistance)
                Drop();
        }

        // ───────────────────────────────────────────────────────────────
        // BEAM UPGRADE
        // ───────────────────────────────────────────────────────────────

        public void UpgradeBeamPower(float amount)
        {
            _beamPower = Mathf.Clamp(_beamPower + amount, 1f, _maxBeamPower);
        }
    }
}
