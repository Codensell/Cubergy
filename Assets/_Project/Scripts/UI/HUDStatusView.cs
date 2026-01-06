using Cubergy.Energy;
using Cubergy.Player;
using Cubergy.Combat;
using TMPro;
using UnityEngine;

namespace Cubergy.UI
{
    public sealed class HudStatusView : MonoBehaviour
    {
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
            _energyPop?.Play();
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

            if (_forms != null && _forms.CurrentForm == PlayerForm.Form2)
                return;

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
            int energy = _wallet != null ? _wallet.Current : 0;
            int goal = GetGoalForCurrentForm();

            if (_energyText != null)
            {
                if (goal <= 0)
                {
                    _energyText.text = "Energy: —";
                }
                else
                {
                    int clamped = energy;
                    if (clamped > goal)
                        clamped = goal;

                    _energyText.text = $"Energy: {clamped}/{goal}";
                }
            }

            if (_formText != null)
            {
                int index = _forms != null ? (int)_forms.CurrentForm : 0;
                _formText.text = $"Form: {index + 1}/3";
            }
            if (_healthText != null)
            {
                int hp = _health != null ? _health.Current : 0;
                int max = _health != null ? _health.Max : 0;
                _healthText.text = $"Health: {hp}/{max}";
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
            if (s < 0.01f) s = 0.01f;

            yield return new UnityEngine.WaitForSeconds(s);

            if (_healthText != null)
                _healthText.color = _healthBaseColor;

            _healthFlashRoutine = null;
        }

    }
}