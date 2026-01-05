using Cubergy.Player;
using UnityEngine;

namespace Cubergy.Player
{
    public sealed class Form1DinoMotionAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private Transform _playerRoot;

        [Header("Body")]
        [SerializeField] private Transform _bodyRoot;
        [SerializeField] private Transform _headRoot;

        [Header("Tail")]
        [SerializeField] private Transform _tailRoot;
        [SerializeField] private Transform _tail01;
        [SerializeField] private Transform _tail02;
        [SerializeField] private Transform _tail03;

        [Header("Legs Roots")]
        [SerializeField] private Transform _legL;
        [SerializeField] private Transform _legR;

        [Header("Legs Segments")]
        [SerializeField] private Transform _legLThigh;
        [SerializeField] private Transform _legLShin;
        [SerializeField] private Transform _legRThigh;
        [SerializeField] private Transform _legRShin;

        [Header("Tuning")]
        [SerializeField] private float _speedForFullEffect = 7f;
        [SerializeField] private float _moveThreshold = 0.05f;

        [SerializeField] private float _bodyStretchZ = 0.10f;
        [SerializeField] private float _bodySquashY = 0.06f;
        [SerializeField] private float _headForward = 0.08f;

        [SerializeField] private float _tailYaw = 18f;
        [SerializeField] private float _tailSpeed = 7f;

        [SerializeField] private float _legSpeed = 9f;

        [Header("Leg Hip Motion")]
        [SerializeField] private float _hipLiftY = 0.08f;
        [SerializeField] private float _hipForwardZ = 0.06f;

        [Header("Leg Rotations")]
        [SerializeField] private float _thighPitch = 22f;
        [SerializeField] private float _shinPitch = 10f;
        [SerializeField] private float _kneeBendOnLift = 14f;

        [Header("Body Vertical Bob")]
        [SerializeField] private float _bodySinkY = 0.05f;
        [SerializeField] private float _bodyRiseY = 0.03f;

        [SerializeField] private float _blend = 12f;
        [SerializeField] private PlayerMovement _movement;

        [Header("Jump Pose")]
        [SerializeField] private float _jumpPushTime = 0.08f;
        [SerializeField] private float _jumpPushThigh = 22f;
        [SerializeField] private float _jumpPushShin = -8f;

        [SerializeField] private float _jumpTuckThigh = -90f;
        [SerializeField] private float _jumpTuckShin = 35f;

        private bool _wasGrounded;
        private float _pushT;


        private Vector3 _bodyBasePos;
        private Vector3 _headBasePos;
        private Vector3 _bodyBaseScale;

        private Quaternion _tailBaseRot;
        private Quaternion _tail01BaseRot;
        private Quaternion _tail02BaseRot;
        private Quaternion _tail03BaseRot;

        private Vector3 _legLBasePos;
        private Vector3 _legRBasePos;

        private Quaternion _legLThighBaseRot;
        private Quaternion _legLShinBaseRot;
        private Quaternion _legRThighBaseRot;
        private Quaternion _legRShinBaseRot;

        private Vector3 _lastPlayerPos;

        private void Awake()
        {
            if (_playerRoot == null)
                _playerRoot = transform.root;

            _lastPlayerPos = _playerRoot.position;

            _bodyBasePos = _bodyRoot != null ? _bodyRoot.localPosition : Vector3.zero;
            _headBasePos = _headRoot != null ? _headRoot.localPosition : Vector3.zero;
            _bodyBaseScale = _bodyRoot != null ? _bodyRoot.localScale : Vector3.one;

            _tailBaseRot = _tailRoot != null ? _tailRoot.localRotation : Quaternion.identity;
            _tail01BaseRot = _tail01 != null ? _tail01.localRotation : Quaternion.identity;
            _tail02BaseRot = _tail02 != null ? _tail02.localRotation : Quaternion.identity;
            _tail03BaseRot = _tail03 != null ? _tail03.localRotation : Quaternion.identity;

            _legLBasePos = _legL != null ? _legL.localPosition : Vector3.zero;
            _legRBasePos = _legR != null ? _legR.localPosition : Vector3.zero;

            _legLThighBaseRot = _legLThigh != null ? _legLThigh.localRotation : Quaternion.identity;
            _legLShinBaseRot = _legLShin != null ? _legLShin.localRotation : Quaternion.identity;
            _legRThighBaseRot = _legRThigh != null ? _legRThigh.localRotation : Quaternion.identity;
            _legRShinBaseRot = _legRShin != null ? _legRShin.localRotation : Quaternion.identity;
            _wasGrounded = _movement != null && _movement.IsGrounded;

        }

