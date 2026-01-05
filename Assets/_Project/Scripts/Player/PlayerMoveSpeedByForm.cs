using UnityEngine;

namespace Cubergy.Player
{
    public sealed class PlayerMoveSpeedByForm : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private PlayerMovement _movement;

        [SerializeField] private float _form0Speed = 15f;
        [SerializeField] private float _form1Speed = 10f;
        [SerializeField] private float _form2Speed = 10f;

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

        private void OnFormChanged(PlayerForm form)
        {
            Apply(form);
        }

        private void Apply(PlayerForm form)
        {
            if (_movement == null)
                return;

            float speed = form switch
            {
                PlayerForm.Form0 => _form0Speed,
                PlayerForm.Form1 => _form1Speed,
                PlayerForm.Form2 => _form2Speed,
                _ => _form0Speed
            };

            _movement.SetMoveSpeed(speed);
        }
    }
}