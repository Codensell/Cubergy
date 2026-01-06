using UnityEngine;
using UnityEngine.AI;

namespace Cubergy.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EnemyMoveToTarget : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (_target == null)
                return;

            if (!_agent.isOnNavMesh)
                return;

            _agent.SetDestination(_target.position);
        }
        public void SetTarget(Transform target)
        {
            _target = target;
        }

    }
}