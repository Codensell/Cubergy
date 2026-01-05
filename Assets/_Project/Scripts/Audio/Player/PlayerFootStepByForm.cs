using UnityEngine;

namespace Cubergy.Audio
{
    public sealed class PlayerFootstepsByForm : MonoBehaviour
    {
        [SerializeField] private Player.PlayerFormController _forms;
        [SerializeField] private Player.PlayerMovement _movement;
        [SerializeField] private PlayerAudioEmitter _audio;

        [SerializeField] private AudioClip _form0Footsteps;
        [SerializeField] private AudioClip _form1Footsteps;

        [SerializeField] private float _moveThreshold = 0.2f;

        private void Update()
        {
            if (!_movement.IsGrounded)
            {
                _audio.StopFootstepsLoop();
                return;
            }

            if (_forms == null || _movement == null || _audio == null)
                return;

            bool moving = IsMoving();

            if (!moving)
            {
                _audio.StopFootstepsLoop();
                return;
            }

            var form = _forms.CurrentForm;

            if (form == Player.PlayerForm.Form0)
            {
                _audio.StartFootstepsLoop(_form0Footsteps, 1f, 1f);
                return;
            }

            if (form == Player.PlayerForm.Form1)
            {
                _audio.StartFootstepsLoop(_form1Footsteps, 1f, 0.95f);
                return;
            }

            _audio.StopFootstepsLoop();
        }

        private bool IsMoving()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector2 v = new Vector2(x, z);
            return v.magnitude >= _moveThreshold;
        }
    }
}