        private void LateUpdate()
        {
            if (_forms != null && _forms.CurrentForm != PlayerForm.Form1)
                return;
            bool grounded = _movement != null && _movement.IsGrounded;

            if (_wasGrounded && !grounded)
                _pushT = _jumpPushTime;

            _wasGrounded = grounded;

            if (_pushT > 0f)
                _pushT -= Time.deltaTime;

            float dt = Time.deltaTime;
            if (dt <= 0f)
                return;

            Vector3 currentPos = _playerRoot.position;
            Vector3 delta = currentPos - _lastPlayerPos;
            _lastPlayerPos = currentPos;

            float horizontalSpeed = new Vector3(delta.x, 0f, delta.z).magnitude / dt;
            float moving01 = horizontalSpeed <= _moveThreshold ? 0f : Mathf.Clamp01(horizontalSpeed / Mathf.Max(0.001f, _speedForFullEffect));

            float t = Time.time;
            float leftSin = Mathf.Sin(t * _legSpeed);
            float rightSin = Mathf.Sin(t * _legSpeed + Mathf.PI);

            float leftLift = Mathf.Max(0f, leftSin) * moving01;
            float rightLift = Mathf.Max(0f, rightSin) * moving01;

            float leftPush = Mathf.Max(0f, -leftSin) * moving01;
            float rightPush = Mathf.Max(0f, -rightSin) * moving01;

            AnimateBody(moving01, leftLift + rightLift, leftPush + rightPush, dt);
            AnimateTail(moving01, dt);
            AnimateLegs(moving01, leftSin, rightSin, leftLift, rightLift, dt);
        }

        private void AnimateBody(float moving01, float liftSum, float pushSum, float dt)
        {
            if (_bodyRoot == null || _headRoot == null)
                return;

            float targetStretch = _bodyStretchZ * moving01;
            float targetSquash = _bodySquashY * moving01;

            Vector3 targetScale = new Vector3(
                _bodyBaseScale.x,
                _bodyBaseScale.y - targetSquash,
                _bodyBaseScale.z + targetStretch
            );

            float bodyBob = (-_bodySinkY * liftSum) + (_bodyRiseY * pushSum);
            Vector3 targetBodyPos = _bodyBasePos + new Vector3(0f, bodyBob, 0f);

            Vector3 targetHeadPos = _headBasePos
                                    + new Vector3(0f, bodyBob * 0.5f, _headForward * moving01);

            float k = 1f - Mathf.Exp(-_blend * dt);

            _bodyRoot.localScale = Vector3.Lerp(_bodyRoot.localScale, targetScale, k);
            _bodyRoot.localPosition = Vector3.Lerp(_bodyRoot.localPosition, targetBodyPos, k);
            _headRoot.localPosition = Vector3.Lerp(_headRoot.localPosition, targetHeadPos, k);
        }

        private void AnimateTail(float moving01, float dt)
        {
            if (_tailRoot == null)
                return;

            float t = Time.time;

            float yawRoot = Mathf.Sin(t * _tailSpeed) * _tailYaw * moving01;
            float yaw1 = Mathf.Sin(t * _tailSpeed + 0.35f) * (_tailYaw * 0.8f) * moving01;
            float yaw2 = Mathf.Sin(t * _tailSpeed + 0.70f) * (_tailYaw * 0.6f) * moving01;
            float yaw3 = Mathf.Sin(t * _tailSpeed + 1.05f) * (_tailYaw * 0.45f) * moving01;

            float k = 1f - Mathf.Exp(-_blend * dt);

            Quaternion r0 = _tailBaseRot * Quaternion.Euler(0f, yawRoot, 0f);
            _tailRoot.localRotation = Quaternion.Slerp(_tailRoot.localRotation, r0, k);

            if (_tail01 != null)
            {
                Quaternion r1 = _tail01BaseRot * Quaternion.Euler(0f, yaw1, 0f);
                _tail01.localRotation = Quaternion.Slerp(_tail01.localRotation, r1, k);
            }

            if (_tail02 != null)
            {
                Quaternion r2 = _tail02BaseRot * Quaternion.Euler(0f, yaw2, 0f);
                _tail02.localRotation = Quaternion.Slerp(_tail02.localRotation, r2, k);
            }

            if (_tail03 != null)
            {
                Quaternion r3 = _tail03BaseRot * Quaternion.Euler(0f, yaw3, 0f);
                _tail03.localRotation = Quaternion.Slerp(_tail03.localRotation, r3, k);
            }
        }

