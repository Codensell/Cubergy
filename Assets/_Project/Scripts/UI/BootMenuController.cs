using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cubergy.Boot
{
    public sealed class BootMenuController : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _instructionButton;

        [Header("Scene Names")]
        [SerializeField] private string _gameSceneName = "Game";
        [SerializeField] private string _instructionSceneName = "Instruction";

        private void OnEnable()
        {
            if (_playButton != null)
                _playButton.onClick.AddListener(LoadGame);

            if (_instructionButton != null)
                _instructionButton.onClick.AddListener(LoadInstruction);
        }

        private void OnDisable()
        {
            if (_playButton != null)
                _playButton.onClick.RemoveListener(LoadGame);

            if (_instructionButton != null)
                _instructionButton.onClick.RemoveListener(LoadInstruction);
        }

        private void LoadGame() => SceneManager.LoadScene(_gameSceneName);
        private void LoadInstruction() => SceneManager.LoadScene(_instructionSceneName);
    }
}