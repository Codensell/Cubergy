using UnityEngine;

namespace Cubergy.Audio
{
    public sealed class SfxPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;

        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (_source == null || clip == null)
                return;

            _source.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }
}