        private void AnimateLegs(float moving01, float leftSin, float rightSin, float leftLift, float rightLift, float dt)
{
    if (_legL == null || _legR == null)
        return;

    float k = 1f - Mathf.Exp(-_blend * dt);

    Vector3 leftHipPos = _legLBasePos + new Vector3(0f, _hipLiftY * leftLift, _hipForwardZ * leftLift);
    Vector3 rightHipPos = _legRBasePos + new Vector3(0f, _hipLiftY * rightLift, _hipForwardZ * rightLift);

    _legL.localPosition = Vector3.Lerp(_legL.localPosition, leftHipPos, k);
    _legR.localPosition = Vector3.Lerp(_legR.localPosition, rightHipPos, k);

    if (_legLThigh != null)
    {
        float thighPitch = leftSin * _thighPitch * moving01;
        Quaternion rot = _legLThighBaseRot * Quaternion.Euler(thighPitch, 0f, 0f);
        _legLThigh.localRotation = Quaternion.Slerp(_legLThigh.localRotation, rot, k);
    }

    if (_legRThigh != null)
    {
        float thighPitch = rightSin * _thighPitch * moving01;
        Quaternion rot = _legRThighBaseRot * Quaternion.Euler(thighPitch, 0f, 0f);
        _legRThigh.localRotation = Quaternion.Slerp(_legRThigh.localRotation, rot, k);
    }

    if (_legLShin != null)
    {
        float shinPitch = leftSin * _shinPitch * moving01;
        float kneeBend = _kneeBendOnLift * leftLift;
        Quaternion rot = _legLShinBaseRot * Quaternion.Euler(shinPitch + kneeBend, 0f, 0f);
        _legLShin.localRotation = Quaternion.Slerp(_legLShin.localRotation, rot, k);
    }

    if (_legRShin != null)
    {
        float shinPitch = rightSin * _shinPitch * moving01;
        float kneeBend = _kneeBendOnLift * rightLift;
        Quaternion rot = _legRShinBaseRot * Quaternion.Euler(shinPitch + kneeBend, 0f, 0f);
        _legRShin.localRotation = Quaternion.Slerp(_legRShin.localRotation, rot, k);
    }

    bool grounded = _movement != null && _movement.IsGrounded;

    if (!grounded)
    {
        float push01 = _jumpPushTime > 0f ? Mathf.Clamp01(_pushT / _jumpPushTime) : 0f;

        float thigh = Mathf.Lerp(_jumpTuckThigh, _jumpPushThigh, push01);
        float shin = Mathf.Lerp(_jumpTuckShin, _jumpPushShin, push01);

        if (_legLThigh != null)
            _legLThigh.localRotation = Quaternion.Slerp(_legLThigh.localRotation, _legLThighBaseRot * Quaternion.Euler(thigh, 0f, 0f), k);
        if (_legRThigh != null)
            _legRThigh.localRotation = Quaternion.Slerp(_legRThigh.localRotation, _legRThighBaseRot * Quaternion.Euler(thigh, 0f, 0f), k);

        if (_legLShin != null)
            _legLShin.localRotation = Quaternion.Slerp(_legLShin.localRotation, _legLShinBaseRot * Quaternion.Euler(shin, 0f, 0f), k);
        if (_legRShin != null)
            _legRShin.localRotation = Quaternion.Slerp(_legRShin.localRotation, _legRShinBaseRot * Quaternion.Euler(shin, 0f, 0f), k);

        _legL.localPosition = Vector3.Lerp(_legL.localPosition, _legLBasePos, k);
        _legR.localPosition = Vector3.Lerp(_legR.localPosition, _legRBasePos, k);
    }
}

    }
}
