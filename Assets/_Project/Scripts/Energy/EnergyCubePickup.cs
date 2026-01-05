using System;
using Cubergy.Audio;
using UnityEngine;

namespace Cubergy.Energy
{
    public sealed class EnergyCubePickup : MonoBehaviour
    {
        public event Action Collected;

        [SerializeField] private int _amount = 1;

        private bool _picked;
        private SfxPlayer _sfx;
        private AudioClip _pickupClip;

        public void Configure(SfxPlayer sfx, AudioClip pickupClip)
        {
            _sfx = sfx;
            _pickupClip = pickupClip;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_picked)
                return;

            var wallet = other.GetComponentInParent<EnergyWallet>();
            if (wallet == null)
                return;

            _picked = true;

            wallet.Add(_amount);
            _sfx?.PlayOneShot(_pickupClip);

            Collected?.Invoke();
            Destroy(gameObject);
        }
    }
}