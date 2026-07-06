using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    [RequireComponent(typeof(CharacterController))]
    public class FpsPlayerController : MonoBehaviour
    {
        [Header("Камера")]
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private float _mouseSensitivity = 0.15f;
        [SerializeField] private float _minPitch = -85f;
        [SerializeField] private float _maxPitch = 85f;

        [Header("Движение")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravity = -18f;

        private CharacterController _controller;
        private float _pitch;
        private Vector3 _verticalVelocity;

        public void SetCameraPivot(Transform pivot) => _cameraPivot = pivot;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Look();
            Move();
        }

        private void Look()
        {
            Vector2 delta = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

            transform.Rotate(Vector3.up * delta.x * _mouseSensitivity);

            _pitch = Mathf.Clamp(_pitch - delta.y * _mouseSensitivity, _minPitch, _maxPitch);
            _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void Move()
        {
            Keyboard keyboard = Keyboard.current;
            Vector2 input = keyboard == null
                ? Vector2.zero
                : new Vector2(
                    (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
                    (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f));

            bool sprinting = keyboard != null && keyboard.leftShiftKey.isPressed;
            float speed = sprinting ? _sprintSpeed : _walkSpeed;

            Vector3 move = (transform.right * input.x + transform.forward * input.y).normalized * speed;

            if (_controller.isGrounded)
            {
                _verticalVelocity.y = -1f;

                if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
                {
                    _verticalVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                }
            }
            else
            {
                _verticalVelocity.y += _gravity * Time.deltaTime;
            }

            _controller.Move((move + _verticalVelocity) * Time.deltaTime);
        }
    }
}
