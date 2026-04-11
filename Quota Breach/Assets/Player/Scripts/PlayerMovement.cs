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
        [SerializeField] private float _throwForce = 6;
        [SerializeField] private LayerMask _canPickUpLayer;

        [Header("Hold Settings")]
        [SerializeField] private float _holdDistance = 3f;
        [SerializeField] private float _pullStrength = 40f;
        [SerializeField] private float _maxHoldDistance = 2.5f;
        [SerializeField] private float _maxObjectSpeed = 10f;

        [Header("UI")]
        [SerializeField] private GameObject _hoverIndicator;
        [SerializeField] private GameObject _normalIndicator;

        private CharacterController _characterController;
        private Camera _playerCamera;

        private Vector3 _velocity;
        private Vector2 _rotation;
        private Vector2 _direction;

        private Rigidbody _currentObject;
        private Collider _currentCollider;
        private Collider _playerCollider;

        private Vector3 _lastPos;
        private Vector3 _trackedVelocity;
        private readonly Queue<Vector3> _velBuffer = new();

        [SerializeField] private int _velocityBufferSize = 5;

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

            if (Input.GetMouseButtonDown(0)) PickUp();
            if (Input.GetMouseButtonUp(0)) Drop();
            if (Input.GetMouseButtonDown(1)) Drop(true);

            UpdateHoverIndicator();
            TrackVelocity();
            AutoDropCheck();
        }

        private void FixedUpdate()
        {
            if (_currentObject != null)
                HoldObjectPhysics();
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
        // PICK UP / DROP
        // ───────────────────────────────────────────────────────────────

        private void PickUp()
        {
            if (_currentObject != null) return;

            if (!Physics.Raycast(
                _playerCamera.transform.position,
                _playerCamera.transform.forward,
                out RaycastHit hit,
                _pickUpDistance,
                _canPickUpLayer)) return;

            _currentObject = hit.collider.GetComponent<Rigidbody>();
            if (_currentObject == null) return;

            _currentCollider = _currentObject.GetComponent<Collider>();

            Physics.IgnoreCollision(_playerCollider, _currentCollider, true);

            _lastPos = _currentObject.position;
            _velBuffer.Clear();
        }

        private void Drop(bool throwObject = false)
        {
            if (_currentObject == null) return;

            Physics.IgnoreCollision(_playerCollider, _currentCollider, false);

            _currentObject.linearVelocity = _trackedVelocity;

            if (throwObject)
                _currentObject.AddForce(_playerCamera.transform.forward * _throwForce, ForceMode.Impulse);

            _currentObject = null;
            _velBuffer.Clear();
        }

        // ───────────────────────────────────────────────────────────────
        // HOLD PHYSICS
        // ───────────────────────────────────────────────────────────────

        private void HoldObjectPhysics()
        {
            Vector3 holdPoint = _playerCamera.transform.position + _playerCamera.transform.forward * _holdDistance;
            Vector3 dir = holdPoint - _currentObject.position;

            _currentObject.AddForce(dir * _pullStrength, ForceMode.Acceleration);

            _currentObject.linearVelocity = Vector3.ClampMagnitude(_currentObject.linearVelocity, _maxObjectSpeed);
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
    }
}
