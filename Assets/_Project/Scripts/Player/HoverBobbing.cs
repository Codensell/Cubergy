using UnityEngine;

namespace Cubergy.Player
{
    public sealed class HoverBobbing : MonoBehaviour
    {
        [SerializeField] private float _amplitude = 0.08f;
        [SerializeField] private float _frequency = 0.7f;

        private Vector3 _baseLocalPosition;
        private float _t;

        private void Awake()
        {
            _baseLocalPosition = transform.localPosition;
        }

        private void OnEnable()
        {
            _t = 0f;
            transform.localPosition = _baseLocalPosition;
        }

        private void Update()
        {
            _t += Time.deltaTime;
            float y = Mathf.Sin(_t * _frequency * Mathf.PI * 2f) * _amplitude;
            transform.localPosition = _baseLocalPosition + new Vector3(0f, y, 0f);
        }
    }
}