using System;
using UnityEngine;

namespace Cubergy.Combat
{
    public sealed class TargetHealth : MonoBehaviour
    {
        /// <summary>
        /// Fired for a specific TargetHealth instance when it dies.
        /// </summary>
        public event Action<TargetHealth, DamageInstigator> Died;

        /// <summary>
        /// Fired for any TargetHealth instance in the scene when it dies.
        /// Useful for systems that must react to deaths of dynamically spawned targets.
        /// </summary>
        public static event Action<TargetHealth, DamageInstigator> AnyDied;

        [SerializeField] private int _hp = 3;

        private bool _isDead;

        public void ApplyDamage(int damage)
        {
            ApplyDamage(damage, DamageInstigator.Unknown);
        }

        public void ApplyDamage(int damage, DamageInstigator instigator)
        {
            if (_isDead)
                return;

            if (damage < 1)
                damage = 1;

            _hp -= damage;

            if (_hp > 0)
                return;

            _isDead = true;

            Died?.Invoke(this, instigator);
            AnyDied?.Invoke(this, instigator);

            Destroy(gameObject);
        }
    }
}