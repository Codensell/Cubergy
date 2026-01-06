using System.Collections;
using Cubergy.Combat;
using UnityEngine;

namespace Cubergy.Enemy
{
    [DisallowMultipleComponent]
    public sealed class EnemyBindPlayerRuntime : MonoBehaviour
    {
        [Header("Optional override")]
        [SerializeField] private Transform _player;

        [Header("Search")]
        [SerializeField] private string _playerTag = "Player";
        [SerializeField] private bool _tryFindByTag = true;
        [SerializeField] private bool _tryFindByDamageReceiver = true;

        [Header("Receivers")]
        [SerializeField] private EnemyChasePlayerByForm _chase;
        [SerializeField] private EnemyLaserAttack _attack;

        [Header("Retry")]
        [SerializeField] private float _retryInterval = 0.25f;
        [SerializeField] private float _maxRetryTime = 5f;

        private Coroutine _routine;

        private void Awake()
        {
            if (_chase == null)
                _chase = GetComponent<EnemyChasePlayerByForm>();

            if (_attack == null)
                _attack = GetComponent<EnemyLaserAttack>();
        }

        private void OnEnable()
        {
            _routine = StartCoroutine(BindWhenReady());
        }

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
        }

        private IEnumerator BindWhenReady()
        {
            float t = 0f;

            while (_player == null && t < _maxRetryTime)
            {
                _player = TryResolvePlayer();
                if (_player != null)
                    break;

                t += _retryInterval;
                yield return new WaitForSeconds(_retryInterval);
            }

            if (_player == null)
                yield break;

            ApplyPlayer(_player);
        }

        private Transform TryResolvePlayer()
        {
            if (_tryFindByTag && !string.IsNullOrWhiteSpace(_playerTag))
            {
                GameObject go = GameObject.FindGameObjectWithTag(_playerTag);
                if (go != null)
                    return go.transform;
            }

            if (_tryFindByDamageReceiver)
            {
                PlayerDamageReceiver receiver = FindFirstObjectByType<PlayerDamageReceiver>();
                if (receiver != null)
                    return receiver.transform;
            }

            return null;
        }

        private void ApplyPlayer(Transform player)
        {
            if (_chase != null)
                _chase.SetPlayer(player);

            if (_attack != null)
                _attack.SetPlayer(player);
        }
    }
}
