using UnityEngine;

namespace Cubergy.Player
{
    public sealed class PlayerVisualOffsetByForm : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private Transform _visualRoot;

        [SerializeField] private float _form0Y;
        [SerializeField] private float _form1Y;
        [SerializeField] private float _form2Y;

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
            if (_visualRoot == null)
                return;

            float y = form switch
            {
                PlayerForm.Form0 => _form0Y,
                PlayerForm.Form1 => _form1Y,
                PlayerForm.Form2 => _form2Y,
                _ => 0f
            };

            Vector3 p = _visualRoot.localPosition;
            _visualRoot.localPosition = new Vector3(p.x, y, p.z);
        }
    }
}