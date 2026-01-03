using UnityEngine;

namespace Cubergy.Player
{
    public sealed class ThirdPersonRmbCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _player;

        [SerializeField] private float _positionSmoothTime = 0.12f;

        [SerializeField] private float _mouseSensitivity = 2.2f;
        [SerializeField] private float _minPitch = -10f;
        [SerializeField] private float _maxPitch = 35f;

        [SerializeField] private float _lookAtHeight = 1.0f;

        private Vector3 _velocity;

        private float _yaw;
        private float _pitch;

        private bool _isRotating;

        private void Awake()
        {
            _yaw = transform.eulerAngles.y;
            _pitch = transform.eulerAngles.x;
        }

        private void Update()
        {
            bool rmb = Input.GetMouseButton(1);

            if (rmb && !_isRotating)
                BeginRotate();
            else if (!rmb && _isRotating)
                EndRotate();

            if (!_isRotating)
                return;

            float mx = Input.GetAxisRaw("Mouse X");
            float my = Input.GetAxisRaw("Mouse Y");

            _yaw += mx * _mouseSensitivity;
            _pitch -= my * _mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            if (_player != null)
                _player.rotation = Quaternion.Euler(0f, _yaw, 0f);
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            transform.position = Vector3.SmoothDamp(transform.position, _target.position, ref _velocity, _positionSmoothTime);

            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 lookAt = _target.position + new Vector3(0f, _lookAtHeight, 0f);
                cam.transform.LookAt(lookAt);
            }
        }

        private void BeginRotate()
        {
            _isRotating = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void EndRotate()
        {
            _isRotating = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
