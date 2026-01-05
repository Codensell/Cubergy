using Cubergy.Player;
using UnityEngine;

namespace Cubergy.Audio
{
    public sealed class PlayerFormChangeSfx : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private PlayerAudioEmitter _audio;

        [SerializeField] private AudioClip _toForm1;
        [SerializeField] private AudioClip _toForm2;

        [SerializeField] private float _volume = 1f;

        private void OnEnable()
        {
            if (_forms != null)
                _forms.FormChanged += OnFormChanged;
        }

        private void OnDisable()
        {
            if (_forms != null)
                _forms.FormChanged -= OnFormChanged;
        }

        private void OnFormChanged(PlayerForm form)
        {
            if (_audio == null)
                return;

            if (form == PlayerForm.Form1)
            {
                _audio.DuckFootstepsForOneShot(_toForm1);
                _audio.PlayOneShot(_toForm1, _volume, 1f);
                return;
            }

            if (form == PlayerForm.Form2)
            {
                _audio.DuckFootstepsForOneShot(_toForm2);
                _audio.PlayOneShot(_toForm2, _volume, 1f);
            }
        }
    }
}