using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubergy.Energy
{
    public sealed class EnergyCubeSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _energyCubePrefab;
        [SerializeField] private Transform _pointsRoot;

        [SerializeField] private float _respawnDelaySeconds = 20f;
        [SerializeField] private bool _spawnOnStart = true;
        [SerializeField] private Cubergy.Audio.SfxPlayer _sfx;
        [SerializeField] private AudioClip _pickupClip;

        private readonly Dictionary<Transform, GameObject> _alive = new();

        private void Start()
        {
            if (_spawnOnStart)
                SpawnAll();
        }

        public void SpawnAll()
        {
            if (_energyCubePrefab == null || _pointsRoot == null)
                return;

            int count = _pointsRoot.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform p = _pointsRoot.GetChild(i);
                if (p == null)
                    continue;

                if (_alive.TryGetValue(p, out GameObject existing) && existing != null)
                    continue;

                SpawnAt(p);
            }
        }

        private void SpawnAt(Transform point)
        {
            GameObject cube = Instantiate(_energyCubePrefab, point.position, point.rotation);

            var pickup = cube.GetComponent<EnergyCubePickup>();
            if (pickup != null)
            {
                pickup.Configure(_sfx, _pickupClip);
                pickup.Collected += () => OnCollected(point);
            }

            _alive[point] = cube;
        }


        private void OnCollected(Transform point)
        {
            if (_alive.ContainsKey(point))
                _alive[point] = null;

            StartCoroutine(RespawnAfter(point));
        }

        private IEnumerator RespawnAfter(Transform point)
        {
            yield return new WaitForSeconds(_respawnDelaySeconds);
            SpawnAt(point);
        }
    }
}