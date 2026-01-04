using Cubergy.Combat;
using UnityEngine;

namespace Cubergy.Player
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 3f;

        [SerializeField] private float _minScale = 0.18f;
        [SerializeField] private float _maxScale = 0.45f;

        [SerializeField] private int _damageMin = 1;
        [SerializeField] private int _damageMax = 3;

        private float _t;
        private int _damage;

        public void Init(float power01)
        {
            float p = Mathf.Clamp01(power01);

            float s = Mathf.Lerp(_minScale, _maxScale, p);
            transform.localScale = Vector3.one * s;

            _damage = Mathf.RoundToInt(Mathf.Lerp(_damageMin, _damageMax, p));
            if (_damage < 1)
                _damage = 1;
        }

        private void Update()
        {
            _t += Time.deltaTime;
            if (_t >= _lifetime)
                Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var target = collision.collider.GetComponentInParent<TargetHealth>();
            if (target != null)
                target.ApplyDamage(_damage);

            Destroy(gameObject);
        }
    }
}