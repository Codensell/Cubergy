using System;
using UnityEngine;

namespace Cubergy.Combat
{
    public sealed class TargetHealth : MonoBehaviour
    {
        public event Action<TargetHealth, DamageInstigator> Died;

        [SerializeField] private int _hp = 3;

        public void ApplyDamage(int damage)
        {
            ApplyDamage(damage, DamageInstigator.Unknown);
        }

        public void ApplyDamage(int damage, DamageInstigator instigator)
        {
            if (damage < 1)
                damage = 1;

            _hp -= damage;

            if (_hp > 0)
                return;

            Died?.Invoke(this, instigator);
            Destroy(gameObject);
        }
    }
}