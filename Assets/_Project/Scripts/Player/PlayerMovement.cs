using UnityEngine;

namespace Cubergy.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private float _moveSpeed = 6f;

        [Header("Gravity")]
        [SerializeField] private float _gravity = -20f;

        private CharacterController _controller;
        private Vector3 _velocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Move();
            ApplyGravity();
        }

        private void Move()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector3 input = (transform.right * x) + (transform.forward * z);
            if (input.sqrMagnitude > 1f)
                input.Normalize();

            Vector3 motion = input * (_moveSpeed * Time.deltaTime);
            _controller.Move(motion);
        }


        private void ApplyGravity()
        {
            if (_controller.isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;

            _velocity.y += _gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}