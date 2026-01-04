using Cubergy.Player;
using UnityEngine;

namespace Cubergy.Combat
{
    public sealed class PlayerHealthByForm : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private PlayerHealth _health;

        [SerializeField] private int _form0MaxHp = 3;
        [SerializeField] private int _form1MaxHp = 6;
        [SerializeField] private int _form2MaxHp = 10;

        [SerializeField] private bool _fillToMaxOnUpgrade = true;

        private void OnEnable()
        {
            if (_forms != null)
                _forms.FormChanged += OnFormChanged;

            Apply(_forms != null ? _forms.CurrentForm : PlayerForm.Form0, true);
        }

        private void OnDisable()
        {
            if (_forms != null)
                _forms.FormChanged -= OnFormChanged;
        }

        private void OnFormChanged(PlayerForm form)
        {
            Apply(form, _fillToMaxOnUpgrade);
        }

        private void Apply(PlayerForm form, bool fillToMax)
        {
            if (_health == null)
                return;

            int max = form switch
            {
                PlayerForm.Form0 => _form0MaxHp,
                PlayerForm.Form1 => _form1MaxHp,
                PlayerForm.Form2 => _form2MaxHp,
                _ => _form0MaxHp
            };

            _health.SetMax(max, fillToMax);
        }
    }
}