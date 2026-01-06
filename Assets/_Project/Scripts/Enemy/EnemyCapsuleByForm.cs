using Cubergy.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Cubergy.Enemy
{
    [DisallowMultipleComponent]
    public sealed class EnemyCapsuleByForm : MonoBehaviour
    {
        [System.Serializable]
        private sealed class CapsuleSettings
        {
            [Min(0.01f)] public float Radius = 0.6f;
            [Min(0.01f)] public float Height = 2.0f;
            public float CenterY = 1.0f;

            [Min(0.01f)] public float AgentRadius = 0.6f;
            [Min(0.01f)] public float AgentHeight = 2.0f;
            public float AgentBaseOffset = 0.0f;
        }

        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private CharacterController _cc;
        [SerializeField] private NavMeshAgent _agent;

        [Header("Per Form Settings")]
        [SerializeField] private CapsuleSettings _form0 = new CapsuleSettings();
        [SerializeField] private CapsuleSettings _form1 = new CapsuleSettings();
        [SerializeField] private CapsuleSettings _form2 = new CapsuleSettings
        {
            Radius = 1.2f,
            Height = 6.0f,
            CenterY = 3.0f,
            AgentRadius = 1.2f,
            AgentHeight = 6.0f,
            AgentBaseOffset = 0.0f
        };

        private void Awake()
        {
            if (_forms == null) _forms = GetComponent<PlayerFormController>();
            if (_cc == null) _cc = GetComponent<CharacterController>();
            if (_agent == null) _agent = GetComponent<NavMeshAgent>();
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
            CapsuleSettings s = form switch
            {
                PlayerForm.Form0 => _form0,
                PlayerForm.Form1 => _form1,
                PlayerForm.Form2 => _form2,
                _ => _form0
            };

            if (_cc != null)
            {
                _cc.radius = Mathf.Max(0.01f, s.Radius);
                _cc.height = Mathf.Max(0.01f, s.Height);
                _cc.center = new Vector3(0f, s.CenterY, 0f);
            }

            if (_agent != null)
            {
                _agent.radius = Mathf.Max(0.01f, s.AgentRadius);
                _agent.height = Mathf.Max(0.01f, s.AgentHeight);
                _agent.baseOffset = s.AgentBaseOffset;
            }
        }
    }
}
