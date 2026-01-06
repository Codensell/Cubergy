using Cubergy.Energy;
using UnityEngine;
using UnityEngine.AI;

namespace Cubergy.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyMoveToTarget))]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EnemyEnergyCubeTargeter : MonoBehaviour
    {
        [SerializeField] private float _searchInterval = 0.5f;
        [SerializeField] private float _maxDistance = 200f;

        [Header("NavMesh Fix")]
        [SerializeField] private float _snapToNavMeshRadius = 2.0f;

        private EnemyMoveToTarget _mover;
        private NavMeshAgent _agent;

        private float _nextSearchTime;

        private void Awake()
        {
            _mover = GetComponent<EnemyMoveToTarget>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            TrySnapToNavMeshIfNeeded();

            if (Time.time < _nextSearchTime)
                return;

            _nextSearchTime = Time.time + Mathf.Max(0.05f, _searchInterval);

            Transform best = FindClosestEnergyCube();
            if (best != null)
                _mover.SetTarget(best);
        }

        private void TrySnapToNavMeshIfNeeded()
        {
            if (_agent.isOnNavMesh)
                return;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, _snapToNavMeshRadius, NavMesh.AllAreas))
                transform.position = hit.position;
        }

        private Transform FindClosestEnergyCube()
        {
            EnergyCubePickup[] cubes = Object.FindObjectsByType<EnergyCubePickup>(FindObjectsSortMode.None);
            if (cubes == null || cubes.Length == 0)
                return null;

            Vector3 from = transform.position;

            float maxSqr = _maxDistance * _maxDistance;
            float bestSqr = float.PositiveInfinity;
            Transform best = null;

            for (int i = 0; i < cubes.Length; i++)
            {
                EnergyCubePickup cube = cubes[i];
                if (cube == null)
                    continue;

                Vector3 d = cube.transform.position - from;
                float sqr = d.sqrMagnitude;

                if (sqr > maxSqr)
                    continue;

                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = cube.transform;
                }
            }

            return best;
        }
    }
}
