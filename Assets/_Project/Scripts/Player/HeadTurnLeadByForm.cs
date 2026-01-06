using UnityEngine;

namespace Cubergy.Player
{
    public sealed class HeadLeadTurnByForm : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private Transform _bodyRoot;
        [SerializeField] private Transform _cameraTransform;

        [Header("Head Pitch")]
        [SerializeField] private float _headPitchLimit = 25f;
        [SerializeField] private float _headPitchFollow = 25f;

        [Header("Form1")]
        [SerializeField] private Transform _form1HeadRoot;

        [Header("Form2")]
        [SerializeField] private Transform _form2HeadRoot;

        [Header("Head Yaw Limits (deg)")]
        [SerializeField] private float _headYawLimit = 60f;

        [Header("Body Follow")]
        [SerializeField] private float _bodyFollowSpeed = 7f;

        [Header("Head Follow")]
        [SerializeField] private float _headFollow = 25f;

        private Transform _head;
        private Quaternion _headBase;

        private void OnEnable()
        {
            if (_forms != null)
                _forms.FormChanged += OnFormChanged;

            Rebind(_forms != null ? _forms.CurrentForm : PlayerForm.Form0);
        }

        private void OnDisable()
        {
            if (_forms != null)
                _forms.FormChanged -= OnFormChanged;
        }

        private void OnFormChanged(PlayerForm form)
        {
            Rebind(form);
        }

        private void Rebind(PlayerForm form)
        {
            if (form == PlayerForm.Form1)
                _head = _form1HeadRoot;
            else if (form == PlayerForm.Form2)
                _head = _form2HeadRoot;
            else
                _head = null;

            if (_head != null)
                _headBase = _head.localRotation;
        }

        private void LateUpdate()
        {
            if (_forms == null || _bodyRoot == null || _cameraTransform == null)
                return;

            PlayerForm form = _forms.CurrentForm;
            if (form != PlayerForm.Form1 && form != PlayerForm.Form2)
                return;

            if (_head == null)
                return;

            bool rmb = Input.GetMouseButton(1);
            bool forward = Input.GetAxisRaw("Vertical") > 0.1f;

            float desiredYaw = GetYawFromForward(_cameraTransform.forward);
            float bodyYaw = _bodyRoot.eulerAngles.y;

            bool shouldBodyFollow = !rmb || forward;

            if (shouldBodyFollow)
            {
                float kBody = 1f - Mathf.Exp(-_bodyFollowSpeed * Time.deltaTime);
                float newYaw = Mathf.LerpAngle(bodyYaw, desiredYaw, kBody);
                _bodyRoot.rotation = Quaternion.Euler(0f, newYaw, 0f);

                bodyYaw = newYaw;
            }

            float deltaYaw = Mathf.DeltaAngle(bodyYaw, desiredYaw);
            float headYaw = Mathf.Clamp(deltaYaw, -_headYawLimit, _headYawLimit);

            float desiredPitch = GetPitchFromForward(_cameraTransform.forward);
            float headPitch = Mathf.Clamp(desiredPitch, -_headPitchLimit, _headPitchLimit);

            float kYaw = 1f - Mathf.Exp(-_headFollow * Time.deltaTime);
            float kPitch = 1f - Mathf.Exp(-_headPitchFollow * Time.deltaTime);
            float k = Mathf.Max(kYaw, kPitch);

            Quaternion target = _headBase * Quaternion.Euler(headPitch, headYaw, 0f);
            _head.localRotation = Quaternion.Slerp(_head.localRotation, target, k);
        }

        private static float GetYawFromForward(Vector3 forward)
        {
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                return 0f;

            forward.Normalize();
            return Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        }

        private static float GetPitchFromForward(Vector3 forward)
        {
            float y = Mathf.Clamp(forward.y, -1f, 1f);
            float horizontal = new Vector2(forward.x, forward.z).magnitude;

            if (horizontal < 0.0001f)
                return 0f;

            return -Mathf.Atan2(y, horizontal) * Mathf.Rad2Deg;
        }
    }
}
