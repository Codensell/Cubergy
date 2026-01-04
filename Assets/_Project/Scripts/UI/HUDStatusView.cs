using Cubergy.Energy;
using Cubergy.Player;
using TMPro;
using UnityEngine;

namespace Cubergy.UI
{
    public sealed class HudStatusView : MonoBehaviour
    {
        [SerializeField] private EnergyWallet _wallet;
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private TextPopScale _energyPop;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private TMP_Text _formText;

        private void OnEnable()
        {
            if (_wallet != null)
                _wallet.Changed += OnEnergyChanged;

            if (_forms != null)
                _forms.FormChanged += OnFormChanged;

            Refresh();
            _energyPop?.Play();
        }

        private void OnDisable()
        {
            if (_wallet != null)
                _wallet.Changed -= OnEnergyChanged;

            if (_forms != null)
                _forms.FormChanged -= OnFormChanged;
        }

        private void OnEnergyChanged(int _)
        {
            Refresh();
            _energyPop?.Play();
        }


        private void OnFormChanged(PlayerForm _)
        {
            Refresh();
        }

        private void Refresh()
        {
            int energy = _wallet != null ? _wallet.Current : 0;
            int goal = GetGoalForCurrentForm();

            if (_energyText != null)
                _energyText.text = $"Energy: {energy}/{goal}";

            if (_formText != null)
            {
                int index = _forms != null ? (int)_forms.CurrentForm : 0;
                _formText.text = $"Form: {index + 1}/3";
            }
        }

        private int GetGoalForCurrentForm()
        {
            if (_forms == null)
                return 10;

            return _forms.CurrentForm switch
            {
                PlayerForm.Form0 => 10,
                PlayerForm.Form1 => 10,
                PlayerForm.Form2 => 0,
                _ => 10
            };
        }
    }
}