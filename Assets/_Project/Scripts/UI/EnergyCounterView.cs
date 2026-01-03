using Cubergy.Energy;
using TMPro;
using UnityEngine;

namespace Cubergy.UI
{
    public sealed class EnergyCounterView : MonoBehaviour
    {
        [SerializeField] private EnergyWallet _wallet;
        [SerializeField] private TMP_Text _text;

        private void OnEnable()
        {
            if (_wallet != null)
                _wallet.Changed += OnChanged;

            Refresh();
        }

        private void OnDisable()
        {
            if (_wallet != null)
                _wallet.Changed -= OnChanged;
        }

        private void OnChanged(int value)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_text == null)
                return;

            int value = _wallet != null ? _wallet.Current : 0;
            _text.text = $"Energy: {value}";
        }
    }
}