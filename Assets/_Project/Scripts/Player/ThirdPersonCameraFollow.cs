using UnityEngine;

namespace Cubergy.Player
{
    public sealed class ThirdPersonRmbCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _player;
        [SerializeField] private PlayerFormController _forms;

        [SerializeField] private float _positionSmoothTime = 0.12f;

        [SerializeField] private float _mouseSensitivity = 2.2f;
        [SerializeField] private float _minPitch = -10f;
        [SerializeField] private float _maxPitch = 35f;

        [SerializeField] private float _lookAtHeight = 1.0f;

        [SerializeField] private float _form2DistanceMultiplier = 2.2f;
        [SerializeField] private float _form2HeightMultiplier = 2.2f;

        private Vector3 _velocity;
        private Vector3 _cameraVelocity;

        private float _yaw;
        private float _pitch;

        private bool _isRotating;

        private Camera _cam;
        private Vector3 _cameraBaseLocalOffset;

        private void Awake()
        {
            _yaw = transform.eulerAngles.y;
            _pitch = transform.eulerAngles.x;

            _cam = Camera.main;
            CacheCameraBaseOffset();
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

            PlayerForm form = _forms != null ? _forms.CurrentForm : PlayerForm.Form0;

            if (_player != null && form == PlayerForm.Form0)
                _player.rotation = Quaternion.Euler(0f, _yaw, 0f);
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            if (_cam == null)
            {
                _cam = Camera.main;
                CacheCameraBaseOffset();
                if (_cam == null)
                    return;
            }

            transform.position = Vector3.SmoothDamp(transform.position, _target.position, ref _velocity, _positionSmoothTime);

            PlayerForm form = _forms != null ? _forms.CurrentForm : PlayerForm.Form0;

            float distMul = form == PlayerForm.Form2 ? _form2DistanceMultiplier : 1f;
            float heightMul = form == PlayerForm.Form2 ? _form2HeightMultiplier : 1f;

            Vector3 localOffset = _cameraBaseLocalOffset;
            localOffset.z *= distMul;
            localOffset.y *= heightMul;

            Vector3 desiredCamPos = transform.position + transform.rotation * localOffset;
            _cam.transform.position = Vector3.SmoothDamp(_cam.transform.position, desiredCamPos, ref _cameraVelocity, _positionSmoothTime);

            Vector3 lookAt = transform.position + new Vector3(0f, _lookAtHeight * heightMul, 0f);
            _cam.transform.LookAt(lookAt);
        }

        private void CacheCameraBaseOffset()
        {
            if (_cam == null)
                return;

            Vector3 worldOffset = _cam.transform.position - transform.position;
            _cameraBaseLocalOffset = Quaternion.Inverse(transform.rotation) * worldOffset;
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
