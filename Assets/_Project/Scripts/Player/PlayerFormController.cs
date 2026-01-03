using System;
using Cubergy.Energy;
using UnityEngine;

namespace Cubergy.Player
{
    public enum PlayerForm
    {
        Form0 = 0,
        Form1 = 1,
        Form2 = 2
    }

    public sealed class PlayerFormController : MonoBehaviour
    {
        [SerializeField] private EnergyWallet _wallet;

        [SerializeField] private GameObject _form0Visual;
        [SerializeField] private GameObject _form1Visual;
        [SerializeField] private GameObject _form2Visual;

        [SerializeField] private HoverBobbing _form0Bobbing;

        [SerializeField] private int _toForm1Energy = 10;
        [SerializeField] private int _toForm2Energy = 10;

        public event Action<PlayerForm> FormChanged;

        public PlayerForm CurrentForm { get; private set; } = PlayerForm.Form0;

        private void OnEnable()
        {
            if (_wallet != null)
                _wallet.Changed += OnEnergyChanged;

            ApplyVisuals();
        }

        private void OnDisable()
        {
            if (_wallet != null)
                _wallet.Changed -= OnEnergyChanged;
        }

        private void OnEnergyChanged(int value)
        {
            if (CurrentForm == PlayerForm.Form0 && value >= _toForm1Energy)
            {
                SwitchTo(PlayerForm.Form1);
                return;
            }

            if (CurrentForm == PlayerForm.Form1 && value >= _toForm2Energy)
            {
                SwitchTo(PlayerForm.Form2);
            }
        }

        private void SwitchTo(PlayerForm form)
        {
            CurrentForm = form;

            if (_wallet != null)
                _wallet.ResetToZero();

            ApplyVisuals();
            FormChanged?.Invoke(CurrentForm);
        }

        private void ApplyVisuals()
        {
            if (_form0Visual != null) _form0Visual.SetActive(CurrentForm == PlayerForm.Form0);
            if (_form1Visual != null) _form1Visual.SetActive(CurrentForm == PlayerForm.Form1);
            if (_form2Visual != null) _form2Visual.SetActive(CurrentForm == PlayerForm.Form2);

            if (_form0Bobbing != null)
                _form0Bobbing.enabled = CurrentForm == PlayerForm.Form0;
        }
    }
}
