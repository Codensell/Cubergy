using System.Collections;
using UnityEngine;

namespace Cubergy.Audio
{
    public sealed class PlayerAudioEmitter : MonoBehaviour
    {
        [SerializeField] private AudioSource _oneShot;
        [SerializeField] private AudioSource _chargeLoop;
        [SerializeField] private AudioSource _footstepsLoop;

        [Header("Footsteps Ducking")]
        [SerializeField] private float _duckByChargeMultiplier = 0.25f;
        [SerializeField] private float _duckByOneShotMultiplier = 0.15f;
        [SerializeField] private float _duckFadeSeconds = 0.05f;

        private float _footstepsBaseVolume = 1f;

        private float _chargeDuck = 1f;
        private float _oneShotDuck = 1f;

        private float _duckCurrent = 1f;
        private float _duckTarget = 1f;
        private float _duckSpeed = 20f;

        private Coroutine _oneShotDuckRoutine;

        private void Awake()
        {
            _duckCurrent = 1f;
            _duckTarget = 1f;
            _duckSpeed = FadeToSpeed(_duckFadeSeconds);
        }

        private void Update()
        {
            _duckTarget = _chargeDuck * _oneShotDuck;

            float dt = Time.deltaTime;
            if (dt <= 0f)
                return;

            float k = 1f - Mathf.Exp(-_duckSpeed * dt);
            _duckCurrent = Mathf.Lerp(_duckCurrent, _duckTarget, k);

            if (_footstepsLoop != null && _footstepsLoop.isPlaying)
                _footstepsLoop.volume = Mathf.Clamp01(_footstepsBaseVolume * _duckCurrent);
        }

        public void PlayOneShot(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (_oneShot == null || clip == null)
                return;

            _oneShot.pitch = pitch;
            _oneShot.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        public void DuckFootstepsForOneShot(AudioClip clip)
        {
            if (clip == null)
                return;

            DuckFootstepsForSeconds(_duckByOneShotMultiplier, clip.length);
        }

        public void SetChargeDucking(bool enabled)
        {
            _chargeDuck = enabled ? _duckByChargeMultiplier : 1f;
        }

        public void StartChargeLoop(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            StartLoop(_chargeLoop, clip, volume, pitch);
        }

        public void StopChargeLoop()
        {
            StopLoop(_chargeLoop);
        }

        public void StartFootstepsLoop(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            _footstepsBaseVolume = Mathf.Clamp01(volume);
            StartLoop(_footstepsLoop, clip, _footstepsBaseVolume * _duckCurrent, pitch);
        }

        public void StopFootstepsLoop()
        {
            StopLoop(_footstepsLoop);
        }

        private void DuckFootstepsForSeconds(float multiplier, float seconds)
        {
            if (_oneShotDuckRoutine != null)
                StopCoroutine(_oneShotDuckRoutine);

            _oneShotDuckRoutine = StartCoroutine(OneShotDuckRoutine(multiplier, seconds));
        }

        private IEnumerator OneShotDuckRoutine(float multiplier, float seconds)
        {
            _oneShotDuck = Mathf.Clamp01(multiplier);

            float s = seconds;
            if (s < 0.01f)
                s = 0.01f;

            yield return new WaitForSeconds(s);

            _oneShotDuck = 1f;
            _oneShotDuckRoutine = null;
        }

        private static void StartLoop(AudioSource source, AudioClip clip, float volume, float pitch)
        {
            if (source == null || clip == null)
                return;

            if (source.isPlaying && source.clip == clip)
                return;

            source.clip = clip;
            source.loop = true;
            source.volume = Mathf.Clamp01(volume);
            source.pitch = pitch;
            source.Play();
        }

        private static void StopLoop(AudioSource source)
        {
            if (source == null)
                return;

            if (!source.isPlaying)
                return;

            source.Stop();
            source.clip = null;
        }

        private static float FadeToSpeed(float seconds)
        {
            float s = seconds;
            if (s < 0.01f)
                s = 0.01f;

            return 6f / s;
        }
    }
}
