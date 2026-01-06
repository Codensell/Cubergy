using Cubergy.Combat;
using Cubergy.Energy;
using Cubergy.Player;
using TMPro;
using UnityEngine;

namespace Cubergy.UI
{
    public sealed class HudStatusView : MonoBehaviour
    {
        private const int EnergyGoal = 10;

        [SerializeField] private EnergyWallet _wallet;
        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private TextPopScale _energyPop;
        [SerializeField] private TextPopScale _formPop;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private TMP_Text _formText;

        [SerializeField] private PlayerHealth _health;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TextPopScale _healthPop;

        [Header("Health Flash")]
        [SerializeField] private Color _healthFlashColor = new Color(1f, 0.25f, 0.25f, 1f);
        [SerializeField] private float _healthFlashSeconds = 2f;

        private Color _healthBaseColor;
        private Coroutine _healthFlashRoutine;

        private void OnEnable()
        {
            if (_wallet != null)
                _wallet.Changed += OnEnergyChanged;

            if (_forms != null)
                _forms.FormChanged += OnFormChanged;

            if (_health != null)
                _health.Changed += OnHealthChanged;

            if (_healthText != null)
                _healthBaseColor = _healthText.color;

            Refresh();
        }

        private void OnDisable()
        {
            if (_wallet != null)
                _wallet.Changed -= OnEnergyChanged;

            if (_forms != null)
                _forms.FormChanged -= OnFormChanged;

            if (_health != null)
                _health.Changed -= OnHealthChanged;

            if (_healthFlashRoutine != null)
            {
                StopCoroutine(_healthFlashRoutine);
                _healthFlashRoutine = null;
            }

            if (_healthText != null)
                _healthText.color = _healthBaseColor;
        }

        private void OnEnergyChanged(int _)
        {
            Refresh();
            _energyPop?.Play();
        }

        private void OnFormChanged(PlayerForm _)
        {
            Refresh();
            _formPop?.Play();
        }

        private void OnHealthChanged(int _, int __)
        {
            Refresh();

            if (_ != __)
            {
                _healthPop?.Play();
                PlayHealthFlash();
            }
        }

        private void Refresh()
        {
            // Energy: всегда 0..10 / 10
            int energy = _wallet != null ? _wallet.Current : 0;
            int clampedEnergy = Mathf.Clamp(energy, 0, EnergyGoal);

            if (_energyText != null)
                _energyText.text = $"Energy: {clampedEnergy}/{EnergyGoal}";

            // Form
            if (_formText != null)
            {
                int index = _forms != null ? (int)_forms.CurrentForm : 0;
                _formText.text = $"Form: {index + 1}/3";
            }

            // Health
            if (_healthText != null)
            {
                int hp = _health != null ? _health.Current : 0;
                int max = _health != null ? _health.Max : 0;
                _healthText.text = $"Health: {hp}/{max}";
            }
        }

        private void PlayHealthFlash()
        {
            if (_healthText == null)
                return;

            if (_healthFlashRoutine != null)
                StopCoroutine(_healthFlashRoutine);

            _healthFlashRoutine = StartCoroutine(HealthFlashRoutine());
        }

        private System.Collections.IEnumerator HealthFlashRoutine()
        {
            _healthText.color = _healthFlashColor;

            float s = _healthFlashSeconds;
            if (s < 0.01f)
                s = 0.01f;

            yield return new WaitForSeconds(s);

            if (_healthText != null)
                _healthText.color = _healthBaseColor;

            _healthFlashRoutine = null;
        }
    }
}
