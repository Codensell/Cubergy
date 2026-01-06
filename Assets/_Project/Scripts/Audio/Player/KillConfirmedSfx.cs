using Cubergy.Combat;
using UnityEngine;

namespace Cubergy.Audio
{
    public sealed class KillConfirmedSfx : MonoBehaviour
    {
        [SerializeField] private PlayerAudioEmitter _audio;
        [SerializeField] private AudioClip _killConfirmed;
        [SerializeField] private float _volume = 1f;

        private void OnEnable()
        {
            TargetHealth.AnyDied += OnTargetDied;
        }

        private void OnDisable()
        {
            TargetHealth.AnyDied -= OnTargetDied;
        }

        private void OnTargetDied(TargetHealth target, DamageInstigator instigator)
        {
            if (instigator != DamageInstigator.Player)
                return;

            if (_audio == null || _killConfirmed == null)
                return;

            _audio.PlayOneShot(_killConfirmed, _volume, 1f);
        }
    }
}