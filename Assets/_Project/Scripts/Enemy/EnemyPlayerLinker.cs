using UnityEngine;

namespace Cubergy.Enemy
{
    [DisallowMultipleComponent]
    public sealed class EnemyPlayerLinker : MonoBehaviour
    {
        [SerializeField] private string _playerTag = "Player";

        [SerializeField] private EnemyChasePlayerByForm _chase;
        [SerializeField] private EnemyLaserAttack _attack;

        private void Awake()
        {
            if (_chase == null) _chase = GetComponent<EnemyChasePlayerByForm>();
            if (_attack == null) _attack = GetComponent<EnemyLaserAttack>();

            var playerGo = GameObject.FindWithTag(_playerTag);
            if (playerGo == null)
                return;

            Transform player = playerGo.transform;

            _chase?.SetPlayer(player);
            _attack?.SetPlayer(player);
        }
    }
}