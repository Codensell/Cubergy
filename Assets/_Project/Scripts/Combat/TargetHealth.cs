using System;
using UnityEngine;

namespace Cubergy.Combat
{
    public sealed class TargetHealth : MonoBehaviour
    {
        public event Action<TargetHealth> Died;

        [SerializeField] private int _hp = 3;

        public void ApplyDamage(int damage)
        {
            if (damage <= 0)
                return;

            _hp -= damage;
            if (_hp <= 0)
            {
                Died?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}