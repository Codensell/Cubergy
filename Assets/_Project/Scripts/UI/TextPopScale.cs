using System.Collections;
using UnityEngine;

namespace Cubergy.UI
{
    public sealed class TextPopScale : MonoBehaviour
    {
        [SerializeField] private float _popScale = 1.25f;
        [SerializeField] private float _upTime = 0.06f;
        [SerializeField] private float _downTime = 0.10f;

        private Vector3 _baseScale;
        private Coroutine _routine;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        public void Play()
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            yield return LerpScale(_baseScale, _baseScale * _popScale, _upTime);
            yield return LerpScale(transform.localScale, _baseScale, _downTime);
            _routine = null;
        }

        private IEnumerator LerpScale(Vector3 from, Vector3 to, float time)
        {
            if (time <= 0f)
            {
                transform.localScale = to;
                yield break;
            }

            float t = 0f;
            while (t < time)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / time);
                transform.localScale = Vector3.LerpUnclamped(from, to, k);
                yield return null;
            }

            transform.localScale = to;
        }
    }
}