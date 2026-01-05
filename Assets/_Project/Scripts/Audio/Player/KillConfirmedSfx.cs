using Cubergy.Combat;
using UnityEngine;

namespace Cubergy.Audio
{
    public sealed class KillConfirmedSfx : MonoBehaviour
    {
        [SerializeField] private PlayerAudioEmitter _audio;
        [SerializeField] private AudioClip _killConfirmed;
        [SerializeField] private float _volume = 1f;

        private TargetHealth[] _targets;

        private void OnEnable()
        {
            _targets = FindObjectsByType<TargetHealth>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            for (int i = 0; i < _targets.Length; i++)
            {
                var t = _targets[i];
                if (t == null)
                    continue;

                t.Died += OnTargetDied;
            }
        }

        private void OnDisable()
        {
            if (_targets == null)
                return;

            for (int i = 0; i < _targets.Length; i++)
            {
                var t = _targets[i];
                if (t == null)
                    continue;

                t.Died -= OnTargetDied;
            }
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