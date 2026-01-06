using System;
using Cubergy.Player;
using UnityEngine;

namespace Cubergy.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerCapsuleByForm : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private CharacterController _cc;

        [Header("Per Form Settings")]
        [SerializeField] private CapsuleSettings _form0 = CapsuleSettings.Default;
        [SerializeField] private CapsuleSettings _form1 = CapsuleSettings.Default;
        [SerializeField] private CapsuleSettings _form2 = CapsuleSettings.Large;

        private void Awake()
        {
            if (_forms == null)
                _forms = GetComponent<PlayerFormController>();

            if (_cc == null)
                _cc = GetComponent<CharacterController>();
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

        private void OnFormChanged(PlayerForm form) => Apply(form);

        private void Apply(PlayerForm form)
        {
            if (_cc == null)
                return;

            CapsuleSettings s = form switch
            {
                PlayerForm.Form0 => _form0,
                PlayerForm.Form1 => _form1,
                PlayerForm.Form2 => _form2,
                _ => _form0
            };

            s = s.Sanitized();

            _cc.radius = s.Radius;
            _cc.height = s.Height;
            _cc.center = new Vector3(_cc.center.x, s.CenterY, _cc.center.z);
        }

        [Serializable]
        private struct CapsuleSettings
        {
            public float Radius;
            public float Height;
            public float CenterY;

            public static CapsuleSettings Default => new CapsuleSettings
            {
                Radius = 0.6f,
                Height = 2f,
                CenterY = 1f
            };

            public static CapsuleSettings Large => new CapsuleSettings
            {
                Radius = 1.2f,
                Height = 6f,
                CenterY = 3f
            };

            public CapsuleSettings Sanitized()
            {
                if (Radius < 0.05f) Radius = 0.05f;
                if (Height < 0.2f) Height = 0.2f;

                float minHeight = 2f * Radius;
                if (Height < minHeight) Height = minHeight;

                return this;
            }
        }
    }
}
