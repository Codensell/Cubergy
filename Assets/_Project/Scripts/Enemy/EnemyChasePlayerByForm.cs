using Cubergy.Player;
using UnityEngine;

namespace Cubergy.Enemy
{
    [DisallowMultipleComponent]
    public sealed class EnemyChasePlayerByForm : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private EnemyMoveToTarget _mover;
        [SerializeField] private MonoBehaviour _energyCubeTargeter;
        [SerializeField] private Transform _player;

        private void Awake()
        {
            if (_forms == null) _forms = GetComponent<PlayerFormController>();
            if (_mover == null) _mover = GetComponent<EnemyMoveToTarget>();
        }

        private void OnEnable()
        {
            if (_forms != null)
                _forms.FormChanged += OnFormChanged;

            Apply(_forms != null ? _forms.CurrentForm : PlayerForm.Form0);
        }

        private void OnDisable()
        {
            if (_forms != null)
                _forms.FormChanged -= OnFormChanged;
        }

        private void LateUpdate()
        {
            if (_forms == null || _mover == null || _player == null)
                return;

            if (_forms.CurrentForm == PlayerForm.Form2)
                _mover.SetTarget(_player);
        }

        public void SetPlayer(Transform player)
        {
            _player = player;
            Apply(_forms != null ? _forms.CurrentForm : PlayerForm.Form0);
        }

        private void OnFormChanged(PlayerForm form) => Apply(form);

        private void Apply(PlayerForm form)
        {
            if (_energyCubeTargeter != null)
                _energyCubeTargeter.enabled = form != PlayerForm.Form2;

            if (_mover == null)
                return;

            if (form == PlayerForm.Form2 && _player != null)
                _mover.SetTarget(_player);
        }
    }
}