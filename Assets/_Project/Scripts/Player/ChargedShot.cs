using System;
using System.Collections;
using System.Collections.Generic;
using Cubergy.Audio;
using Cubergy.Combat;
using UnityEngine;

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

        [Header("Aim")]
        [SerializeField] private LayerMask _aimMask = ~0;

        [Header("Ignore Self")]
        [SerializeField] private Transform _selfRoot;

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
        [SerializeField] private float _form1Range = 70f;
        [SerializeField] private float _form2Range = 70f;

        [SerializeField] private float _form1WidthMin = 0.03f;
        [SerializeField] private float _form1WidthMax = 0.10f;

        [SerializeField] private float _form2WidthMin = 0.05f;
        [SerializeField] private float _form2WidthMax = 0.14f;

        [SerializeField] private float _laserDuration = 0.06f;
        [SerializeField] private float _beamDuration = 0.25f;

        private bool _isCharging;
        private float _charge01;
        private Coroutine _fireRoutine;

        private readonly RaycastHit[] _hits = new RaycastHit[32];
        private readonly RaycastHit[] _damageHits = new RaycastHit[64];
        private readonly HashSet<TargetHealth> _damaged = new HashSet<TargetHealth>();

        private void Awake()
        {
            if (_selfRoot == null)
                _selfRoot = transform;
        }

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
                StopChargeAudio();
                ResetCharge();
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryFire(Mathf.Clamp01(_charge01));
                StopChargeAudio();
                ResetCharge();
            }
        }

        private void StopChargeAudio()
        {
            if (_audio == null)
                return;

            _audio.StopChargeLoop();
            _audio.SetChargeDucking(false);
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

            if (_forms != null && _forms.CurrentForm == PlayerForm.Form2 && _form2FireDelay > 0f)
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
            return _forms != null && _forms.CurrentForm == PlayerForm.Form1 ? _form1Muzzle : _form2Muzzle;
        }

        private void FireNow(float power01)
        {
            Transform muzzle = GetMuzzle();
            if (muzzle == null)
                return;

            float p = Mathf.Clamp01(power01);

            float width = GetWidth(p);
            float duration = _forms != null && _forms.CurrentForm == PlayerForm.Form2 ? _beamDuration : _laserDuration;

            Vector3 aimDir = muzzle.forward;
            if (aimDir.sqrMagnitude < 0.0001f)
                aimDir = transform.forward;

            float range = GetRange();

            float beamLen = range;
            if (RaycastIgnoreSelf(muzzle.position, aimDir, range, out RaycastHit firstHit))
                beamLen = firstHit.distance;

            if (_laserVfx != null)
                _laserVfx.Play(muzzle.position, aimDir, width, beamLen, duration);

            int damage = GetDamage(p);

            if (_forms != null && _forms.CurrentForm == PlayerForm.Form2)
                ApplyDamageAll(muzzle.position, aimDir, beamLen, damage, _aimMask);
            else
                ApplyDamageFirst(muzzle.position, aimDir, beamLen, damage, _aimMask);
        }

        private float GetWidth(float power01)
        {
            if (_forms != null && _forms.CurrentForm == PlayerForm.Form1)
                return Mathf.Lerp(_form1WidthMin, _form1WidthMax, power01);

            return Mathf.Lerp(_form2WidthMin, _form2WidthMax, power01);
        }
        private float GetRange()
        {
            if (_forms != null && _forms.CurrentForm == PlayerForm.Form1)
                return _form1Range > 0f ? _form1Range : _range;

            return _form2Range > 0f ? _form2Range : _range;
        }

        private int GetDamage(float power01)
        {
            if (_forms != null && _forms.CurrentForm == PlayerForm.Form1)
                return Mathf.RoundToInt(Mathf.Lerp(_form1DamageMin, _form1DamageMax, power01));

            return Mathf.RoundToInt(Mathf.Lerp(_form2DamageMin, _form2DamageMax, power01));
        }

        private bool IsSelf(Collider col)
        {
            return col != null && _selfRoot != null && col.transform.IsChildOf(_selfRoot);
        }

        private bool RaycastIgnoreSelf(Vector3 origin, Vector3 dir, float maxDistance, out RaycastHit bestHit)
        {
            int count = Physics.RaycastNonAlloc(new Ray(origin, dir), _hits, maxDistance, _aimMask, QueryTriggerInteraction.Ignore);

            int bestIndex = -1;
            float bestDist = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                RaycastHit h = _hits[i];
                if (IsSelf(h.collider))
                    continue;

                if (h.distance < bestDist)
                {
                    bestDist = h.distance;
                    bestIndex = i;
                }
            }

            if (bestIndex >= 0)
            {
                bestHit = _hits[bestIndex];
                return true;
            }

            bestHit = default;
            return false;
        }

        private void ApplyDamageFirst(Vector3 origin, Vector3 dir, float distance, int damage, LayerMask mask)
        {
            if (damage < 1)
                damage = 1;

            if (!RaycastIgnoreSelf(origin, dir, distance, out RaycastHit hit))
                return;

            var target = hit.collider.GetComponentInParent<TargetHealth>();
            if (target != null)
                target.ApplyDamage(damage, DamageInstigator.Player);
        }

        private void ApplyDamageAll(Vector3 origin, Vector3 dir, float distance, int damage, LayerMask mask)
        {
            if (damage < 1)
                damage = 1;

            int count = Physics.RaycastNonAlloc(new Ray(origin, dir), _damageHits, distance, mask, QueryTriggerInteraction.Ignore);
            if (count <= 0)
                return;

            Array.Sort(_damageHits, 0, count, RaycastHitDistanceComparer.Instance);

            _damaged.Clear();

            for (int i = 0; i < count; i++)
            {
                RaycastHit h = _damageHits[i];

                if (IsSelf(h.collider))
                    continue;

                var target = h.collider.GetComponentInParent<TargetHealth>();
                if (target == null)
                    continue;

                if (_damaged.Add(target))
                    target.ApplyDamage(damage, DamageInstigator.Player);
            }
        }

        private sealed class RaycastHitDistanceComparer : IComparer<RaycastHit>
        {
            public static readonly RaycastHitDistanceComparer Instance = new RaycastHitDistanceComparer();

            public int Compare(RaycastHit a, RaycastHit b)
            {
                return a.distance.CompareTo(b.distance);
            }
        }
    }
}
