using UnityEngine;

namespace Cubergy.Player
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 3f;

        [SerializeField] private float _minScale = 0.18f;
        [SerializeField] private float _maxScale = 0.45f;

        private float _t;

        public void Init(float power01)
        {
            float s = Mathf.Lerp(_minScale, _maxScale, Mathf.Clamp01(power01));
            transform.localScale = Vector3.one * s;
        }

        private void Update()
        {
            _t += Time.deltaTime;
            if (_t >= _lifetime)
                Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision _)
        {
            Destroy(gameObject);
        }
    }
}