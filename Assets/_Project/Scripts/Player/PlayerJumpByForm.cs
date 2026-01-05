using UnityEngine;

namespace Cubergy.Player
{
    public sealed class PlayerJumpByForm : MonoBehaviour
    {
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private PlayerMovement _movement;

        [SerializeField] private float _form1JumpHeight = 1.6f;

        private void Update()
        {
            if (_forms == null || _movement == null)
                return;

            if (_forms.CurrentForm != PlayerForm.Form1)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
                _movement.Jump(_form1JumpHeight);
        }
    }
}