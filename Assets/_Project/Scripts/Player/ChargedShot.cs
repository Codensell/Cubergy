using System.Collections;
using Cubergy.Combat;
using UnityEngine;
using Cubergy.Audio;

namespace Cubergy.Player
{
    public sealed class ChargedShot : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private PlayerAudioEmitter _audio;
        [SerializeField] private AudioClip _chargeLoopClip;
        [SerializeField] private float _chargeLoopVolume = 0.8f;
        [SerializeField] private float _chargePitchMin = 0.95f;
        [SerializeField] private float _chargePitchMax = 1.10f;

        [Header("Muzzles")]
        [SerializeField] private Transform _form1Muzzle;
        [SerializeField] private Transform _form2Muzzle;

        [Header("Charge")]
        [SerializeField] private float _chargeTime = 0.6f;

        [Header("Form2 Delay")]
        [SerializeField] private float _form2FireDelay = 0.25f;

        [Header("Damage")]
        [SerializeField] private int _form1DamageMin = 1;
        [SerializeField] private int _form1DamageMax = 3;
        [SerializeField] private int _form2DamageMin = 2;
        [SerializeField] private int _form2DamageMax = 6;

        [Header("Laser")]
        [SerializeField] private LaserShotVfx _laserVfx;
        [SerializeField] private float _range = 70f;

        [SerializeField] private float _form1WidthMin = 0.03f;
        [SerializeField] private float _form1WidthMax = 0.10f;

        [SerializeField] private float _form2WidthMin = 0.05f;
        [SerializeField] private float _form2WidthMax = 0.14f;

        [SerializeField] private float _laserDuration = 0.06f;
        [SerializeField] private float _beamDuration = 0.25f;

        private bool _isCharging;
        private float _charge01;
        private Coroutine _fireRoutine;

        private void Update()
        {
            if (!CanShoot())
            {
                ResetCharge();
                StopRoutine();

                if (_audio != null)
                {
                    _audio.StopChargeLoop();
                    _audio.SetChargeDucking(false);
                }

                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _isCharging = true;
                _charge01 = 0f;

                if (_audio != null)
                {
                    _audio.SetChargeDucking(true);
                    _audio.StartChargeLoop(_chargeLoopClip, _chargeLoopVolume, _chargePitchMin);
                }
            }

            if (!_isCharging)
                return;

            _charge01 += Time.deltaTime / Mathf.Max(0.0001f, _chargeTime);

            if (_audio != null)
            {
                float pitch = Mathf.Lerp(_chargePitchMin, _chargePitchMax, Mathf.Clamp01(_charge01));
                _audio.StartChargeLoop(_chargeLoopClip, _chargeLoopVolume, pitch);
            }

            if (_charge01 >= 1f)
            {
                TryFire(1f);

                if (_audio != null)
                {
                    _audio.StopChargeLoop();
                    _audio.SetChargeDucking(false);
                }

                ResetCharge();
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryFire(Mathf.Clamp01(_charge01));

                if (_audio != null)
                {
                    _audio.StopChargeLoop();
                    _audio.SetChargeDucking(false);
                }

                ResetCharge();
            }
        }

        private bool CanShoot()
        {
            if (_forms == null)
                return false;

            PlayerForm f = _forms.CurrentForm;
            return f == PlayerForm.Form1 || f == PlayerForm.Form2;
        }

        private void ResetCharge()
        {
            _isCharging = false;
            _charge01 = 0f;
        }

        private void StopRoutine()
        {
            if (_fireRoutine == null)
                return;

            StopCoroutine(_fireRoutine);
            _fireRoutine = null;
        }

        private void TryFire(float power01)
        {
            Transform muzzle = GetMuzzle();
            if (muzzle == null)
                return;

            StopRoutine();

            if (_forms.CurrentForm == PlayerForm.Form2 && _form2FireDelay > 0f)
            {
                _fireRoutine = StartCoroutine(FireDelayed(power01));
                return;
            }

            FireNow(power01);
        }

        private IEnumerator FireDelayed(float power01)
        {
            yield return new WaitForSeconds(_form2FireDelay);

            if (!CanShoot())
            {
                _fireRoutine = null;
                yield break;
            }

            FireNow(power01);
            _fireRoutine = null;
        }

        private Transform GetMuzzle()
        {
            return _forms.CurrentForm == PlayerForm.Form1 ? _form1Muzzle : _form2Muzzle;
        }

        private void FireNow(float power01)
        {
            Transform muzzle = GetMuzzle();
            if (muzzle == null)
                return;

            float p = Mathf.Clamp01(power01);

            float width = GetWidth(p);
            float duration = _forms.CurrentForm == PlayerForm.Form2 ? _beamDuration : _laserDuration;

            if (_laserVfx != null)
                _laserVfx.Play(muzzle.position, muzzle.forward, width, _range, duration);

            int damage = GetDamage(p);
            ApplyDamageAlongRay(muzzle.position, muzzle.forward, _range, damage);
        }

        private float GetWidth(float power01)
        {
            if (_forms.CurrentForm == PlayerForm.Form1)
                return Mathf.Lerp(_form1WidthMin, _form1WidthMax, power01);

            return Mathf.Lerp(_form2WidthMin, _form2WidthMax, power01);
        }

        private int GetDamage(float power01)
        {
            if (_forms.CurrentForm == PlayerForm.Form1)
                return Mathf.RoundToInt(Mathf.Lerp(_form1DamageMin, _form1DamageMax, power01));

            return Mathf.RoundToInt(Mathf.Lerp(_form2DamageMin, _form2DamageMax, power01));
        }

        private static void ApplyDamageAlongRay(Vector3 origin, Vector3 dir, float range, int damage)
        {
            if (damage < 1)
                damage = 1;

            Ray ray = new Ray(origin, dir);

            RaycastHit[] hits = Physics.RaycastAll(ray, range);
            if (hits == null || hits.Length == 0)
                return;

            for (int i = 0; i < hits.Length; i++)
            {
                var target = hits[i].collider.GetComponentInParent<TargetHealth>();
                if (target != null)
                    target.ApplyDamage(damage, DamageInstigator.Player);

            }
        }
    }
}
