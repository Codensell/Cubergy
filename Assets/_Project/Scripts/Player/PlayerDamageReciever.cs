using UnityEngine;

namespace Cubergy.Combat
{
    public sealed class PlayerDamageReceiver : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _health;

        public void ApplyDamage(int amount)
        {
            if (_health == null)
                return;

            _health.Damage(amount);
        }
    }
}