using System.Collections.Generic;
using UnityEngine;

namespace Cubergy.Combat
{
    public sealed class TargetWaveSpawner : MonoBehaviour
    {
        [SerializeField] private TargetHealth _targetPrefab;
        [SerializeField] private Transform[] _spawnPoints;

        private readonly List<TargetHealth> _alive = new();

        public int AliveCount => _alive.Count;

        public void SpawnWave()
        {
            ClearWave();

            if (_targetPrefab == null || _spawnPoints == null || _spawnPoints.Length == 0)
                return;

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                Transform p = _spawnPoints[i];
                if (p == null)
                    continue;

                TargetHealth t = Instantiate(_targetPrefab, p.position, p.rotation);
                t.Died += OnTargetDied;
                _alive.Add(t);
            }
        }

        public void ClearWave()
        {
            for (int i = 0; i < _alive.Count; i++)
            {
                TargetHealth t = _alive[i];
                if (t == null)
                    continue;

                t.Died -= OnTargetDied;
                Destroy(t.gameObject);
            }

            _alive.Clear();
        }

        private void OnTargetDied(TargetHealth t, DamageInstigator instigator)
        {
            if (t != null)
                t.Died -= OnTargetDied;

            _alive.Remove(t);
        }
    }
}