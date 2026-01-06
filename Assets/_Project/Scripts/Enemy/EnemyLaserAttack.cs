using Cubergy.Combat;
using Cubergy.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Cubergy.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EnemyLaserAttack : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;

        [Header("Target")]
        [SerializeField] private Transform _player;
        [SerializeField] private float _aimHeight = 1.0f;

        [Header("Muzzles")]
        [SerializeField] private Transform _form1Muzzle;
        [SerializeField] private Transform _form2Muzzle;

        [Header("Ranges")]
        [SerializeField] private float _form1AttackRange = 18f;
        [SerializeField] private float _form2AttackRange = 30f;

        [Header("Damage")]
        [SerializeField] private int _form1Damage = 1;
        [SerializeField] private int _form2Damage = 2;

        [Header("Cadence")]
        [SerializeField] private float _stopBeforeFire = 0.12f;
        [SerializeField] private float _moveTimeBetweenShots = 0.6f;

        [Header("Cast & Cooldown")]
        [SerializeField] private float _castTime = 1.5f;
        [SerializeField] private float _cooldownAfterShot = 2.0f;
        [SerializeField] private bool _lockAimPointOnCastStart = true;
        [SerializeField] private bool _requireLineOfSight = true;

        [Header("Aim & Raycast")]
        [SerializeField] private LayerMask _aimMask = ~0;
        [SerializeField] private Transform _selfRoot;

        [Header("VFX")]
        [SerializeField] private LaserShotVfx _laserVfx;
        [SerializeField] private float _laserWidth = 0.08f;
        [SerializeField] private float _laserDuration = 0.06f;

        [Header("NavMesh Recovery")]
        [SerializeField] private float _snapToNavMeshRadius = 2.0f;

        private NavMeshAgent _agent;

        private float _cooldownT;
        private float _castT;
        private float _preCastDelayT;

        private bool _isCasting;
        private bool _wasStopping;

        private Vector3 _lockedAimPoint;

        private readonly RaycastHit[] _hits = new RaycastHit[32];

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            if (_forms == null)
                _forms = GetComponent<PlayerFormController>();

            if (_selfRoot == null)
                _selfRoot = transform;
        }

        // ✅ ВОТ ЭТОГО МЕТОДА НЕ ХВАТАЛО (его ждут твои биндеры/линкеры)
        public void SetPlayer(Transform player)
        {
            _player = player;
            CancelCastAndResume();
        }

        private void OnEnable()
        {
            TrySnapAgentToNavMesh();
            CancelCastAndResume();
        }

        private void OnDisable()
        {
            _wasStopping = false;
            _isCasting = false;
            _castT = 0f;
            _preCastDelayT = 0f;
        }

        private void Update()
        {
            if (_forms == null || _player == null || _agent == null)
                return;

            if (!_agent.enabled)
                return;

            if (!_agent.isOnNavMesh)
            {
                TrySnapAgentToNavMesh();
                if (!_agent.isOnNavMesh)
                    return;
            }

            PlayerForm form = _forms.CurrentForm;

            if (form == PlayerForm.Form0)
            {
                CancelCastAndResume();
                return;
            }

            if (_cooldownT > 0f)
            {
                _cooldownT -= Time.deltaTime;
                CancelCastAndResume();
                return;
            }

            float range = form == PlayerForm.Form1 ? _form1AttackRange : _form2AttackRange;

            Vector3 from = transform.position;
            Vector3 to = _player.position + new Vector3(0f, _aimHeight, 0f);

            Vector3 flat = to - from;
            flat.y = 0f;

            bool inRange = flat.sqrMagnitude <= range * range;
            if (!inRange)
            {
                CancelCastAndResume();
                return;
            }

            if (_requireLineOfSight)
            {
                Transform muzzleForLos = form == PlayerForm.Form1 ? _form1Muzzle : _form2Muzzle;

                Vector3 origin = muzzleForLos != null ? muzzleForLos.position : transform.position;
                Vector3 dir = to - origin;
                float dist = dir.magnitude;

                if (dist > 0.001f)
                {
                    dir /= dist;

                    if (RaycastIgnoreSelf(origin, dir, dist, out RaycastHit hit))
                    {
                        if (hit.collider.GetComponentInParent<PlayerDamageReceiver>() == null)
                        {
                            CancelCastAndResume();
                            return;
                        }
                    }
                    else
                    {
                        CancelCastAndResume();
                        return;
                    }
                }
            }

            StopIfNeeded();

            if (!_isCasting && _preCastDelayT <= 0f)
                _preCastDelayT = Mathf.Max(0f, _stopBeforeFire);

            if (!_isCasting && _preCastDelayT > 0f)
            {
                _preCastDelayT -= Time.deltaTime;
                if (_preCastDelayT > 0f)
                    return;
            }

            if (!_isCasting)
            {
                _isCasting = true;
                _castT = Mathf.Max(0.01f, _castTime);
                _lockedAimPoint = _lockAimPointOnCastStart ? to : Vector3.zero;
            }

            _castT -= Time.deltaTime;
            if (_castT > 0f)
                return;

            Vector3 targetPoint = _lockAimPointOnCastStart ? _lockedAimPoint : to;
            Fire(form, targetPoint);

            _isCasting = false;
            _preCastDelayT = 0f;

            ResumeIfNeeded();

            _cooldownT = Mathf.Max(0.05f, _cooldownAfterShot);
        }

        private void CancelCastAndResume()
        {
            _isCasting = false;
            _castT = 0f;
            _preCastDelayT = 0f;
            ResumeIfNeeded();
        }

        private void StopIfNeeded()
        {
            if (_wasStopping)
                return;

            if (_agent.enabled && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _wasStopping = true;
            }
        }

        private void ResumeIfNeeded()
        {
            if (!_wasStopping)
                return;

            if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
                _agent.isStopped = false;

            _wasStopping = false;
        }

        private void TrySnapAgentToNavMesh()
        {
            if (_agent == null || !_agent.enabled)
                return;

            if (_agent.isOnNavMesh)
                return;

            if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, _snapToNavMeshRadius, NavMesh.AllAreas))
                return;

            _agent.Warp(hit.position);
        }

        private void Fire(PlayerForm form, Vector3 targetPoint)
        {
            Transform muzzle = form == PlayerForm.Form1 ? _form1Muzzle : _form2Muzzle;
            if (muzzle == null)
                return;

            Vector3 origin = muzzle.position;
            Vector3 dir = (targetPoint - origin).normalized;
            if (dir.sqrMagnitude < 0.0001f)
                return;

            float maxDist = form == PlayerForm.Form1 ? _form1AttackRange : _form2AttackRange;

            float beamLen = maxDist;
            if (RaycastIgnoreSelf(origin, dir, maxDist, out RaycastHit hit))
                beamLen = hit.distance;

            if (_laserVfx != null)
                _laserVfx.Play(origin, dir, _laserWidth, beamLen, _laserDuration);

            if (!RaycastIgnoreSelf(origin, dir, maxDist, out RaycastHit best))
                return;

            var receiver = best.collider.GetComponentInParent<PlayerDamageReceiver>();
            if (receiver == null)
                return;

            int dmg = form == PlayerForm.Form1 ? _form1Damage : _form2Damage;
            receiver.ApplyDamage(dmg);
        }

        private bool IsSelf(Collider col)
        {
            return col != null && _selfRoot != null && col.transform.IsChildOf(_selfRoot);
        }

        private bool RaycastIgnoreSelf(Vector3 origin, Vector3 dir, float maxDistance, out RaycastHit bestHit)
        {
            int count = Physics.RaycastNonAlloc(
                new Ray(origin, dir),
                _hits,
                maxDistance,
                _aimMask,
                QueryTriggerInteraction.Ignore);

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
    }
}
