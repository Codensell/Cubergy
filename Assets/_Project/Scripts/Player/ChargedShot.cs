using UnityEngine;

namespace Cubergy.Player
{
    public sealed class ChargedShot : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private Rigidbody _projectilePrefab;

        [SerializeField] private float _chargeTime = 0.6f;
        [SerializeField] private float _minSpeed = 12f;
        [SerializeField] private float _maxSpeed = 28f;
        [SerializeField] private float _spawnForwardOffset = 0.35f;

        private Collider[] _ownerColliders;
        private bool _isCharging;
        private float _charge01;
        
        private void Awake()
        {
            _ownerColliders = GetComponentsInChildren<Collider>(true);
        }
        
        private void Update()
        {
            if (_forms == null || _forms.CurrentForm != PlayerForm.Form2)
            {
                _isCharging = false;
                _charge01 = 0f;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _isCharging = true;
                _charge01 = 0f;
            }

            if (!_isCharging)
                return;

            _charge01 += Time.deltaTime / Mathf.Max(0.0001f, _chargeTime);

            if (_charge01 >= 1f)
            {
                Fire(1f);
                _isCharging = false;
                _charge01 = 0f;
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Fire(Mathf.Clamp01(_charge01));
                _isCharging = false;
                _charge01 = 0f;
            }
        }

        private void Fire(float power01)
        {
            if (_muzzle == null || _projectilePrefab == null)
                return;

            float speed = Mathf.Lerp(_minSpeed, _maxSpeed, power01);

            Vector3 spawnPos = _muzzle.position + _muzzle.forward * _spawnForwardOffset;

            Rigidbody rb = Instantiate(_projectilePrefab, spawnPos, _muzzle.rotation);

            var projectile = rb.GetComponent<Projectile>();
            if (projectile != null)
                projectile.Init(power01);

            var projCollider = rb.GetComponent<Collider>();
            if (projCollider != null && _ownerColliders != null)
            {
                for (int i = 0; i < _ownerColliders.Length; i++)
                {
                    Collider c = _ownerColliders[i];
                    if (c == null)
                        continue;

                    Physics.IgnoreCollision(projCollider, c, true);
                }
            }

            rb.linearVelocity = _muzzle.forward * speed;
        }

    }
}