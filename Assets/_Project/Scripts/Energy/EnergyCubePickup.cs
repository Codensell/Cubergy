using UnityEngine;

namespace Cubergy.Energy
{
    public sealed class EnergyCubePickup : MonoBehaviour
    {
        [SerializeField] private int _amount = 1;

        private void OnTriggerEnter(Collider other)
        {
            var wallet = other.GetComponentInParent<EnergyWallet>();
            if (wallet == null)
                return;

            wallet.Add(_amount);
            Destroy(gameObject);
        }
    }
}