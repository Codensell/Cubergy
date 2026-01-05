using System.Collections;
using UnityEngine;

namespace Cubergy.Combat
{
    public sealed class LaserShotVfx : MonoBehaviour
    {
        [SerializeField] private LineRenderer _line;

        private Coroutine _routine;

        private void Awake()
        {
            Hide();
        }

        public void Play(Vector3 origin, Vector3 direction, float width, float length, float duration)
        {
            if (_line == null)
                return;

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(PlayRoutine(origin, direction, width, length, duration));
        }

        private IEnumerator PlayRoutine(Vector3 origin, Vector3 direction, float width, float length, float duration)
        {
            Vector3 end = origin + direction * length;

            _line.enabled = true;
            _line.startWidth = width;
            _line.endWidth = width;
            _line.SetPosition(0, origin);
            _line.SetPosition(1, end);

            yield return new WaitForSeconds(duration);

            Hide();
            _routine = null;
        }

        private void Hide()
        {
            if (_line == null)
                return;

            _line.enabled = false;
            _line.SetPosition(0, Vector3.zero);
            _line.SetPosition(1, Vector3.zero);
        }
    }
}